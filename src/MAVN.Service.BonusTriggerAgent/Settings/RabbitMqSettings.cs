using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.BonusTriggerAgent.Settings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }

        [AmqpCheck]
        public string PartnersIntegrationConnectionString { get; set; }

        [AmqpCheck]
        public string ReferralConnectionString { get; set; }
    }
}
