using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected double bacLimit;
        protected bool startBac, displayNUI, bacBeenSet;
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
        }
        #endregion

        #region Commands
        #endregion

        #region NUI Callbacks
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Breathalyzer", "LegalLimit") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                bacLimit = loaded["Breathalyzer"]["LegalLimit"].DoubleValue;
            }
            else
            {
                Debug($"[Breathalyzer]: Config file has not been configured correctly.");
            }
        }

        private void DisplayNUI(bool display = true)
        {
            if (!display)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "DISPLAY_NUI"
                }));

                SetNuiFocus(false, false);
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI"
            }));

            SetNuiFocus(true, true);
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:openNUI")]
        private void OnOpenNUI()
        {
            DisplayNUI();

            if (startBac is false)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "BAC_LEVEL",
                    level = "0.00"
                }));
            }

            startBac = true;
        }

        [EventHandler("Breathalyzer:Client:bacError")]
        private void OnBacError(double level)
        {
            AddChatTemplate("", "");
            AddChatMessage("[Breathalyzer]", $"{level}", 44, 230, 41);
        }

        [EventHandler("Breathalyzer:Client:bacSuccess")]
        private void OnBacSuccess(double level)
        {

        }

        [EventHandler("Breathalyzer:Client:bacResult")]
        private void OnBacResult(double level)
        {
            Wait(5000);
            PlaySoundFrontend(-1, "5_Second_Timer", "DLC_HEISTS_GENERAL_FRONTEND_SOUNDS", false);

            if (level is -0.01)
            {
                level = 0.00;
            }

            if (level > bacLimit - 0.01)
            {
                SendNuiMessage(Json.Stringify(new
                {

                }));
            }
            else if (level > 0.00 && level < bacLimit)
            {

            }
            else
            {

            }
        }
        #endregion

        #region Ticks
        #endregion
    }
}