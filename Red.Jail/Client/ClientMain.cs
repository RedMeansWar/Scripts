using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common;
using Red.Common.Client;
using SharpConfig;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;

namespace Red.Jail.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected readonly Vector3 jailPosition = new(1788.8f, 2581.9f, 44.88f); // Noclip Position
        protected readonly Vector3 exitJailPosition = new(1835.55f, 2584.67f, 45.95f); // Normal Exit Position (In front enterance)
        protected readonly Vector3 releasePosition = new(1848.62f, 2585.95f, 45.67f);
        protected readonly Vector3 jailTeleportPosition = new(1662.6f, 2615.29f, 45.50f);
        protected DateTime releaseTime;
        protected DateTime lastNotifiedTime = DateTime.MinValue;
        protected readonly Random random = new();
        protected Character currentCharacter;
        protected int maxJailTime, minJailTime; // config variables
        protected bool isJailed, isLockedDown, isJailReady, displayNUI;

        private List<ReducingActivity> completedActivites = new();

        private readonly IReadOnlyList<Vector3> jailInterfaces = new List<Vector3>
        {
            new Vector3(459.79f, -989.13f, 24.91f), // Mission Row PD, Lower booking area
            new Vector3(1853.11f, 3690.12f, 34.27f), // Sandy Shores PD, desk
            new Vector3(-449.48f, 6012.42f, 31.72f), // Paleto Bay PD, desk
            new Vector3(1838.5f, 2590.7f, 45.9f), // Bolingbroke Prison, door
        };

        private readonly IReadOnlyList<ReducingActivity> reducingActivities = new List<ReducingActivity>
        {
            new(new(1774.5f, 2597.6f, 45.8f), 273.4f, 20, "clean2", "Clean Gym"),
            new(new(1778.4f, 2592.1f, 45.8f), 3.9f, 20, "warmth", "Prepare Food"),
            new(new(1786.8f, 2557.2f, 45.8f), 292.2f, 20, "janitor", "Sweep Floor"),
            new(new(1645.6f, 2536.9f, 45.6f), 184.4f, 20, "pushup", "Workout"),
            new(new(1765.9f, 2579.5f, 45.8f), 314.4f, 20, "pickup", "Pickup towels"),
        };

        private readonly IReadOnlyList<Vector3> teleportLocations = new List<Vector3>()
        {
            new(1789f, 2585.9f, 45.8f),
            new(1788.5f, 2582.3f, 45.8f),
            new(1789.1f, 2578.5f, 45.8f),
            new(1788.6f, 2574.4f, 45.8f),
            new(1770.2f, 2573.9f, 45.8f),
            new(1770.6f, 2577.5f, 45.8f),
            new(1770.6f, 2581.4f, 45.8f),
            new(1770.6f, 2585.5f, 45.8f),
            new(1785.5f, 2601f, 50.5f),
            new(1788.5f, 2598.2f, 50.5f),
            new(1788.5f, 2594.3f, 50.5f),
            new(1788.8f, 2590.3f, 50.5f),
            new(1788.4f, 2586.3f, 50.5f),
            new(1788.5f, 2582.5f, 50.5f),
            new(1788.8f, 2578.3f, 50.5f),
            new(1788.3f, 2574.4f, 50.5f),
            new(1786.1f, 2569f, 50.5f),
            new(1782.5f, 2569f, 50.5f),
            new(1778.1f, 2569.2f, 50.5f),
            new(1774.6f, 2569.2f, 50.5f),
            new(1770.6f, 2573.4f, 50.5f),
            new(1770.7f, 2577.4f, 50.5f),
            new(1770.7f, 2581.4f, 50.5f),
            new(1770.9f, 2585.4f, 50.5f),
            new(1770.8f, 2589.3f, 50.5f),
            new(1770.6f, 2593f, 50.5f),
            new(1770.5f, 2597.4f, 50.5f)
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNUICallback("submitPrisoner", SubmitPrisoner);
            RegisterNUICallback("closeNUI", CloseNUI);
        }
        #endregion

        #region Commands
        [Command("jail")]
        private void JailCommand() => displayNUI = true;
        #endregion

        #region NUI Callbacks
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

        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNUIFocus(false, false);
            result(new { success = true, message = "success" });
        }

        #endregion

        #region Methods
        private void DisplayInterface()
        {
            if (!displayNUI)
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_NUI",
                    players = Players.Where(p => Entity.Exists(p.Character) && p.Character.CalculateDistanceTo(PlayerPed.Position) < 30f).OrderBy(p => p.Character.CalculateDistanceTo(PlayerPed.Position)).Select(x => new { x.ServerId, x.Name }).ToArray()
                }));

                SetNUIFocus(true, true);
                displayNUI = true;
            }
            else
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_NUI"
                }));

                SetNUIFocus(false, false);
                displayNUI = false;
            }
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

        private void ReleasePrisoner()
        {
            isJailed = false;
            releaseTime = DateTime.MinValue;
            completedActivites = new();
            Tick -= JailTick;
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

        private static async Task SwitchOut(Vector3 position, int delay)
        {
            PlayerPed.IsInvincible = true;
            PlayerPed.IsInvincible = true;

            PlayerPed.IsCollisionEnabled = false;
            PlayerPed.HasGravity = false;

            SwitchOutPlayer(PlayerPed.Handle, 0, 1);
            await Delay(delay);

            PlayerPed.Position = position;
            SwitchInPlayer(PlayerPed.Handle);

            await Delay(delay);

            PlayerPed.IsInvincible = false;
            PlayerPed.IsCollisionEnabled = true;
            PlayerPed.HasGravity = true;

            await Delay(delay);
        }
        #endregion

        #region Event Handlers
        [EventHandler("Jail:Client:jailedPlayer")]
        private async void OnJailedPlayer(int length, string src, string reason, bool distanceCheck = true)
        {
            isJailReady = !isJailReady;

            if (!int.TryParse(src, out int networkId))
            {
                return;
            }

            Player jailer = Players[networkId];
            float distance = PlayerPed.CalculateDistanceTo(jailer.Character.Position);

            if (distance > 20 && distanceCheck)
            {
                TriggerServerEvent("Jail:Server:jailTooFar", src);
                return;
            }

            if (length > maxJailTime)
            {
                length = maxJailTime;
            }
            else if (length <= 0)
            {
                length = 1;
            }

            PlayerPed.Weapons.RemoveAll();

            if (PlayerPed.Model.Hash == GetHashKey("mp_f_freemode_01"))
            {
                SetPedComponentVariation(PlayerPed.Handle, 3, 15, 0, 0);
                SetPedComponentVariation(PlayerPed.Handle, 4, 71, 6, 0);
                SetPedComponentVariation(PlayerPed.Handle, 6, 5, 0, 0);
                SetPedComponentVariation(PlayerPed.Handle, 8, 15, 0, 0);
                SetPedComponentVariation(PlayerPed.Handle, 11, 36, 0, 0);
            }
            else
            {
                if (random.Next(2) == 0)
                {
                    SetPedComponentVariation(PlayerPed.Handle, 3, 15, 0, 0);
                    SetPedComponentVariation(PlayerPed.Handle, 4, 71, 6, 0);
                }
                else
                {
                    SetPedComponentVariation(PlayerPed.Handle, 3, 15, 0, 0);
                    SetPedComponentVariation(PlayerPed.Handle, 4, 71, 6, 0);
                }

                SetPedComponentVariation(PlayerPed.Handle, 4, 3, 7, 2);
                SetPedComponentVariation(PlayerPed.Handle, 8, 60, 500, 2);
                SetPedComponentVariation(PlayerPed.Handle, 6, 6, random.Next(2), 2);
            }

            if (isJailReady)
            {
                await SwitchOut(jailTeleportPosition, 2500);
            }
            else
            {
                await SwitchOut(teleportLocations[random.Next(teleportLocations.Count)], 2500);
            }

            releaseTime = DateTime.UtcNow.AddSeconds(length);
            Scaleform("~r~JAILED", $"You've been jailed for {length} months");

            isJailed = true;
            Tick += JailTick;
        }

        [EventHandler("Jail:Client:unjailPlayer")]
        private async void OnUnjailPlayer()
        {
            if (!isJailed)
            {
                return;
            }

            ReleasePrisoner();
            await SwitchOut(releasePosition, 1800);
            Scaleform("~b~RELEASED", "You've been released from jail");
        }
        #endregion

        #region Ticks
        private async Task JailTick()
        {
            if (isJailed)
            {
                if (PlayerPed.CalculateDistanceTo(jailPosition) > 280f)
                {
                    PlayerPed.Position = jailPosition;
                    releaseTime = releaseTime.AddSeconds(30);

                }
            }

            ReducingActivity activity = reducingActivities.First(a => PlayerPed.CalculateDistanceTo(a.Position) < 1f);

            if (activity is not null && !completedActivites.Contains(activity))
            {
                DisplayHelpText($"Press ~INPUT_CONTEXT~ to {activity.Description}");

                if (Game.IsControlJustPressed(0, Control.Context))
                {
                    DateTime end = DateTime.UtcNow.AddSeconds(20);
                    Debug.WriteLine("release time " + releaseTime.ToString());

                    TriggerEvent("dpEmotes:PlayEmote", activity.Animation);
                    while (PlayerPed.CalculateDistanceTo(activity.Position) < 2f && DateTime.UtcNow < end)
                    {
                        await Delay(1500);

                        if (activity.Animation == "pickup")
                        {
                            TriggerEvent("dpEmotes:PlayEmote", activity.Animation);
                        }
                    }

                    TriggerEvent("dpEmote:StopEmote");
                    releaseTime = releaseTime.AddSeconds(-20.0);

                    if (releaseTime < DateTime.UtcNow)
                    {
                        releaseTime = DateTime.UtcNow.AddSeconds(1);
                    }

                    Debug.WriteLine("release time is now" + releaseTime.ToString());
                    DisplayNotification("~g~20 months have been taken off your sentence!", true);

                    completedActivites.Add(activity);
                    await Delay(1000);
                }
            }
        }

        [Tick]
        private async Task CheckLocation()
        {
            if (PlayerPed.CalculateDistanceTo(new(1905f, 2601f, 45f)) < 100)
            {
                Prop entryProp = new(GetClosestObjectOfType(1897f, 2606f, 45f, 1f, 3110450777, false, false, false));
                Prop exitProp = new(GetClosestObjectOfType(1905f, 2604f, 45f, 1f, 3110450777, false, false, false));

                if (Entity.Exists(entryProp))
                {
                    entryProp.Delete();
                }

                if (Entity.Exists(exitProp))
                {
                    exitProp.Delete();
                }
            }

            if (jailInterfaces.Any(location => PlayerPed.CalculateDistanceTo(location) < 1f))
            {
                DisplayHelpText("Press ~INPUT_CONTEXT~ to open the jail interface.");

                if (Game.IsControlJustPressed(0, Control.Context))
                {
                    DisplayInterface();
                }
            }

            if (isJailed)
            {
                double timeLeft = releaseTime.Subtract(DateTime.UtcNow).TotalSeconds;

                if (timeLeft > 0)
                {
                    if (lastNotifiedTime != DateTime.MaxValue && DateTime.UtcNow.Subtract(lastNotifiedTime).TotalSeconds > 30.0)
                    {
                        lastNotifiedTime = DateTime.MaxValue;
                        ChatMessage("[Warden]", $"{Math.Ceiling(timeLeft)} more months until you're released.", 0, 100, 255);
                        await Delay(1000);
                    }
                }
                else
                {
                    if (isJailReady)
                    {
                        OnUnjailPlayer();
                        return;
                    }

                    if (lastNotifiedTime != DateTime.MaxValue)
                    {
                        lastNotifiedTime = DateTime.MaxValue;
                        ChatMessage("[Warden]", "You are eligble for release. Go to the front to be released.", 0, 100, 255);
                        await Delay(1000);
                    }

                    DrawText3d(exitJailPosition.X, exitJailPosition.Y, exitJailPosition.Z, "Go here to be released", 1f, 5f, 255, 255, 255, 255);

                    if (PlayerPed.CalculateDistanceTo(exitJailPosition) < 3f)
                    {
                        DisplayHelpText("Press ~INPUT_CONTEXT~ to be released.");

                        if (Game.IsControlJustPressed(0, Control.Context))
                        {
                            OnUnjailPlayer();
                        }
                    }
                }

                if (isJailReady)
                {
                    return;
                }

            }

        }
        #endregion

        #region Classes
        internal class ReducingActivity
        {
            public Vector3 Position { get; set; }
            public float Heading { get; set; }
            public int ReductionAmount { get; set; }
            public string Animation { get; set; }
            public string Description { get; set; }

            public ReducingActivity(Vector3 position, float heading, int reductionAmount, string animation, string description)
            {
                Position = position;
                Heading = heading;
                ReductionAmount = reductionAmount;
                Animation = animation;
                Description = description;
            }
        }

        internal class DiggingTunnel
        {

        }
        #endregion
    }
}