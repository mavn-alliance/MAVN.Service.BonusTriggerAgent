using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using MAVN.Service.BonusTriggerAgent.Contract.Events;
using MAVN.Service.BonusTriggerAgent.Domain;
using MAVN.Service.CustomerProfile.Client;
using MAVN.Service.CustomerProfile.Client.Models.Enums;
using MAVN.Service.CustomerProfile.Contract;

namespace MAVN.Service.BonusTriggerAgent.DomainServices.Subscribers
{
    public class EmailVerificationSubscriber
        : RabbitSubscriber<EmailVerifiedEvent>
    {
        private readonly IRabbitPublisher<BonusTriggerEvent> _bonusTriggerEventPublisher;
        private readonly ICustomerProfileClient _customerProfileClient;
        private readonly bool _isPhoneVerificationDisabled;

        public EmailVerificationSubscriber(
            string connectionString, 
            string exchangeName, 
            ILogFactory logFactory, 
            IRabbitPublisher<BonusTriggerEvent> bonusTriggerEventPublisher,
            ICustomerProfileClient customerProfileClient,
            bool isPhoneVerificationDisabled)
            : base(connectionString, exchangeName, logFactory)
        {
            _bonusTriggerEventPublisher = bonusTriggerEventPublisher ??
                                          throw new ArgumentNullException(nameof(bonusTriggerEventPublisher));
            
            _customerProfileClient = customerProfileClient;
            _isPhoneVerificationDisabled = isPhoneVerificationDisabled;

            GuidsFieldsToValidate.Add(nameof(EmailVerifiedEvent.CustomerId));
        }

        public override async Task<bool> ProcessMessageAsync(EmailVerifiedEvent message)
        {
            if (!message.WasEmailEverVerified)
            {
                var customer = await _customerProfileClient.CustomerProfiles.GetByCustomerIdAsync(message.CustomerId, true);

                if (customer.ErrorCode == CustomerProfileErrorCodes.None)
                {
                    if (_isPhoneVerificationDisabled || (customer.Profile?.IsPhoneVerified ?? false))
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
