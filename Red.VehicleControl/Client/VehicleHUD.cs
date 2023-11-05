using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Red.Common.Client.Hud;
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

            HUD.DrawRect(0.132f, 0.9475f, 0.045f, 0.03f, 16, 16, 16, 185);

            if (vehicle.Model.IsCar || vehicle.Model.IsVehicle)
            {
                DrawText2d(0.018f, 0.77f, 0.5f, vehicle.EngineHealth < 110 ? "~r~Oil" : vehicle.EngineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
                DrawText2d(0.04f, 0.77f, 0.5f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);
                DrawText2d(0.09f, 0.768f, 0.57f, vehicle.Mods.LicensePlate, 255, 255, 255, 200, Alignment.Center);
                DrawText2d(0.14f, 0.77f, 0.5f, vehicle.EngineHealth < 110 ? "~r~Fluid" : vehicle.EngineHealth < 315 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200, Alignment.Right);
                DrawText2d(0.157f, 0.77f, 0.5f, vehicle.BodyHealth < 310 ? "~g~AC" : vehicle.BodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200, Alignment.Right);
                DrawText2d(0.157f, 0.745f, 0.5f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200, Alignment.Right);

                if (usingMPH is true)
                {
                    DrawText2d(0.133f, 0.93f, 0.55f, "mph", 255, 255, 255, 200);
                    DrawText2d(0.114f, 0.93f, 0.55f, $"{Math.Ceiling(vehicle.Speed * 2.236936f)}", 255, 255, 255, 200);
                }
                else
                {
                    DrawText2d(0.13f, 0.5475f, 0.55f, $"{Math.Ceiling(vehicle.Speed * 3.6f)}", 255, 255, 255, 200);
                    DrawText2d(0.148f, 0.5475f, 0.55f, "kph", 255, 255, 255, 200);
                }
            }

            if (vehicle.Model.IsHelicopter || vehicle.Model.IsPlane)
            {
                DrawText2d(0.018f, 0.77f, 0.5f, vehicle.EngineHealth < 110 ? "~r~Oil" : vehicle.EngineHealth < 315 ? "~y~Oil" : "Oil", 255, 255, 255, 200);
                DrawText2d(0.04f, 0.77f, 0.5f, vehicle.IsInBurnout ? "~r~DSC" : "DSC", 255, 255, 255, 200);
                DrawText2d(0.09f, 0.768f, 0.57f, vehicle.Mods.LicensePlate, 255, 255, 255, 200, Alignment.Center);
                DrawText2d(0.14f, 0.77f, 0.5f, vehicle.EngineHealth < 110 ? "~r~Fluid" : vehicle.EngineHealth < 315 ? "~y~Fluid" : "Fluid", 255, 255, 255, 200, Alignment.Right);
                DrawText2d(0.157f, 0.77f, 0.5f, vehicle.BodyHealth < 310 ? "~g~AC" : vehicle.BodyHealth < 900 ? "~y~AC" : "AC", 255, 255, 255, 200, Alignment.Right);
                DrawText2d(0.157f, 0.745f, 0.5f, vehicle.IsEngineRunning ? "~g~ENG" : "~r~ENG", 255, 255, 255, 200, Alignment.Right);

                DrawText2d(0.133f, 0.93f, 0.55f, "mph", 255, 255, 255, 200);
                DrawText2d(0.114f, 0.93f, 0.55f, $"{Math.Ceiling(vehicle.HeightAboveGround * 3.2808f)}", 255, 255, 255, 200);
            }
        }
        #endregion
    }
}
