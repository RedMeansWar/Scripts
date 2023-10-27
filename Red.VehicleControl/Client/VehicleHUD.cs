using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
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

            DrawRectangle(0.138f, 0.9475f, 0.046f, 0.03f, 0, 0, 0, 100);

            if (vehicle.Model.IsCar || vehicle.Model.IsVehicle)
            {
                DrawText2d(0.0510f, 3.5f, 0.47f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);
                DrawText2d(0.0757f, 3.65f, 0.60f, vehicle.Mods.LicensePlate, 255, 255, 255, 200);
                DrawText2d(0.1560f, 3.95f, 0.47f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200);

                DrawText2d(0.1610f, 3.5f, 0.47f, vehicle.BodyHealth < 310 ? "~g~AC" : vehicle.BodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200);
                DrawText2d(0.1325f, 3.5f, 0.47f, vehicle.EngineHealth < 110 ? "~r~Fluid" : vehicle.EngineHealth < 315 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200);
                DrawText2d(0.0299f, 3.5f, 0.47f, vehicle.EngineHealth < 110 ? "~r~Oil" : vehicle.EngineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);

                if (usingMPH is true)
                {
                    DrawText2d(0.148f, 0.5475f, 0.55f, "mph", 255, 255, 255, 200);
                    DrawText2d(0.13f, 0.5475f, 0.55f, $"{Math.Ceiling(vehicle.Speed * 2.236936f)}", 255, 255, 255, 200);
                }
                else
                {
                    DrawText2d(0.13f, 0.5475f, 0.55f, $"{Math.Ceiling(vehicle.Speed * 3.6f)}", 255, 255, 255, 200);
                    DrawText2d(0.148f, 0.5475f, 0.55f, "kph", 255, 255, 255, 200);
                }
            }

            if (vehicle.Model.IsHelicopter || vehicle.Model.IsPlane)
            {
                DrawText2d(0.0299f, 3.95f, 0.47f, "~r~Cruise", 255, 255, 255, 210);

                DrawText2d(0.0510f, 3.5f, 0.47f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);
                DrawText2d(0.0735f, 3.65f, 0.66f, vehicle.Mods.LicensePlate, 255, 255, 255, 200);
                DrawText2d(0.1560f, 3.95f, 0.47f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200);

                DrawText2d(0.1610f, 3.5f, 0.47f, vehicle.BodyHealth < 310 ? "~g~AC" : vehicle.BodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200);
                DrawText2d(0.1380f, 3.5f, 0.47f, vehicle.EngineHealth < 110 ? "~r~Fluid" : vehicle.EngineHealth < 315 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200);
                DrawText2d(0.0299f, 3.5f, 0.47f, vehicle.EngineHealth < 110 ? "~r~Oil" : vehicle.EngineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
            }
        }
        #endregion
    }
}
