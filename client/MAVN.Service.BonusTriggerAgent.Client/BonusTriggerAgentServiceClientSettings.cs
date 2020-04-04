using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.BonusTriggerAgent.Client 
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
