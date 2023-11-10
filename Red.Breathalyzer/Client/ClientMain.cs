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
        protected bool isDisplayed = IsNuiFocused();
        protected bool bacBeenSet, start;
        protected double limit;
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();

            // NUI Registration
            RegisterNuiCallback("startBreathalyzer", new Action<IDictionary<string, object>, CallbackDelegate>(StartBreathalyzer));
            RegisterNuiCallback("resetBreathalyzer", new Action<IDictionary<string, object>, CallbackDelegate>(ResetBreathalyzer));
            RegisterNuiCallback("cancelNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CancelNUI));
        }
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Breathalyzer", "LegalLimit") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                limit = loaded["Breathalyzer"]["LegalLimit"].DoubleValue;
            }
            else
            {
                Debug.WriteLine($"[Breathalyzer]: Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:requestBreathalyze")]
        private void OnRequestBreathalyze()
        {
            if (bacBeenSet is false)
            {
                PlaySoundFrontend(-1, "NAV", "HUD_AMMO_SHOP_SOUNDSET", true);

                var input = GetUserInput($"Set your BAC level, legal limit: {limit}", "0.00", 4);

                if (input is null)
                {
                    AddChatMessage("", "");
                }

            }
        }
        #endregion

        #region NUI Callbacks
        private void StartBreathalyzer(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void ResetBreathalyzer(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void CancelNUI(IDictionary<string, object> data, CallbackDelegate result)
        {

        }
        #endregion
    }
}