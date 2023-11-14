using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool displayUI, firstStart, setBAC;
        protected double bacLimit;
        protected Task bacLevel = GetUserInput("Set your BAC level", "0.00", 4);
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
            // Callbacks
            RegisterNuiCallback("startBac", new Action<IDictionary<string, object>, CallbackDelegate>(StartBAC));
            RegisterNuiCallback("resetBac", new Action<IDictionary<string, object>, CallbackDelegate>(ResetBAC));
            RegisterNuiCallback("closeNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CloseNUI));

            // Suggestions
            AddCommandSuggestion("setbac", "Set your blood alcohol concentration level");
            AddCommandSuggestion("bac", "Open the breathalyzer UI.");
            AddCommandSuggestion("baclevel", "Checks your BAC level");
        }
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
                Debug.WriteLine($"[Breathalyzer]: Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Commands
        [Command("setbac")]
        private void SetBacCommand(string[] args)
        {
            if (args.Length < 4)
            {

            }
            else if (args.Length == 0)
            {
                if (setBAC is false)
                {
                    PlaySoundFrontend(-1, "NAV", "HUD_AMMO_SHOP_SOUNDSET", true);

                    if (bacLevel is null)
                    {

                    }

                    AddChatMessage("[Breathalyzer]", $"BAC Level was set to {bacLevel}");
                    TriggerServerEvent("Breathalyzer:Server:setBAC", "0.00");

                    setBAC = true;
                }
                else
                {
                    TriggerServerEvent("Breathalyzer:Server:setBAC", bacLevel);
                    setBAC = true;
                }
            }
        }

        [Command("baclevel")]
        private void BacLevelCommand()
        {
            AddChatMessage("[Breathalyzer]", $"your BAC level is {bacLevel}", 255, 0, 0);
        }
        #endregion

        #region NUI Callbacks
        private void StartBAC(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void ResetBAC(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {

        }
        #endregion

        #region Events
        [EventHandler("Breathalyzer:Client:OpenUI")]
        private void OnOpenUI()
        {
            SetNuiFocus(true, true);

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_UI"
            }));
        }

        [EventHandler("Breathalyzer:Client:Success")]
        private void OnBacSuccess()
        {

        }

        [EventHandler("Breathalyzer:Client:Error")]
        private void OnBacError()
        {

        }

        [EventHandler("Breathalyer:Client:Result")]
        private void OnResult()
        {

        }
        #endregion
    }
}