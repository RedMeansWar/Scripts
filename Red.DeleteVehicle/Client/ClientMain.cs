using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.DeleteVehicle.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected int tempTimer = Game.GameTime;
        #endregion

        #region Commands
        [Command("dv")]
        private void DvCommand() => CommandHandler();

        [Command("deleteveh")]
        private void DeleteVehCommand() => CommandHandler();

        [Command("deletevehicle")]
        private void DeleteVehicleCommand() => CommandHandler();

        [Command("delveh")]
        private void DelVehCommand() => CommandHandler();
        #endregion

        #region Command Handlers
        private async void CommandHandler()
        {
            #region Chat Suggestions
            TriggerEvent("chat:addSuggestion", "/dv", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/deleteveh", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/deletevehicle", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/delveh", "Delete the closest vehicle to the player.", "");
            #endregion

            Vehicle closestVehicle = GetClosestVehicleToPlayer(3f);

            if (Game.PlayerPed.CurrentVehicle is null)
            {
                if (closestVehicle is null)
                {
                    ErrorNotification("You must be in or near a vehicle.");
                    return;
                }

                if (closestVehicle.Driver.Exists() && closestVehicle.Driver.IsPlayer)
                {
                    ErrorNotification("That vehicle still has a driver.");
                    return;
                }

                if (closestVehicle.IsUpsideDown || Game.PlayerPed.CurrentVehicle.IsUpsideDown)
                {
                    bool deleted = await DvVehicle(closestVehicle);

                    ShowNotification(deleted ? "~g~~h~Success~h~~s~: Vehicle deleted." : "~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
                }

                if (NetworkGetEntityOwner(closestVehicle.Handle) == Game.Player.Handle)
                {
                    bool deleted = await DvVehicle(closestVehicle);

                    ShowNotification(deleted ? "~g~~h~Success~h~~s~: Vehicle deleted." : "~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
                }
                else
                {
                    TriggerServerEvent("DeleteVehicle:Server:deleteVehicle", closestVehicle.NetworkId);

                    int tempTimer = Game.GameTime;

                    while (closestVehicle.Exists())
                    {
                        if (Game.GameTime - tempTimer > 5000)
                        {
                            ErrorNotification("Failed to delete vehicle, try again.");
                            return;
                        }

                        await Delay(0);
                    }
                }
            }
            else if (Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
            }
            else
            {
                bool deleted = await DvVehicle(Game.PlayerPed.CurrentVehicle);

                ShowNotification(deleted ? "~g~~h~Success~h~~s~: Vehicle deleted." : "~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
            }
        }

        private async Task<bool> DvVehicle(Vehicle vehicle)
        {
            vehicle.IsPersistent = true;
            vehicle.Delete();

            int tempTimer = Game.GameTime;

            while (vehicle.Exists())
            {
                if (Game.GameTime - tempTimer > 5000)
                {
                    return false;
                }

                await Delay(0);
            }

            return true;
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task CommandsTick()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle is null)
            {
                await Delay(500);
                return;
            }
        }
        #endregion
    }
}