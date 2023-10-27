using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.VehicleControl.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool cruising, radarCruise, usingMPH;
        protected float targetSpeed = -1f;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<VehicleClass> ignoredVehicleClasses = new List<VehicleClass>
        {
            VehicleClass.Cycles, VehicleClass.Motorcycles, VehicleClass.Planes, VehicleClass.Helicopters, VehicleClass.Boats, VehicleClass.Trains
        };

        protected readonly IReadOnlyList<int> tireIndex = new List<int>
        {
            0, 1, 2, 3, 4, 5, 45, 47
        };
        #endregion

        #region Constructor
        public ClientMain() => RegisterKeyMapping("cruisecontrol", "Toggle cruise control", "keyboard", "f7");
        #endregion

        #region Commands
        [Command("engine")]
        private void EngineCommand() => ToggleEngine(PlayerPed.CurrentVehicle);

        [Command("eng")]
        private void EngCommand() => EngineCommand();

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
        private void OnHoodCommand()
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
        #endregion

        #region Methods
        private void ToggleEngine(Vehicle vehicle)
        {
            if (vehicle is null)
            {
                ErrorNotification("You must be in a vehicle");
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
            if (!Screen.Hud.IsVisible || Game.PlayerPed.CurrentVehicle is null)
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
                    cruise += $"~s~: {Math.Ceiling(targetSpeed * 3.6f)}mph";
                }
            }

            DrawText2d(0.0299f, 3.95f, 0.47f, cruise, 255, 255, 255, 210);
        }
        #endregion
    }
}