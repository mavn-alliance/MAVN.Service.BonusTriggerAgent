using Lykke.HttpClientGenerator;

namespace MAVN.Service.BonusTriggerAgent.Client
{
    /// <summary>
    /// BonusTriggerAgent API aggregating interface.
    /// </summary>
    public class BonusTriggerAgentClient : IBonusTriggerAgentClient
    {
        // Note: Add similar Api properties for each new service controller

        /// <summary>Inerface to BonusTriggerAgent Api.</summary>
        public IBonusTriggerAgentApi Api { get; private set; }

        /// <summary>C-tor</summary>
        public BonusTriggerAgentClient(IHttpClientGenerator httpClientGenerator)
        {
            Api = httpClientGenerator.Generate<IBonusTriggerAgentApi>();
        }
    }
}
