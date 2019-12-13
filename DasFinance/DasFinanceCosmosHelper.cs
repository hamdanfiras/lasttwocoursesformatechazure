using DasFinanceCore;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasFinance
{
    public class DasFinanceCosmosHelper
    {
        CosmosClient cosmosClient;
        DasFinanceCosmosConfig config;
        private readonly ILogger<DasFinanceCosmosHelper> logger;

        public DasFinanceCosmosHelper(IOptions<DasFinanceCosmosConfig> config, ILogger<DasFinanceCosmosHelper> logger)
        {
            this.config = config.Value;
            this.logger = logger;
        }

        public Container Container { get; private set; }

        public async Task Init()
        {
            if (cosmosClient == null)
            {
                cosmosClient = new CosmosClient(this.config.Endpoint, this.config.AuthKey);
                var res = await cosmosClient.CreateDatabaseIfNotExistsAsync(this.config.Database, 10000);
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var containerResult = await res.Database.CreateContainerIfNotExistsAsync(this.config.Container, "/id");
                    if (containerResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        this.Container = containerResult.Container;
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

        public async Task AddPaymentTransaction(PaymentTransaction payment)
        {
            var res = await Container.CreateItemAsync(payment);
            logger.LogError("Could not create payment");
        }
    }

    public class DasFinanceCosmosConfig
    {
        public string Endpoint { get; set; }
        public string AuthKey { get; set; }
        public string Database { get; set; }
        public string Container { get; set; }
    }
}
