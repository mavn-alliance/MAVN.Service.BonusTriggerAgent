using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.Service.BonusTriggerAgent.Contract.Events;
using Lykke.Service.BonusTriggerAgent.DomainServices.Subscribers;
using Lykke.Service.BonusTriggerAgent.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.BonusTriggerAgent.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private const string PropertyFirstPurchaseExchangeName = "lykke.mavn.propertyintegration.propertypurchasefirstreward";
        private const string ReferralRealEstatePurchasePaymentExchangeName = "lykke.bonus.realestatepurchasepayment";
        private const string PropertyLeadApprovedReferralExchangeName = "lykke.bonus.propertyleadapprovedreferral";
        private const string OfferToPurchaseByLeadExchangeName = "lykke.bonus.purchasereferral.offertopurchasebylead";

        private const string BonusTriggerExchangeName = "lykke.bonus.trigger";
        private const string BonusTypesExchangeName = "lykke.bonus.types";
        private const string CustomerPhoneVerifiedExchangeName = "lykke.customer.phoneverified";
        private const string FriendReferralExchangeName = "lykke.bonus.friendreferral";
        private const string EmailVerifiedExchangeName = "lykke.customer.emailverified";
        private const string HotelCheckoutExchangeName = "lykke.partnersintegration.bonuscustomertrigger";
        private const string HotelCheckoutReferralExchangeName = "lykke.bonus.hotelrferral.referralused";

        private readonly RabbitMqSettings _settings;
        private readonly string _baseCurrencyCode;
        private readonly bool _isRealEstateFeatureDisabled;

        public RabbitMqModule(IReloadingManager<AppSettings> settingsManager)
        {
            var appSettings = settingsManager.CurrentValue;
            _settings = appSettings.BonusTriggerAgentService.RabbitMq;
            _baseCurrencyCode = appSettings.BonusTriggerAgentService.BaseCurrencyCode;
            _isRealEstateFeatureDisabled = appSettings.BonusTriggerAgentService.IsRealEstateFeatureDisabled
                ?? false;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //RabbitMq Publishers
            builder.RegisterJsonRabbitPublisher<BonusTriggerEvent>(
                _settings.RabbitMqConnectionString,
                BonusTriggerExchangeName);

            builder.RegisterJsonRabbitPublisher<BonusTypeDetectedEvent>(
                _settings.RabbitMqConnectionString,
                BonusTypesExchangeName);

            //RabbitMq Subscribers
            builder.RegisterType<CustomerPhoneVerifiedSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", CustomerPhoneVerifiedExchangeName);

            builder.RegisterType<FriendReferralSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", FriendReferralExchangeName);

            builder.RegisterType<EmailVerificationSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", EmailVerifiedExchangeName);

            builder.RegisterType<HotelCheckoutSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.PartnersIntegrationConnectionString)
                .WithParameter("exchangeName", HotelCheckoutExchangeName)
                .WithParameter("assetName", _baseCurrencyCode);

            builder.RegisterType<HotelCheckoutReferralSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.ReferralConnectionString)
                .WithParameter("exchangeName", HotelCheckoutReferralExchangeName)
                .WithParameter("assetName", _baseCurrencyCode);

            if (!_isRealEstateFeatureDisabled)
                RegisterSubscribersForRealEstateFeature(builder);
        }

        private void RegisterSubscribersForRealEstateFeature(ContainerBuilder builder)
        {
            builder.RegisterType<PropertyLeadApprovedReferralSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", PropertyLeadApprovedReferralExchangeName);

            builder.RegisterType<ReferralRealEstatePurchasePaymentSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", ReferralRealEstatePurchasePaymentExchangeName)
                .WithParameter("assetName", _baseCurrencyCode);

            builder.RegisterType<PropertyPurchaseFirstRewardSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", PropertyFirstPurchaseExchangeName);

            builder.RegisterType<OfferToPurchaseByLeadSubscriber>()
                .As<IStartStop>()
                .SingleInstance()
                .WithParameter("connectionString", _settings.RabbitMqConnectionString)
                .WithParameter("exchangeName", OfferToPurchaseByLeadExchangeName)
                .WithParameter("assetName", _baseCurrencyCode);
        }
    }
}
