using System;

namespace Lykke.Service.BonusTriggerAgent.Domain.Models
{
    public class PurchaseReferralModel
    {
        public string CustomerId { get; set; }
        public string ReferrerId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ReferralId { get; set; }
    }
}
