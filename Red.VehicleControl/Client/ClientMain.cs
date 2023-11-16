using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Misc.Vehicles;

namespace Red.VehicleControl.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool cruising, radarCruise, usingMPH;
        protected float targetSpeed = -1f;
        protected int repairShop, lsCustom, benny;
        protected Ped PlayerPed = Game.PlayerPed;
        protected Random random = new();

        protected readonly IReadOnlyList<VehicleClass> ignoredVehicleClasses = new List<VehicleClass>
        {
            VehicleClass.Cycles, VehicleClass.Motorcycles, VehicleClass.Planes, VehicleClass.Helicopters, VehicleClass.Boats, VehicleClass.Trains
        };

        protected readonly IReadOnlyList<int> tireIndex = new List<int>
        {
            0, 1, 2, 3, 4, 5, 45, 47
        };

        protected static readonly IReadOnlyList<Vector3> repairLocations = new List<Vector3>()
        {
            new(2006.27f, 3797.94f, 32.18f),
            new(535.96f, -178.87f, 54.4f)
        };

        protected static readonly IReadOnlyList<Vector3> repairLoc = new List<Vector3>()
        {
            new(1175.89f, 2640.16f, 37.75f),
            new(-336.81f, -135.94f, 39),
            new(731.23f, -1088.13f, 22.17f),
            new(-1153.8f, -2004.81f, 13.18f),
            new(110.62f, 6625.71f, 31.79f),
            new(-211.63f, 1323.38f, 30.89f)
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            CreateBlips();
            RegisterKeyMapping("cruisecontrol", "Toggle cruise control", "keyboard", "f7");

            AddCommandSuggestion("/engine", "Turns on or off a vehicle's engine.");
            AddCommandSuggestion("/eng", "Turns on or off a vehicle's engine.");
            AddCommandSuggestion("/cruisecontrol", "Sets the vehicle speed to a static speed. (also can be activated using F7)");
            AddCommandSuggestion("/flip", "Flips an upside down vehicle to be upright.");
            AddCommandSuggestion("/trunk", "Opens and closes a vehicle's trunk.");
            AddCommandSuggestion("/hood", "Opens and closes s vehicle's hood.");
            AddCommandSuggestion("/door", "Open a vehicle's door. (Ex: /door 1)");
            AddCommandSuggestion("/fix", "Fixes the vehicle you are in. (must be near a vehicle shop to use this)");
        }
        #endregion

        #region Commands
        [Command("engine")]
        private void EngineCommand() => ToggleEngine(PlayerPed.CurrentVehicle);

        [Command("eng")]
        private void EngCommand() => ToggleEngine(PlayerPed.CurrentVehicle);

        [Command("cruisecontrol")]
        private void CruiseControlCommand()
        {
            cruising = !cruising;

            if (!cruising && PlayerPed.CurrentVehicle is not null)
            {
                CancelCruise();
            }
        }

        [Command("flip")]
        private void FlipCommand()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle ?? GetClosestVehicle();

            if (vehicle is not null)
            {
                if (PlayerPed.SeatIndex != VehicleSeat.Driver)
                {
                    ErrorNotification("You must be the driver.");
                    return;
                }

                if (SetVehicleOnGroundProperly(vehicle.Handle))
                {
                    SuccessNotification("Flipped vehicle.");
                    return;
                }

                ErrorNotification("Failed to flip vehicle, try again.");
            }
            else
            {
                ErrorNotification("You must be in a vehicle.");
            }
        }

        [Command("trunk")]
        private void TrunkCommand()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle ?? GetClosestVehicle(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle is not null && Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                ErrorNotification("The trunk isn't intact.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Trunk].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, false);
                }

                SuccessNotification("Trunk closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Trunk].Open();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Trunk, true);
                }
                SuccessNotification("Trunk opened.");
            }
        }

        [Command("hood")]
        private void HoodCommand()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle ?? GetClosestVehicle(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle is not null && Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsBroken)
            {
                ErrorNotification("The hood isn't intact.");
                return;
            }

            if (vehicle.Doors[VehicleDoorIndex.Hood].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, false);
                }

                SuccessNotification("Hood closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[VehicleDoorIndex.Hood].Open();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, (int)VehicleDoorIndex.Hood, true);
                }

                SuccessNotification("Hood opened.");
            }
        }

        [Command("door")]
        private void OnDoorCommand(string[] args)
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle ?? GetClosestVehicle(1f);

            if (vehicle is null)
            {
                ErrorNotification("You must be in or near a vehicle.");
                return;
            }

            if (Game.PlayerPed.CurrentVehicle is not null && Game.PlayerPed.SeatIndex != VehicleSeat.Driver)
            {
                ErrorNotification("You must be the driver.");
                return;
            }

            if (vehicle.LockStatus != VehicleLockStatus.Unlocked)
            {
                ErrorNotification("You must unlock the car.");
                return;
            }

            if (!int.TryParse(args[0], out int doorIndex) || doorIndex < 0 || doorIndex > 4)
            {
                ErrorNotification("Invalid door.");
                return;
            }

            if (doorIndex > 0)
            {
                doorIndex--;
            }

            if (vehicle.Doors[(VehicleDoorIndex)doorIndex].IsBroken)
            {
                ErrorNotification("That door isn't intact.");
                return;
            }

            if (vehicle.Doors[(VehicleDoorIndex)doorIndex].IsOpen)
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, doorIndex, false);
                }

                SuccessNotification("Door closed.");
            }
            else
            {
                if (NetworkGetEntityOwner(vehicle.Handle) == Game.Player.Handle)
                {
                    vehicle.Doors[(VehicleDoorIndex)doorIndex].Open();
                }
                else
                {
                    TriggerServerEvent("VehicleControl:Server:doorAction", vehicle.NetworkId, doorIndex, true);
                }

                SuccessNotification("Door opened.");
            }
        }

        [Command("fix")]
        private async void FixCommand()
        {
            Vector3 playerPos = PlayerPed.Position;

            if (!PlayerPed.IsInVehicle() && PlayerPed.CurrentVehicle is null)
            {
                DisplayNotification("~r~You must be in a vehicle to repair it at a shop.");
                return;
            }

            if (DistanceFromBlip(repairShop, playerPos.X, playerPos.Y, playerPos.Z) <= 10f)
            {
                DisplayNotification("~g~The mechanic is working on your vehicle.");
                await Delay(10000);

                DisplayNotification("~g~The vehicle has been repaired.");
                await Delay(500);
                PlayerPed.CurrentVehicle.Repair();
            }
        }
        #endregion

        #region Methods
        private void ToggleEngine(Vehicle vehicle)
        {
            if (vehicle is null)
            {
                ErrorNotification("You must be in a vehicle.");
                return;
            }

            if (vehicle.EngineHealth < 110)
            {
                ErrorNotification("You can't turn off an engine that is broken.");
                return;
            }

            PlayerPed.SetConfigFlag(429, true);
            vehicle.IsEngineRunning = !vehicle.IsEngineRunning;
        }

        private void CancelCruise()
        {
            SetVehicleMaxSpeed(PlayerPed.CurrentVehicle.Handle, 500f);
            targetSpeed = -1f;
            cruising = false;
        }

        private bool HaveAnyTiresBurst() => tireIndex.Any(t => IsVehicleTyreBurst(Game.PlayerPed.CurrentVehicle.Handle, t, false));

        private void CreateBlips()
        {
            foreach (var location in repairLocations)
            {
                repairShop = AddBlipForCoord(location.X, location.Y, location.Z);

                SetBlipSprite(repairShop, 446);
                SetBlipScale(repairShop, 1f); 
                BeginTextCommandSetBlipName("STRING");
                AddTextComponentSubstringPlayerName("Repair Shop");
                EndTextCommandSetBlipName(repairShop);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("VehicleControl:Client:doorAction")]
        private void OnDoorAction(int netId, int doorIndex, bool open)
        {
            Vehicle vehicle = (Vehicle)Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                Info($"Got Network ID '{netId}' from doorAction event and wasn't able to convert to vehicle, bailing.");
                return;
            }

            if (open)
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Open();
            }
            else
            {
                vehicle.Doors[(VehicleDoorIndex)doorIndex].Close();
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task CruiseControlTick()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle is null && cruising)
            {
                cruising = false;
                targetSpeed = -1f;

                await Delay(1000);
                return;
            }

            while (cruising)
            {
                if (ignoredVehicleClasses.Contains(vehicle.ClassType))
                {
                    CancelCruise();
                    return;
                }

                if (targetSpeed == -1)
                {
                    targetSpeed = GetEntitySpeed(vehicle.Handle);
                    SetVehicleMaxSpeed(vehicle.Handle, targetSpeed);
                }

                if (vehicle.Speed > 1f || !radarCruise)
                {
                    Game.SetControlNormal(0, Control.VehicleAccelerate, 0.9f);
                }

                if (vehicle.Driver is null || vehicle.Driver != Game.PlayerPed || vehicle.IsInWater || vehicle.IsInBurnout || !vehicle.IsEngineRunning || vehicle.IsInAir || vehicle.HasCollided || targetSpeed * 2.236936f < 25f || targetSpeed * 2.236936f > 100f || HaveAnyTiresBurst() || Game.IsControlJustPressed(0, Control.VehicleHandbrake))
                {
                    CancelCruise();
                    return;
                }

                if (Game.GetControlValue(0, Control.VehicleAccelerate) > 250f)
                {
                    float current = targetSpeed * 2.236936f;
                    float newSpeed = (float)Math.Ceiling(++current);
                    targetSpeed = newSpeed * 2.236936f;
                    SetVehicleMaxSpeed(vehicle.Handle, targetSpeed);
                    await Delay(200);
                }

                if (Game.IsControlPressed(0, Control.VehicleBrake))
                {
                    int delay = 3;

                    while (Game.IsControlPressed(0, Control.VehicleBrake))
                    {
                        delay--;

                        if (delay == 0)
                        {
                            CancelCruise();
                            return;
                        }

                        await Delay(100);
                    }

                    float current = targetSpeed * 2.236936f;
                    float newSpeed = (float)Math.Ceiling(--current);
                    targetSpeed = newSpeed * 3.6f;
                    SetVehicleMaxSpeed(vehicle.Handle, targetSpeed);
                }

                RaycastResult rr = World.RaycastCapsule(vehicle.Position, vehicle.GetOffsetPosition(new(0f, 15f, 0f)), 2f, IntersectOptions.Everything, vehicle);

                if (rr.DitHitEntity)
                {
                    Vehicle forwardVehicle = rr.HitEntity as Vehicle;

                    if (forwardVehicle.Driver.Handle == 0)
                    {
                        radarCruise = false;

                        if (vehicle.Speed < targetSpeed)
                        {
                            SetVehicleMaxSpeed(vehicle.Handle, targetSpeed);
                        }

                        return;
                    }

                    radarCruise = true;

                    if (vehicle.Speed > GetEntitySpeed(forwardVehicle.Handle))
                    {
                        float current = vehicle.Speed;
                        float newSpeed = (float)Math.Ceiling(current - 3);
                        SetVehicleMaxSpeed(vehicle.Handle, newSpeed * 3.6f);
                        vehicle.AreBrakeLightsOn = true;
                        await Delay(50);
                    }

                    if (vehicle.Speed < targetSpeed && GetEntitySpeed(forwardVehicle.Handle) > vehicle.Speed)
                    {
                        float current = vehicle.Speed;
                        float newSpeed = (float)Math.Ceiling(++current);
                        SetVehicleMaxSpeed(vehicle.Handle, newSpeed * 3.6f);
                    }
                }
                else
                {
                    radarCruise = false;

                    if (vehicle.Speed < targetSpeed)
                    {
                        SetVehicleMaxSpeed(vehicle.Handle, targetSpeed);
                    }
                }

                await Delay(0);
            }
            await Delay(100);
        }

        [Tick]
        private async Task CruiseControlUiTick()
        {
            if (!HUDIsVisable || Game.PlayerPed.CurrentVehicle is null)
            {
                await Delay(1000);
                return;
            }

            string cruise = "~r~Cruise";

            if (cruising)
            {
                cruise = $"~g~{(radarCruise ? "DRCC" : "Cruise")}";

                if (targetSpeed - Game.PlayerPed.CurrentVehicle.Speed > 1f)
                {
                    cruise += $"~s~: {Math.Ceiling(Game.PlayerPed.CurrentVehicle.Speed * 2.236936f)}mph";
                }
            }

            DrawText2d(0.01f, 0.065f, 0.45f, cruise, 255, 255, 255, 200);
        }
        #endregion
    }
}