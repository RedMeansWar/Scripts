using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.SpikeStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        #endregion

        #region Commands
        [Command("setspikes")]
        private void SetSpikesCommand(string[] args)
        {
            string arg = args[0].ToLower();
            
            switch (arg)
            {
                case "2":
                    break;
                case "3":
                    break;
                case "4":
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Ticks
        #endregion
    }
}