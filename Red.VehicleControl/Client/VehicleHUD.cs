using System;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.VehicleControl.Client
{
    internal class VehicleHUD : BaseScript
    {
        #region Variables
        protected static Ped PlayerPed = Game.PlayerPed;
        protected static Vehicle CurrentVehicle = PlayerPed.CurrentVehicle;
        protected bool usingMPH;
        #endregion

        #region Constructor
        public VehicleHUD() => ReadConfigFile();
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("HUD", "UsingMPH") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                usingMPH = loaded["HUD"]["UsingMPH"].BoolValue;
            }
            else
            {
                Error($"[VehicleControl] - Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task VehicleHudTick()
        {
            Vehicle vehicle = Game.PlayerPed.CurrentVehicle;

            if (vehicle is null || !HUDIsVisable)
            {
                await Delay(1000);
                return;
            }

            DrawRect(0.095f, 0.0475f, 0.046f, 0.03f, 0, 0, 0, 100);
            DrawText2d(0.87f, -0.125f, 0.6f, $"{Math.Ceiling(Game.PlayerPed.CurrentVehicle.Speed * 2.236936f)}", 255, 255, 255, 255, Alignment.Right);
            DrawText2d(0.875f, -0.135f, 0.4f, "mph", 255, 255, 255, 255);

            if (vehicle.Model.IsPlane || vehicle.Model.IsHelicopter)
            {
                DrawRect(0.095f, 0.17f, 0.046f, 0.03f, 0, 0, 0, 100);
                DrawText2d(0.87f, 0f, 0.6f, $"{Math.Ceiling(vehicle.HeightAboveGround * 3.2808f)}", 255, 255, 255, 255, Alignment.Right);
                DrawText2d(0.875f, -0.01f, 0.4f, "feet", 255, 255, 255, 255);
            }

            DrawText2d(0.5f, .045f, 0.55f, vehicle.Mods.LicensePlate, 255, 255, 255, 255, Alignment.Center);
            DrawText2d(1f, 0.065f, 0.45f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200, Alignment.Right);
            DrawText2d(0.15f, 0.04f, 0.45f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);

            float bodyHealth = vehicle.BodyHealth;
            float engineHealth = vehicle.EngineHealth;

            DrawText2d(1f, .04f, .45f, bodyHealth < 310 ? "~r~AC" : bodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200, Alignment.Right);
            DrawText2d(.75f, .04f, .45f, engineHealth < 110 ? "~r~Fluid" : engineHealth < 315 ? "~r~Fluid" : engineHealth < 900 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200);
            DrawText2d(.01f, .04f, .45f, engineHealth < 110 ? "~r~Oil" : engineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
        }
        #endregion
    }
}
