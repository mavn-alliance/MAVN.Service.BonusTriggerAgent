using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using Lykke.Service.CustomerProfile.Client;
using Lykke.Service.CustomerProfile.Client.Models.Enums;
using Lykke.Service.CustomerProfile.Contract;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class CustomerPhoneVerifiedSubscriber
        : RabbitSubscriber<CustomerPhoneVerifiedEvent>
    {
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;

        public CustomerPhoneVerifiedSubscriber(
            string connectionString, 
            string exchangeName, 
            ILogFactory logFactory, 
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICustomerProfileClient customerProfileClient)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher
                                          ?? throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
            
            _customerProfileClient = customerProfileClient;

            GuidsFieldsToValidate.Add(nameof(CustomerPhoneVerifiedEvent.CustomerId));
        }

        public override async Task<bool> ProcessMessageAsync(CustomerPhoneVerifiedEvent message)
        {
            if (message.WasPhoneEverVerified)
                return true;
            
            var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(message.CustomerId, true);
                
            if (customer.ErrorCode == CustomerProfileErrorCodes.None)
            {
                if (customer.Profile?.IsEmailVerified ?? false)
                {
                    await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
                    {
                        CustomerId = message.CustomerId,
                        TimeStamp = message.Timestamp,
                        Type = BonusTypes.SignUp.EventName,
                        Data = new Dictionary<string, string>()
                    });
                }
            }

            return true;
        }
    }
}
