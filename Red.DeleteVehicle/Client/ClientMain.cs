using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.DeleteVehicle.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected int tempTimer = Game.GameTime;
        protected static Ped PlayerPed;
        #endregion

        #region Commands
        [Command("dv")]
        private void DvCommand() => DeleteVehicleHandler();

        [Command("delveh")]
        private void DelVehCommand() => DeleteVehicleHandler();

        [Command("deletevehicle")]
        private void DeleteVehicleCommand() => DeleteVehicleHandler();

        [Command("deleteveh")]
        private void DeleteVehCommand() => DeleteVehicleHandler();
        #endregion

        #region Handlers
        private async void DeleteVehicleHandler()
        {
            TriggerEvent("chat:addSuggestion", "/dv", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/deleteveh", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/deletevehicle", "Delete the closest vehicle to the player.", "");
            TriggerEvent("chat:addSuggestion", "/delveh", "Delete the closest vehicle to the player.", "");

            if (PlayerPed.CurrentVehicle is null)
            {
                Vehicle closestVehicle = GetClosestVehicle(3f);

                if (closestVehicle is null)
                {
                    Screen.ShowNotification("~r~~h~Error~h~~s~: You must be in or near a vehicle.", true);
                    return;
                }

                if (closestVehicle.Driver.Exists() && closestVehicle.Driver.IsPlayer)
                {
                    Screen.ShowNotification("~r~~h~Error~h~~s~: That vehicle still has a driver in it.", true);
                    return;
                }

                if (NetworkGetEntityOwner(closestVehicle.Handle) == Game.Player.Handle)
                {
                    bool deleted = await DeleteVehicle(closestVehicle);
                    Screen.ShowNotification(deleted ? "~g~~h~Success~h~~s~: Vehicle deleted." : "~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
                }
                else
                {
                    TriggerServerEvent("DeleteVehicle:Server:deleteVehicle", closestVehicle.NetworkId);

                    int tempTimer = Game.GameTime;

                    while (closestVehicle.Exists())
                    {
                        if (Game.GameTime - tempTimer > 5000)
                        {
                            Screen.ShowNotification("~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
                            return;
                        }

                        await Delay(0);
                    }

                    Screen.ShowNotification("~g~~h~Success~h~~s~: Vehicle deleted.", true);
                }
            }
            else if (PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You must be the driver.", true);
            }
            else
            {
                bool deleted = await DeleteVehicle(PlayerPed.CurrentVehicle);

                Screen.ShowNotification(deleted ? "~g~~h~Success~h~~s~: Deleted vehicle." : "~r~~h~Error~h~~s~: Failed to delete vehicle, try again.", true);
            }
        }
        #endregion

        #region Methods
        private static Vehicle GetClosestVehicle(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(PlayerPed.Position, PlayerPed.Position, radius, (IntersectOptions)10, PlayerPed);
            return raycast.HitEntity as Vehicle;
        }
        #endregion

        #region Ticks
        private async Task<bool> DeleteVehicle(Vehicle vehicle)
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

        [Tick]
        private async Task CommandsTick()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            if (vehicle is null)
            {
                await Delay(500);
                return;
            }
        }
        #endregion
    }
}