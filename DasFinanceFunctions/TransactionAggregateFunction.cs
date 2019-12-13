using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasFinanceCore;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DasFinanceFunctions
{
    public static class TransactionAggregateFunction
    {
        [FunctionName("TransactionAggregateFunction")]
        public static async void Run([TimerTrigger("*/30 * * * * *")]TimerInfo myTimer, ILogger logger)
        {
            using (var cosmosClient = new CosmosClient("https://localhost:8081", "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="))
            {

                var res = await cosmosClient.CreateDatabaseIfNotExistsAsync("Transactions", 10000);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var containerResult = await res.Database.CreateContainerIfNotExistsAsync("PaymentTransactions", "/id");
                    if (containerResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var container = containerResult.Container;

                        QueryDefinition query = new
                            QueryDefinition("select * from PaymentTransactions s where s.CreateDate >= @start and s.CreateDate < @end")
                           .WithParameter("@start", DateTime.UtcNow.Date.AddDays(-1))
                           .WithParameter("@end", DateTime.UtcNow.Date.AddDays(1));

                        FeedIterator<PaymentTransaction> resultSet = container.GetItemQueryIterator<PaymentTransaction>(query);

                        List<PaymentTransaction> allTransactions = new List<PaymentTransaction>();

                        while (resultSet.HasMoreResults)
                        {
                            var nextres = await resultSet.ReadNextAsync();
                            //PaymentTransaction transaction = nextres.FirstOrDefault();
                            //Console.WriteLine($"\n Transaction: {transaction.Id}");
                            allTransactions.AddRange(nextres.ToList());
                        }

                        foreach (var trans in allTransactions)
                        {
                            Console.WriteLine($"\n Transaction: {trans.Id}");
                        }

                        var settlements = allTransactions.GroupBy(t => t.AccountId).Select(g => new Settlement
                        {
                            Date = DateTime.UtcNow.Date.AddDays(-1),
                            AccountId = g.Key,
                            TotalAmount = g.Sum(tr =>
                            {
                                var amount = tr.IsReverse ? -tr.Amount : tr.Amount;
                                return amount;
                            }),
                            Id = Guid.NewGuid()
                        }).ToList();

                        await res.Database.SaveSettlement(settlements);

                    }
                    else
                    {
                        logger.LogCritical("Cannot create container");
                    }
                }
                else
                {
                    logger.LogCritical("Cannot connect to database");
                }
            }
        }

        private static async Task SaveSettlement(this Database database, List<Settlement> settlements)
        {
            var containerResult  = await database.CreateContainerIfNotExistsAsync("Settlement", "/AccountId");

            if (containerResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                foreach (Settlement settlement in settlements)
                {
                    await containerResult.Container.CreateItemAsync(settlement);
                }
            }
        }
    }
}
