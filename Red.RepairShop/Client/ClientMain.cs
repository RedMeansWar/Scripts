using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.ClientExtensions;
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
            new(-209.77f, -1324.05f, 30.89f)
        };
        #endregion

        #region Commands
        [Command("repair")]
        private async void RepairCommand()
        {
            int chance = random.Next(0, 101);

            if (PlayerCurrentVehicle != null && PlayerCurrentVehicle.EngineHealth == 300f)
            {
                Vector3 distance = PlayerCurrentVehicle.Position - PlayerPed.Position;
                distance.Normalize();

                float dotVector = Vector3.Dot(distance, PlayerCurrentVehicle.ForwardVector);

                if (dotVector > 0.78f)
                {

                    await Delay(2500);
                    DisplayNotification("Attempting to repair your vehicle.");

                    if (chance <= 30)
                    {
                        DisplayNotification("~r~You managed to slightly break your vehicle's engine, you should get to a shop!");
                        SetVehicleEngineHealth(PlayerCurrentVehicle.Handle, 410f);
                    }
                    else
                    {
                        DisplayNotification("~g~You managed to slightly repair your vehicle, you should get to a shop!");
                        SetVehicleEngineHealth(PlayerCurrentVehicle.Handle, 550f);
                    }
                }
            }

            if (repairBlip.CalculateDistanceTo(PlayerPed.Position) > 5f)
            {
                DisplayNotification("~g~The mechanic is looking at your vehicle...");
                await Delay(3000);

                Screen.ShowSubtitle("~g~Your vehicle has been repaired");
                PlayerCurrentVehicle.Repair();
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task RepairBlipsTick()
        {
            float zoomLevel = GetFollowPedCamZoomLevel();

            foreach (Vector3 location in repairShopsPosition)
            {
                repairBlip = World.CreateBlip(location);
                repairBlip.Sprite = BlipSprite.Repair;
                repairBlip.Scale = 0.7f + (zoomLevel * 0.01f);
            }
        }
        #endregion
    }
}