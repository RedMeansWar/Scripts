using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Client.Misc;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Misc.Object;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Misc.Vehicles;

namespace Red.SpikeStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Prop spikeProp1, spikeProp2, spikeProp3;
        #endregion

        #region Commands
        [Command("setspikes")]
        private void SetSpikesCommand(string[] args)
        {

        }

        [Command("clearspikes")]
        private void ClearSpikesCommand()
        {

        }
        #endregion

        #region Methods

        #endregion
    }
}