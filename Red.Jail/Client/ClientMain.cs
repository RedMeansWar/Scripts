using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common;
using Red.Common.Client;
using SharpConfig;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.NUI;

namespace Red.Jail.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected readonly Vector3 teleportLocation = new(1662.6f, 2615.29f, 45.50f);
        protected readonly Vector3 jailLocation = new(1788.8f, 2581.9f, 44.88f);
        protected readonly Vector3 prisonLocation = new(1848.62f, 2585.95f, 45.67f);
        protected readonly Vector3 lockdownPosition = new(1779.1f, 2590.6f, 50.5f);
        protected readonly Vector3 exitJailPosition = new(1835.55f, 2584.67f, 45.95f);
        protected DateTime releaseTime;
        protected DateTime lastNotifiedTime = DateTime.MinValue;
        protected int maxJailTime, minJailTime;
        protected bool isJailed, isLockedDown;

        private readonly IReadOnlyList<ReducingActivity> reducingActivities = new List<ReducingActivity>
        {
            new(new(1774.5f, 2597.6f, 45.8f), 273.4f, 20, "clean2", "Clean Gym"),
            new(new(1778.4f, 2592.1f, 45.8f), 3.9f, 20, "warmth", "Prepare Food"),
            new(new(1786.8f, 2557.2f, 45.8f), 292.2f, 20, "janitor", "Sweep Floor"),
            new(new(1645.6f, 2536.9f, 45.6f), 184.4f, 20, "pushup", "Workout"),
            new(new(1765.9f, 2579.5f, 45.8f), 314.4f, 20, "pickup", "Pickup towels"),
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNUICallback("submitPrisoner", SubmitPrisoner);
        }
        #endregion

        #region Commands
        [Command("jail")]
        private void JailCommand() => DisplayInterface(true);
        #endregion

        #region NUI Callbacks
        #endregion

        #region Methods
        private bool DisplayInterface(bool display)
        {
            if (display)
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_NUI"
                }));

                SetNUIFocus(true, true);
            }

            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);

            return false;
        }

        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Jail", "MaximumJailSentence") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                maxJailTime = loaded["Jail"]["MaxJailSentence"].IntValue;
                minJailTime = loaded["Jail"]["MinimumJailSentence"].IntValue;
            }
            else
            {
                TriggerServerEvent("Jail:Server:configError", "Config file wasn't setup correctly!");
            }
        }
        #endregion

        #region Event Handlers
        private void SubmitPrisoner(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (int.TryParse(data["time"].ToString(), out int length) && int.TryParse(data["id"].ToString(), out int id))
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_NUI"
                }));

                SetNUIFocus(false, false);
                Debug.WriteLine("Revoking Nui Callback");

                if (length > maxJailTime)
                {
                    length = maxJailTime;
                }
                else if (length <= minJailTime)
                {
                    length = minJailTime;
                }
                else if (length <= 0)
                {
                    length = 1;
                }

                TriggerServerEvent("Jail:Server:submitToJail", id, length, data["reason"].ToString());
            }
           
            result(new { success = true, message = "sucess" });
        }

        private async void Scaleform(string mainText, string description)
        {
            int scaleform = RequestScaleformMovie("mp_big_message_freemode");

            while (!HasScaleformMovieLoaded(scaleform))
            {
                await Delay(0);
            }

            PushScaleformMovieFunction(scaleform, "SHOW_SHARD_WASTED_MP_MESSAGE");
            PushScaleformMovieFunctionParameterString(mainText);

            PushScaleformMovieFunctionParameterString(description);
            PopScaleformMovieFunction();

            int timeout = 2000;

            while (timeout > 0)
            {
                DrawScaleformMovieFullscreen(scaleform, 255, 255, 255, 255, 0);
                await Delay(5);

                timeout -= 5;
            }

            SetScaleformMovieAsNoLongerNeeded(ref scaleform);
        }
        #endregion

        #region Classes
        internal class ReducingActivity
        {
            public Vector3 Position { get; set; }
            public float Heading { get; set; }
            public int ReductionAmount { get; set; }
            public string Animation { get; set; }
            public string ActivityName { get; set; }

            public ReducingActivity(Vector3 position, float heading, int reductionAmount, string animation, string activityName)
            {
                Position = position;
                Heading = heading;
                ReductionAmount = reductionAmount;
                Animation = animation;
                ActivityName = activityName;
            }
        }
        #endregion
    }
}