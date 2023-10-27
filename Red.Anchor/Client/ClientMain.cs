using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;

namespace Red.Anchor.Client
{
    public class ClientMain : BaseScript
    {
        protected Ped PlayerPed = Game.PlayerPed;

        [Command("anchor")]
        private void AnchorCommand()
        {
            if (!Game.PlayerPed.IsInBoat)
            {
                ErrorNotification("You must be conning a boat.");
                return;
            }

            Vehicle boat = Game.PlayerPed.CurrentVehicle;

            if (boat.Speed >= 5f)
            {
                ErrorNotification("You're going to fast to anchor the boat.");
                return;
            }

            if (IsBoatAnchoredAndFrozen(boat.Handle))
            {
                SetBoatAnchor(boat.Handle, false);
                SuccessNotification("Un-anchored boat.");
            }
            else
            {
                SetBoatFrozenWhenAnchored(boat.Handle, true);
                SetBoatAnchor(boat.Handle, true);
                SuccessNotification("Anchored boat.");
            }
        }
    }
}