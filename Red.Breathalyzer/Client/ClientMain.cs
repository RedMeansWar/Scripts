using System;
using System.Collections.Generic;
using Red.Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool displayNui, bacBeenSet;
        protected double bacLevel = 0.00;
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNuiCallback("closeNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CloseNUI));
            RegisterNuiCallback("startTest", new Action<IDictionary<string, object>, CallbackDelegate>(StartBacTest));
            RegisterNuiCallback("restartTest", new Action<IDictionary<string, object>, CallbackDelegate>(ResetBacTest));

            RegisterNUICallback("", CloseNUI);
        }
        #endregion

        #region Commands
        [Command("breathalyzer")]
        private void BreathalyzerCommand()
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI"
            }));

            SetNuiFocus(true, true);
        }

        [Command("bac")]
        private void BacCommand() => BreathalyzerCommand();
        #endregion

        #region NUI Callbacks
        private void StartBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void ResetBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {

        }

        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNuiFocus(false, false);
            result(new { success = true, msg = "success" });
        }
        #endregion
    }
}