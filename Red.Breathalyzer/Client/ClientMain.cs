using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        protected bool bacBeenSet;
        #endregion

        #region Constructor
        public ClientMain()
        {
            isDisplayed = !isDisplayed;

            // NUI Registration
            RegisterNuiCallback("startBreathalyzer", new Action<IDictionary<string, object>, CallbackDelegate>(StartBreathalyzer));
            RegisterNuiCallback("resetBreathalyzer", new Action<IDictionary<string, object>, CallbackDelegate>(ResetBreathalyzer));
            RegisterNuiCallback("cancelNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CancelNUI));
        }
        #endregion

        #region Methods

        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:nuiError")]
        private void OnNuiError()
        {
            TriggerEvent("chat:addMessage", new
            {
                template = "<div style='background-color: rgba(250, 22, 10, 0.5); text-align: center; border-radius: 0.5vh; padding: 0.7vh; font-size: 1.7vh;'><b>Input must be a number!</b></div>"
            });
        }

        [EventHandler("Breathalyzer:Client:testSuccess")]
        private void OnTestSuccess(string value)
        {
            TriggerEvent("chat:addMessage", new
            {
                template = "<div style='background-color: rgba(44, 230, 41, 0.5); text-align: center; border-radius: 0.5vh; padding: 0.7vh; font-size: 1.7vh;'><b>SUCCESS: BAC set to {0}</b></div>",
                args = new[] { value }
            });
        }
        #endregion

        #region NUI Callbacks
        private void StartBreathalyzer(IDictionary<string, object> data, CallbackDelegate result)
        {
            PlaySoundFrontend(-1, "PIN_BUTTON", "ATM_SOUNDS", true);
            
            Player closestPlayer = GetClosestPlayer();
            Ped Character = Game.Player.Character;

            float distance = (closestPlayer is null) ? Vector3.Distance(Character.Position, closestPlayer.Character.Position) : -1f;

            if (distance != 1f && distance < 3f)
            {
                TriggerServerEvent("Breathalyzer:Server:startBacTest", closestPlayer.ServerId);
                DisplayNotification($"~b~Breathalyzer~w~: Testing " + closestPlayer.Name);
            }
            else
            {
                ErrorNotification("You need to be near a player to use this.");
            }
        }

        private void ResetBreathalyzer(IDictionary<string, object> data, CallbackDelegate result)
        {
            PlaySoundFrontend(-1, "PIN_BUTTON", "ATM_SOUNDS", true);
            DisplayNotification("~b~Breathalyzer~w~: Resetting...");

            SendNuiMessage(Json.Stringify(new
            {
                text = "0.00"
            }));
        }

        private void CancelNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "CANCEL_UI"
            }));

            SetNuiFocus(false, false);

            if (isDisplayed && Game.IsControlJustPressed(0, Control.SkipCutscene))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "CANCEL_UI"
                }));
                SetNuiFocus(false, false);
            }
        }
        #endregion
    }
}