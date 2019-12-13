using DasFinanceCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DasFinance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PaymentTransactionController : ControllerBase
    {
        private readonly DasFinanceCosmosHelper cosmosDb;

        public PaymentTransactionController(DasFinanceCosmosHelper cosmosDb)
        {
            this.cosmosDb = cosmosDb;
        }

        [HttpPost]
        public async Task<IActionResult> Post(PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null)
            {
                return BadRequest();
            }


            paymentTransaction.Id = Guid.NewGuid();
            paymentTransaction.CreateDate = DateTime.UtcNow;

            await cosmosDb.Init();
            await cosmosDb.AddPaymentTransaction(paymentTransaction);
            return NoContent();
        }
    }
}
