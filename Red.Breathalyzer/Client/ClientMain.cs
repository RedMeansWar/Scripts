using System;
using System.Collections.Generic;
using Red.Common.Client;
using CitizenFX.Core;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;
using System.Reflection;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected string bac = "0.00";
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNUICallback("startTest", StartBacTest);
            RegisterNUICallback("resetTest", ResetBacTest);
            RegisterNUICallback("closeNUI", CloseDisplay);
        }
        #endregion

        #region Commands
        [Command("bac")]
        private void BacCommand() => DisplayNUI(true);

        [Command("breathalyzer")]
        private void BreathalyzerCommand() => DisplayNUI(true);

        [Command("resetbac")]
        private void ResetBacCommand() => bac = "0.00";

        [Command("setbac")]
        private async void SetBacCommand(string[] args)
        {
            if (args.Length > 5)
            {
                ErrorNotification("Your BAC level can't be more than 5 characters.", false);
                return;
            }

            if (args.Length == 0)
            {
                var bacUserInput = await GetUserInput("Set BAC Level (Legal Limit is 0.08):", 5);
                bac = bacUserInput;

                if (string.IsNullOrEmpty(bacUserInput))
                {
                    ErrorNotification("You can't leave this blank!");
                    return;
                }
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:doBacTest")]
        private async void OnDoBacTeste(string testerId) => TriggerServerEvent("Breathalyzer:Server:returnBacTest", testerId, bac);

        [EventHandler("Breathalyzer:Client:displayClientNotification")]
        private void OnDisplayClientNotification(string message) => DisplayNotification(message);
        #endregion

        #region NUI Callbacks
        private void StartBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            Player targetPlayer = GetClosestPlayer(1.8f);

            if (targetPlayer is null)
            {
                ErrorNotification("You need to be near a player to use this.");
                return;
            }

            TriggerServerEvent("Breathalyzer:Server:returnBacTest", targetPlayer.ServerId);
        }

        private void ResetBacTest(IDictionary<string, object> data, CallbackDelegate result) => SendNUIMessage(Json.Stringify(new { type = "RESET_NUI" }));

        private void CloseDisplay(IDictionary<string, object> data, CallbackDelegate result) => DisplayNUI(false);
        #endregion

        #region Methods
        private bool DisplayNUI(bool display)
        {
            if (display)
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_NUI",
                    bacLevel = bac
                }));

                SetNUIFocus(true, true);
                return true;
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            return false;
        }
        #endregion
    }
}