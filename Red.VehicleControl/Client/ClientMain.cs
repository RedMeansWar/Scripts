using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;
using static Red.Common.Client.Hud.HUD;

namespace Red.VehicleControl.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<VehicleClass> ignoredVehicleClasses = new List<VehicleClass>
        {
            VehicleClass.Cycles, VehicleClass.Motorcycles, VehicleClass.Planes, VehicleClass.Helicopters, VehicleClass.Boats, VehicleClass.Trains
        };

        protected readonly IReadOnlyList<int> tireIndex = new List<int>
        {
            0, 1, 2, 3, 4, 5, 45, 47
        };

        protected bool cruising, radarCruise;
        protected float targetSpeed = -1f;
        #endregion

        #region Constructor
        public ClientMain() => RegisterKeyMapping("+cruisecontrol", "Toggle cruise control", "keyboard", "f7");
        #endregion

        #region Commands
        [Command("+cruisecontrol")]
        private void OnCruiseControlCommand()
        {
            cruising = !cruising;

            if (!cruising && Game.PlayerPed.CurrentVehicle is not null)
            {
                CancelCruise();
            }
        }
        #endregion

        #region Methods
        private void CancelCruise()
        {
            SetVehicleMaxSpeed(PlayerPed.CurrentVehicle.Handle, 500f);
            targetSpeed = -1;
            cruising = false;
        }

        private bool HaveAnyTiresBurst() => tireIndex.Any(t => IsVehicleTyreBurst(PlayerPed.CurrentVehicle.Handle, t, false));
        #endregion

        #region Ticks
        [Tick]
        private async Task ReticleTick()
        {
            Weapon weapon = Game.PlayerPed.Weapons.Current;

            if (weapon is null)
            {
                return;
            }

            if (weapon.Hash == WeaponHash.Musket || !((weapon.Group == WeaponGroup.Sniper && IsFirstPersonAimCamActive()) || weapon.Group == WeaponGroup.Unarmed || weapon.Group == 0))
            {
                HideHudComponentThisFrame((int)HudComponent.Reticle);
            }

            Hud.HideComponentThisFrame(HudComponent.Cash | HudComponent.CashChange | HudComponent.MpCash | HudComponent.MpTagCashFromBank | HudComponent.VehicleName);
            HideHudComponentThisFrame(7);
            HideHudComponentThisFrame(9);
        }

        [Tick]
        private async Task HudTick()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle is null || !Hud.IsVisible)
            {
                await Delay(1000);
                return;
            }

            DrawRectangle(0.095f, 0.06f, 0.046f, 0.03f, 0, 0, 0, 100);
            DrawText2d(0.847f, -0.111f, 0.6f, $"{Math.Ceiling(vehicle.Speed * 2.236936f)}", 255, 255, 255, 255, Alignment.Right);
            DrawText2d(0.875f, -0.118f, 0.4f, "mph", 255, 255, 255, 255);

            if (vehicle.Model.IsPlane || vehicle.Model.IsHelicopter)
            {
                DrawRectangle(0.095f, 0.17f, 0.046f, 0.03f, 0, 0, 0, 100);
                DrawText2d(0.87f, 0f, 0.6f, $"{Math.Ceiling(vehicle.HeightAboveGround * 3.2808f)}", 255, 255, 255, 255, Alignment.Right);
                DrawText2d(0.875f, -0.01f, 0.4f, "feet", 255, 255, 255, 255);
            }

            DrawText2d(0.5f, 0.049f, 0.55f, vehicle.Mods.LicensePlate, 255, 255, 255, 255, Alignment.Center);
            DrawText2d(1f, 0.07f, 0.45f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200, Alignment.Right);
            DrawText2d(0.15f, 0.044f, 0.45f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);

            DrawText2d(1f, 0.044f, 0.45f, vehicle.BodyHealth < 310 ? "~r~AC" : vehicle.BodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200, Alignment.Right);
            DrawText2d(0.75f, 0.044f, 0.45f, vehicle.EngineHealth < 110 ? "~r~Fluid" : vehicle.EngineHealth < 315 ? "~r~Fluid" : vehicle.EngineHealth < 900 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200);
            DrawText2d(0.01f, 0.044f, 0.45f, vehicle.EngineHealth < 110 ? "~r~Oil" : vehicle.EngineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
        }

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
            if (!HUDIsVisible || PlayerPed.CurrentVehicle is null)
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


            DrawText2d(0.01f, 0.07f, 0.45f, cruise, 255, 255, 255, 200);
        }
        #endregion
    }
}