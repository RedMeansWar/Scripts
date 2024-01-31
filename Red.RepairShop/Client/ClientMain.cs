using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Red.Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Client;

namespace Red.RepairShop.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Blip repairBlip;
        protected Random random = new();

        protected readonly List<Vector3> repairShopsPosition = new()
        {
            new(535.32f, -180.27f, 54.34f),
            new(1998.66f, 3797f, 32.18f),
            new(110.43f, 6627.4f, 31.79f),
            new(723.8f, -1088.89f, 22.17f),
            new(-340.32f, -137.62f, 39.01f),
            new(-1155.15f, -2003.02f, 13.18f),
            new(1174.87f, 2640.67f, 37.75f),
            new(-209.77f, -1324.05f, 30.89f),
            new(1773.75f, 3333.8f, 41.35f)
        };
        #endregion

        #region Commands
        [Command("repair")]
        private async void RepairCommand()
        {
            Vehicle currentVehicle = PlayerPed.CurrentVehicle;

            if (currentVehicle is null)
            {
                ErrorNotification("You need to be in a vehicle to do this.");
                return;
            }

            if (currentVehicle.Driver != PlayerPed)
            {
                ErrorNotification("You need to be the driver to do this.");
                return;
            }

            foreach (Vector3 location in repairShopsPosition)
            {
                if (PlayerPed.CalculateDistanceTo(location) < 15f)
                {
                    ShowSubtitle("The mechanic is looking at your vehicle...", 5000);

                    await Delay(5500);
                    ShowSubtitle("The mechanic ~g~fixed~w~ your vehicle!", 4000);

                    currentVehicle.Repair();
                }
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task RepairBlipsTick()
        {
            foreach (Vector3 location in repairShopsPosition)
            {
                repairBlip = World.CreateBlip(location);
                repairBlip.Sprite = (BlipSprite)446;
                repairBlip.Scale = 1f;
            }
        }
        #endregion
    }
}