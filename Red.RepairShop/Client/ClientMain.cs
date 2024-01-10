using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Red.Common.Client;
using CitizenFX.Core;
using CitizenFX.Core.UI;
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
            new(-209.77f, -1324.05f, 30.89f)
        };
        #endregion

        #region Commands
        [Command("repair")]
        private async void RepairCommand()
        {
            Vehicle currentVehicle = PlayerPed.CurrentVehicle;

            if (currentVehicle is null)
            {
                return;
            }

            if (repairBlip.CalculateDistanceTo(PlayerPed.Position) < 15f)
            {
                DisplayNotification("The mechanic is looking at your vehicle...");
                await Delay(3000);

                currentVehicle.Repair();

                Screen.ShowSubtitle("The mechanic has ~g~repaired~ ~w~your vehicle!");
            }

            if (PlayerPed.IsOnFoot && currentVehicle.BodyHealth < 310f || PlayerPed.IsInVehicle() && currentVehicle.BodyHealth < 310f)
            {
                DisplayNotification("~g~Attempting to fix...");

                if (random.Next(0, 101) < 30)
                {
                    await Delay(3000);
                    ErrorNotification("You managed to somehow damage your vehicle even more, get it to a mechanic!", false);

                    currentVehicle.EngineHealth = 250f;
                }
                else
                {
                    await Delay(3000);
                    SuccessNotification("You managed to somehow fix your a vehicle a little, get it to a mechanic!");
                    currentVehicle.EngineHealth = 320f;
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