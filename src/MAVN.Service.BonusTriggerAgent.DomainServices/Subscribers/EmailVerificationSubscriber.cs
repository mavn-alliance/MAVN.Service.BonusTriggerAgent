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
    public class EmailVerificationSubscriber
        : RabbitSubscriber<EmailVerifiedEvent>
    {
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;

        public EmailVerificationSubscriber(
            string connectionString, 
            string exchangeName, 
            ILogFactory logFactory, 
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICustomerProfileClient customerProfileClient)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher ??
                                          throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
            
            _customerProfileClient = customerProfileClient;

            GuidsFieldsToValidate.Add(nameof(EmailVerifiedEvent.CustomerId));
        }

        public override async Task<bool> ProcessMessageAsync(EmailVerifiedEvent message)
        {
            if (!message.WasEmailEverVerified)
            {
                var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(message.CustomerId, true);

                if (customer.ErrorCode == CustomerProfileErrorCodes.None)
                {
                    if (customer.Profile?.IsPhoneVerified ?? false)
                    {
                        await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
                        {
                            CustomerId = message.CustomerId,
                            TimeStamp = message.TimeStamp,
                            Type = BonusTypes.SignUp.EventName,
                            Data = new Dictionary<string, string>()
                        });
                    }
                }
            }

            await _bonusTriggerEventPublisher.PublishAsync(new BonusTriggerEvent
            {
                CustomerId = message.CustomerId,
                TimeStamp = message.TimeStamp,
                Type = BonusTypes.EmailVerificationTrigger.EventName,
                Data = new Dictionary<string, string>()
            });
            
            return true;
        }
    }
}
