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
                ErrorNotification("Your BAC level can't be more or than 5 characters.");
                return;
            }

            if (args.Length != 0)
            {
                bac = args[0];
                DisplayNotification($"~g~Your BAC level is now set to {bac}", true);
            }

            if (args.Length == 0)
            {
                var bacUserInput = await GetUserInput("Set BAC Level (Legal Limit is 0.08)", 5);
                bac = bacUserInput;

                if (string.IsNullOrEmpty(bacUserInput))
                {
                    ErrorNotification("You can't leave this blank!");
                    return;
                }

                if (bacUserInput.Length > 5)
                {
                    ErrorNotification("BAC level can't be more than 5 characters!");
                    return;
                }

                DisplayNotification($"~g~Your BAC level is now set to {bac}", true);
            }
        }

        [Command("mybac")]
        private void MyBacCommand() => AddChatMessage("[Breathalyzer]", $"Your BAC level is set to: {bac}", 255, 0, 0);
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:doBacTest")]
        private async void OnDoBacTest(string testerId) => TriggerServerEvent("Breathalyzer:Server:returnBacTest", testerId, bac);

        [EventHandler("Breathalyzer:Client:returnBacLevel")]
        private async void OnReturnBacLevel(string bacLevel)
        {
            bac = bacLevel;
            
            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_BAC",
                level = $"{bacLevel}"
            }));

            await Delay(500);
            PlaySoundFrontend(-1, "5_SEC_WARNING", "HUD_MINI_GAME_SOUNDSET", true);
        }
        #endregion

        #region NUI Callbacks
        private async void StartBacTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            Player targetPlayer = GetClosestPlayer(2f);

            if (targetPlayer is null)
            {
                ErrorNotification("You need to be closer the player you wish to test.");
                return;
            }

            PlayerPed.PlayAnim("weapons@first_person@aim_rng@generic@projectile@shared@core", "idlerng_med", 1.0f, -1, 5000, 50, 0, false, false, false);
            await Delay(5000);
            
            TriggerServerEvent("Breathalyzer:Server:submitBacTest", targetPlayer.ServerId);
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
                    level = "0.00"
                }));

                SetNUIFocus(true, true);
                return true;
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);
            return false;
        }
        #endregion
    }
}