using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

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
            Vehicle boat = PlayerPed.CurrentVehicle;

            if (!PlayerPed.IsInBoat || PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                // do nothing
            }

            if (PlayerPed.IsInBoat)
            {
                Tick += AnchorBoatTick;
            }

            if (boat.IsEngineRunning)
            {
                // do nothing
            }
        }
        #endregion

        #region Commands
        [Command("anchor")]
        private void AnchorCommand()
        {
            Vehicle boat = PlayerPed.CurrentVehicle;

            if (!PlayerPed.IsInBoat || PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You need to be conning a boat.", true);
                return;
            }

            if (boat.Speed >= 5f)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You're going too fast to anchor the boat.", true);
                return;
            }

            if (!boat.IsEngineOnFire)
            {
                if (IsBoatAnchoredAndFrozen(boat.Handle))
                {
                    SetBoatAnchor(boat.Handle, false);
                    Screen.ShowNotification("~g~~h~Success~h~~s~: Un-anchored boat.", true);
                }
                else
                {
                    SetBoatFrozenWhenAnchored(boat.Handle, true);
                    SetBoatAnchor(boat.Handle, true);
                    Screen.ShowNotification("~g~~h~Success~h~~s~: Anchored Boat.", true);
                }
            }
            else
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must have the engine off to anchor the boat.", true);
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
                else
                {
                    if (boat.IsEngineRunning)
                    {
                        Screen.ShowNotification("~r~~h~Error~h~~s~: You have to have the engine off.");
                        return;
                    }
                    else
                    {
                        if (IsBoatAnchoredAndFrozen(boat.Handle))
                        {
                            if (IsControlJustPressed(0, (int)Control.Context))
                            {
                                SetBoatAnchor(boat.Handle, false);
                                Screen.ShowNotification("~g~~h~Success~h~~s~: Un-anchored boat.");
                            }

                            Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to un-anchor the boat.");
                        }
                        else
                        {
                            if (IsControlJustPressed(0, (int)Control.Context))
                            {
                                SetBoatFrozenWhenAnchored(boat.Handle, true);
                                SetBoatAnchor(boat.Handle, true);
                                Screen.ShowNotification("~g~~h~Success~h~~s~: Anchored Boat");
                            }

                            Screen.DisplayHelpTextThisFrame("Press ~INPUT_CONTEXT~ to anchor the boat.");
                        }
                    }
                }
            }
        }
        #endregion
    }
}