using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.Breathalyzer.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool nuiDisplayed = false;
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNuiCallback("openNUI", new Action<IDictionary<string, object>, CallbackDelegate>(DisplayBreathalyzer));
            RegisterNuiCallback("closeNUI", new Action<IDictionary<string, object>, CallbackDelegate>(HideBreathalyzer));
            RegisterNuiCallback("startBacTest", new Action<IDictionary<string, object>, CallbackDelegate>(StartBreathalyzerTest));
        }
        #endregion

        #region Commands
        [Command("breathalyzer")]
        private void BreathalyzerCommand() => DisplayNUI();
        #endregion

        #region Methods
        private void DisplayNUI(bool display = true)
        {
            nuiDisplayed = display;

            if (nuiDisplayed == false)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI",
                }));

                Debug.WriteLine("[Breathalyzer]: Revoking NUI");
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_UI"
            }));

            Debug.WriteLine("[Breathalyzer]: Invoking NUI");
        }
        #endregion

        #region NUI Callbacks
        private void DisplayBreathalyzer(IDictionary<string, object> data, CallbackDelegate result) => DisplayNUI();
        private void HideBreathalyzer(IDictionary<string, object> data, CallbackDelegate result) => DisplayNUI(false);

        private void StartBreathalyzerTest(IDictionary<string, object> data, CallbackDelegate result)
        {
            DisplayNUI();

            Player targetPlayer = GetClosestPlayer();

            if (targetPlayer is null)
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You need to be near a player to use this.", true);
                return;
            }

            TriggerServerEvent("Bac:Server:startBacTest", targetPlayer.ServerId);
        }
        #endregion

        #region Methods
        private Player GetClosestPlayer(float radius = 2f)
        {
            Vector3 plyPos = Game.PlayerPed.Position;
            Player closestPlayer = null;
            float closestDist = float.MaxValue;

            foreach (Player p in Players)
            {
                if (p is null || p == Game.Player)
                {
                    continue;
                }

                float dist = Vector3.DistanceSquared(p.Character.Position, plyPos);
                if (dist < closestDist && dist < radius)
                {
                    closestPlayer = p;
                    closestDist = dist;
                }
            }

            return closestPlayer;
        }
        #endregion

        #region Ticks
        private async Task DisableControlsTick()
        {
            while (nuiDisplayed)
            {
                Wait(0);

                DisableControlAction(0, 1, nuiDisplayed); // INPUT_LOOK_LR (Mouse Right)
                DisableControlAction(0, 2, nuiDisplayed); // INPUT_LOOK_UD (Mouse Down)
                DisableControlAction(0, 18, nuiDisplayed); // INPUT_SKIP_CUTSCENE (Enter)
                DisableControlAction(0, 106, nuiDisplayed); // INPUT_VEH_MOUSE_CONTROL_OVERRIDE (Left Mouse Button) - Stop the player from getting to a vehicle.
                DisableControlAction(0, 142, nuiDisplayed); // INPUT_MELEE_ATTACK_ALTERNATE (Left Mouse Button) - Stop the player from attacking.
                DisableControlAction(0, 322, nuiDisplayed); // INPUT_REPLAY_TOGGLE_TIMELINE	(Escape)
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Breathalyzer:Client:requestBac")]
        private void OnRequestBacLevel()
        {

        }
        #endregion
    }

    public static class Extensions
    {
        public static T GetValue<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.TryGetValue(key, out object value) && value is T t)
            {
                return t;
            }

            return defaultVal;
        }
    }
}