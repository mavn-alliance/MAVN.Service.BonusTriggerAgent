using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.BonusTriggerAgent.Contract.Events;
using Lykke.Service.BonusTriggerAgent.Domain;
using Lykke.Service.Referral.Contract.Events;

namespace Lykke.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class FriendReferralSubscriber
        : RabbitSubscriber<FriendReferralEvent>
    {
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;

        public FriendReferralSubscriber(
            string connectionString,
            string exchangeName,
            ILogFactory logFactory,
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher
                                          ?? throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));

            GuidsFieldsToValidate.Add(nameof(FriendReferralEvent.ReferredId));
            GuidsFieldsToValidate.Add(nameof(FriendReferralEvent.ReferrerId));
        }

        public override async Task<bool> ProcessMessageAsync(FriendReferralEvent message)
        {
            await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
            {
                CustomerId = message.ReferrerId,
                TimeStamp = message.Timestamp,
                Type = BonusTypes.FriendReferred.EventName,
                Data = new Dictionary<string, string>
                {
                    {"Referred", message.ReferredId },
                    {"ReferralId", message.ReferralId}
                }
            });

            return true;
        }
    }
}
