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
        #region Variables
        protected Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Constructor
        public ClientMain()
        {
            TriggerEvent("chat:addSuggestion", "/anchor", "Stops a boat from moving.", "");
        }
        #endregion

        #region Command
        [Command("anchor")]
        private void AnchorCommand()
        {
            if (!Game.PlayerPed.IsInBoat)
            {
                ErrorNotification("You must be conning a boat.");
                return;
            }

            Vehicle boat = Game.PlayerPed.CurrentVehicle;

            if (boat.Speed >= 2f)
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
        #endregion

        #region Ticks
        [Tick]
        private async Task AnchorBoatTick()
        {
            Vehicle boat = PlayerPed.CurrentVehicle;

            if (!PlayerPed.IsInBoat || PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                // do nothing
            }
            else
            {
                if (boat.Speed >= 5f)
                {
                    ClearAllHelpMessages();

                }
                else if (IsBoatAnchoredAndFrozen(boat.Handle))
                {
                    if (IsControlJustPressed(0, (int)Control.Context))
                    {
                        SetBoatAnchor(boat.Handle, false);
                        SuccessNotification("Un-anchored boat.");
                    }

                    Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to un-anchor the boat.");
                }
                else
                {
                    if (IsControlJustPressed(0, (int)Control.Context))
                    {
                        SetBoatFrozenWhenAnchored(boat.Handle, true);
                        SetBoatAnchor(boat.Handle, true);
                        SuccessNotification("Anchored Boat");
                    }

                    Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to anchor the boat.");
                }
            }
        }
        #endregion
    }
}