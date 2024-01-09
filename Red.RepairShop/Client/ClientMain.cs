using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.ClientExtensions;
using static Red.Common.Client.Client;
using Red.Common.Client.Misc;

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
            new(-209.77f, -1324.05f, 30.89f) // bennys
        };
        #endregion

        #region Commands
        [Command("repair")]
        private async void RepairCommand()
        {
            Vehicle closestVehicle = GetClosestVehicle(1.5f);
            Player currentPlayer = Game.Player;
            Entity entity = currentPlayer.GetTargetedEntity();

            if (repairBlip.CalculateDistanceTo(PlayerPed.Position) < 10f)
            {
                 
            }

            if (closestVehicle.CalculateDifferenceBetweenHeadings(entity) < 2f && PlayerCurrentVehicle.Doors[VehicleDoorIndex.Hood].IsOpen)
            {
                int chance = random.Next(0, 101);

                PlayerPed.Task.PlayAnimation("amb@prop_human_bum_bin@enter", "enter", 8.0f, 1500, AnimationFlags.UpperBodyOnly);
                await Delay(1500);
                PlayerPed.Task.ClearAnimation("amb@prop_human_bum_bin@enter", "enter");

                if (chance <= 30)
                {
                    DisplayNotification("~r~You managed to slightly break your vehicle's engine, you should get to a shop!");
                    SetVehicleEngineHealth(PlayerCurrentVehicle.Handle, 550f);
                }
                else
                {
                    DisplayNotification("~g~You managed to slightly repair your vehicle, you should get to a shop!");
                    SetVehicleEngineHealth(PlayerCurrentVehicle.Handle, 800f);
                }
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task LoadRepairBlips()
        {
            foreach (Vector3 location in repairShopsPosition)
            {
                if (repairBlip is null)
                {
                    repairBlip = World.CreateBlip(location);

                }
            }
        }
        #endregion
    }
}