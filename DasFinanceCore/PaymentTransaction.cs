using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasFinanceCore
{
    public class PaymentTransaction : ICosmosType
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string AccountId { get; set; }
        public bool IsReverse { get; set; }
        public decimal Amount { get; set; }

        public string CosmosType => nameof(PaymentTransaction);
    }

    public class Settlement : ICosmosType
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string AccountId { get; set; }
        public decimal TotalAmount { get; set; }

        public string CosmosType => nameof(Settlement);
    }

    // trick to save different types under one container
    public interface ICosmosType
    {
         string CosmosType { get;  }
    }
}
