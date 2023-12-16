using System.Collections.Generic;
using Red.Common.Client;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;
using System;

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
            RegisterNUICallback("closeNUI", CloseNUI);
            RegisterNUICallback("startTest", StartBacTest);
            RegisterNUICallback("restartTest", ResetBacTest);
        }
        #endregion

        #region Commands
        [Command("breathalyzer")]
        private void BreathalyzerCommand()
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI",
                level = bacLevel
            }));

            SetNUIFocus(true, true);
            displayNui = true;
        }

        [Command("bac")]
        private void BacCommand() => BreathalyzerCommand();
        #endregion

        #region NUI Callbacks
        private void StartBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            Player closestPlayer = GetClosestPlayer(3f);

            if (closestPlayer is null)
            {
                ErrorNotification("You need to be near a player to do this.");
                return;
            }

            PlaySoundFrontend(-1, "PIN_BUTTON", "ATM_SOUNDS", true);
            TriggerServerEvent("Breathalyzer:Server:doBacTest", closestPlayer.ServerId);
        }

        private void ResetBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "RESET_BAC"
            }));

            SetNUIFocus(true, true);
            result(new { success = true, msg = "success" });
        }

        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);
            displayNui = false;

            result(new { success = true, msg = "success" });
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyer:Client:requestBac")]
        private async void OnRequestBac()
        {
            await Delay(0);

            if (IsControlJustReleased(1, 246))
            {
                var result = await GetUserInput("Set BAC Level (Legal limit is 0.08):", 4);

                if (string.IsNullOrEmpty(result))
                {
                    ErrorNotification("You must put a valid BAC level.");
                    return;
                }

                if (double.TryParse(result, out var value))
                {
                    bacLevel = value;
                    TriggerServerEvent("Breathalyzer:Server:returnBac", bacLevel);
                }
            }
        }
        #endregion
    }
}