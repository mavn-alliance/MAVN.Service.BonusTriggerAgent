using JetBrains.Annotations;

namespace Lykke.Service.BonusTriggerAgent.Client
{
    /// <summary>
    /// BonusTriggerAgent client interface.
    /// </summary>
    [PublicAPI]
    public interface IBonusTriggerAgentClient
    {
        // Make your app's controller interfaces visible by adding corresponding properties here.
        // NO actual methods should be placed here (these go to controller interfaces, for example - IBonusTriggerAgentApi).
        // ONLY properties for accessing controller interfaces are allowed.

        /// <summary>Application Api interface</summary>
        IBonusTriggerAgentApi Api { get; }
    }
}
