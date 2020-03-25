using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.BonusTriggerAgent.Client 
{
    /// <summary>
    /// BonusTriggerAgent client settings.
    /// </summary>
    public class BonusTriggerAgentServiceClientSettings 
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl {get; set;}
    }
}
