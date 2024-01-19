using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Client;

namespace Red.SpikeStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected string modelName = "p_ld_stinger_s";
        protected Prop spikeProp;

        protected readonly List<int> tireIndex = new()
        {
            0,
            1,
            2,
            4,
            5,
            45,
            46
        };
        #endregion

        #region Commands
        [Command("setspikes")]
        private void SetSpikesCommand(string[] args)
        {
            if (args.Length != 0 || !int.TryParse(args[0], out int spikeDeployAmount) || spikeDeployAmount < 2 || spikeDeployAmount > 4)
            {
                ChatMessage("[Spike Strips]", "Invalid spikestrip amount! Usage: /setspikes <spike amouunt>", 255, 0, 0);
                return;
            }

            if (PlayerPed.CannotDoAction() || !PlayerPed.IsOnFoot)
            {
                ChatMessage("[Spike Strips]", "You cannot do this right now!", 255, 0, 0);
                return;
            }
        }
        #endregion
    }
}