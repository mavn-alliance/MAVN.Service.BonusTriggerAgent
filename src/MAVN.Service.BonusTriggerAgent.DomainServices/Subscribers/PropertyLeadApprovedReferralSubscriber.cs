using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using Lykke.Service.Referral.Contract.Events;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class PropertyLeadApprovedReferralSubscriber : RabbitSubscriber<PropertyLeadApprovedReferralEvent>
    {
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        
        public PropertyLeadApprovedReferralSubscriber(
            string connectionString,
            string exchangeName,
            ILogFactory logFactory,
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher ??
                throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));

            GuidsFieldsToValidate.Add(nameof(PropertyLeadApprovedReferralEvent.ReferrerId));
        }

        public override async Task<bool> ProcessMessageAsync(PropertyLeadApprovedReferralEvent message)
        {
            var bonusTriggerEvent = new BonusTriggerEvent
            {
                CustomerId = message.ReferrerId,
                TimeStamp = message.TimeStamp,
                Type = BonusTypes.PropertyLeadApprovedReferral.EventName,
                Data = new Dictionary<string, string>()
                {
                    {"ReferralId", message.ReferralId}
                }
            };

            if (message.StakedCampaignId.HasValue)
            {
                bonusTriggerEvent.Data.Add("StakedCampaignId", message.StakedCampaignId.Value.ToString());
            }

            await _bonusTriggerEventPublisher.PublishAsync(bonusTriggerEvent);

            return true;
        }
    }
}
