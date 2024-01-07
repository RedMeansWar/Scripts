using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.SpikesStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        #endregion

        #region Commands
        [Command("setspikes")]
        private void SetSpikeCommand(string[] args)
        {
            if (args.Length == 0)
            {
            }

            if (args.Length == 2)
            {

            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Ticks
        [Tick]
        private async Task CheckDistanceToSpikeProp()
        {

        }
        #endregion
    }
}