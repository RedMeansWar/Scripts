using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.VendingMachines.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Dictionary<int, int> propModels = new()
        {
            { GetHashKey("prop_vend_soda_01"), GetHashKey("prop_ecola_can") },
            { GetHashKey("prop_vend_soda_02"), GetHashKey("prop_ld_can_01b") }
        };

        /*
        protected string buyAnimDict = "MINI@SPRUNK";
        protected string buyAnimDict = "PLYR_BUY_DRINK_PT1";
        */
        #endregion

        #region Methods
        private async Task Dispense(Entity vendingMachine)
        {
            Prop closestProp = PlayerPed.GetClosestPropToClient(1.5f);

            if (closestProp is null)
            {
                return;
            }

            bool owner = NetworkGetEntityOwner(vendingMachine.Handle) == Game.Player.Handle;

            if (!owner)
            {
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task FindVendingMachine()
        {
            if (PlayerPed.CannotDoAction())
            {
                await Delay(2500);
                return;
            }

            Prop prop = World.GetAllProps().Where(p => propModels.ContainsKey(p.Model)).OrderBy(p => PlayerPed.CalculateDistanceTo(p.Position)).FirstOrDefault();

            if (prop is null)
            {
                await Delay(4000);
                return;
            }

            float distance = PlayerPed.CalculateDistanceTo(prop.Position);

            if (distance > 5.0f)
            {
                await Delay(1500);
                return;
            }

            if (!IsPauseMenuActive() && distance < 1.5f)
            {
                DisplayHelpText("Press ~INPUT_CONTEXT~ to buy a soda for $1");

                if (Game.IsControlPressed(0, Control.Context))
                {
                    await Dispense(prop);
                }
            }
        }
        #endregion
    }
}