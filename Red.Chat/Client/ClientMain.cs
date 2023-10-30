using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Chat.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool hasChatInit, chatActive;
        protected string twitterUsername;
        protected Character currentCharacter;
        protected List<ChatSuggestion> suggestionsList;
        protected Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Constructor
        public ClientMain() => RegisterNuiCallback("chatResult", new Action<IDictionary<string, object>, CallbackDelegate>(OnChatResult));
        #endregion

        #region Methods
        private void LoadSuggestions()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "suggestions.json");

                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.WriteLine("'suggestions.json' is null or whitespace! Won't be able to populate chat suggestions list.");
                    suggestionsList = new();
                    return;
                }

                List<ChatSuggestion> list = Json.Parse<List<ChatSuggestion>>(json);

                if (list is null)
                {
                    Debug.WriteLine("Couldn't populate chat suggestion list!");
                    suggestionsList = new();
                    return;
                }

                suggestionsList = list;
                Debug.WriteLine($"Loaded {suggestionsList.Count} chat suggestion(s)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:characterSelected")]
        private void OnCharacterSelected(string json)
        {
            currentCharacter = Json.Parse<Character>(json);
            twitterUsername = GetResourceKvpString($"red_chat_twitter_{currentCharacter.CharacterId}");

            if (string.IsNullOrEmpty(twitterUsername))
            {
                twitterUsername = $"{currentCharacter.FirstName}{currentCharacter.LastName}{currentCharacter.DoB:yy}";
            }
        }

        [EventHandler("chat:addMessage")]
        private void OnAddChatMessate(dynamic chatMessage)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = chatMessage
            }));
        }

        [EventHandler("chat:addTemplate")]
        private void OnChatAddTemplate(string id, string template)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_TEMPLATE_ADD",
                template = new
                {
                    id,
                    html = template
                }
            }));
        }

        [EventHandler("chat:chatMessage")]
        private void onChatMessage(dynamic author, dynamic colors, dynamic chatMessage, Vector3 authorPos)
        {
            string distanceString = "";

            if (!authorPos.IsZero)
            {
                distanceString = $" ({Math.Round(Vector3.DistanceSquared(authorPos, PlayerPed.Position))}m)";
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                template = author == "" ? "defaultAlt" : "default",
                message = new
                {
                    color = new[] { colors[0], colors[1], colors[2] },
                    multiline = true,
                    args = author == "" ? new[] { $"{chatMessage}" } : new[] { $"{author}{distanceString}", $"{chatMessage}" }
                }
             }));
        }

        [EventHandler("chat:radioMessage")]
        private void OnRadioMessage(string name, string message)
        {
            if (currentCharacter?.Department == "Civ")
            {
                return;
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 60, 179, 113 },
                    multiline = true,
                    args = new[] { $"[Radio] {name}", message }
                }
            }));
        }

        [EventHandler("chat:twitterMessage")]
        private void OnTwitterMessage(string name, string message)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 0, 172, 238 },
                    multiline = true,
                    args = new[] { $"[Twitter] {name}", message }
                }
            }));
        }

        [EventHandler("chat:911Message")]
        private void On911Message(string name, string message)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 205, 92, 92 },
                    multiline = true,
                    args = new[] { $"[911] {name}", message }
                }
            }));
        }

        [EventHandler("chat:311Message")]
        private void On311Message(string name, string message)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 205, 92, 92 },
                    multiline = true,
                    args = new[] { $"[311] {name}", message }
                }
            }));
        }
        #endregion

        #region NUI Callbacks
        private async void OnChatResult(IDictionary<string, object> data, CallbackDelegate result)
        {
            chatActive = false;
            SetNuiFocus(false, false);
            string message = data.GetVal<string>("message", null);
            bool cancel = data.GetVal("canceled", false);

            if (!cancel && !string.IsNullOrWhiteSpace(message))
            {
                Vector3 plyPos = Game.PlayerPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArgs = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^4[OOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 0, 115, 255 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyPos) < 20f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/gooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:messageEntered", $"^*[GOOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs);
                        }
                        break;

                    case "/me":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^2[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 255, 255 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyPos) < 20f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/mer":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^*[ME] {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 0, 0 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyPos) < 20f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;
                    case "/meb":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^*[ME] {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 60, 139, 250 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyPos) < 20f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;
                    case "/gme":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:messageEntered", $"^* ^5[GME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs);
                        }
                        break;

                    case "/rt":
                        if (currentCharacter?.Department == "Civ")
                        {
                            TriggerEvent("Chat:Server:chatMessage", "System", new[] { 194, 39, 39 }, "You aren't authorized to use the /rt command. You must not be a civilian character.");
                            result(new { success = false, message = "not authorized" });
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:radioMessage", joinedArgs);
                        }
                        break;

                    case "/twt":
                    case "/twitter":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:twitterMessage", $"@{twitterUsername}", joinedArgs);
                        }
                        break;
                    case "/911":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            uint streetNameAsHash = 0;
                            uint crossingRoadAsHash = 0;

                            GetStreetNameAtCoord(PlayerPed.Position.X, PlayerPed.Position.Y, PlayerPed.Position.Z, ref streetNameAsHash, ref crossingRoadAsHash);
                            string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                            string crossingRoad = GetStreetNameFromHashKey(crossingRoadAsHash);

                            string location = $"{Exports["core_framework"].getClosestPostal(PlayerPed.Position)}, {streetName}";

                            if (!string.IsNullOrWhiteSpace(crossingRoad))
                            {
                                location += $" / {crossingRoad}";
                            }

                            TriggerServerEvent("Chat:Server:911Message", location, joinedArgs);
                        }
                        break;
                    case "/311":
                        uint streetNameHash = 0;
                        uint crossingRoadHash = 0;

                        GetStreetNameAtCoord(PlayerPed.Position.X,PlayerPed.Position.Y, PlayerPed.Position.Z, ref streetNameHash, ref crossingRoadHash);
                        string street = GetStreetNameFromHashKey(streetNameHash);
                        string crossRoad = GetStreetNameFromHashKey(crossingRoadHash);

                        string nonEmergencyLocation = $"{Exports["core_framework"].getClosestPostal(PlayerPed.Position)}, {street}";

                        if (!string.IsNullOrWhiteSpace(crossRoad))
                        {
                            nonEmergencyLocation += $" / {crossRoad}";
                        }
                        TriggerServerEvent("Chat:Server:311Message", nonEmergencyLocation, joinedArgs);
                        break;
                    default:
                        if (message.StartsWith("/"))
                        {
                            ExecuteCommand(message.Substring(1));
                            CancelEvent();
                        }
                        break;
                }
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ChatTick()
        {
            if (!hasChatInit)
            {
                hasChatInit = true;

                LoadSuggestions();

                foreach (ChatSuggestion suggestion in suggestionsList)
                {
                    SendNuiMessage($"{{\"type\":\"ON_SUGGESTION_ADD\",\"suggestion\":{{\"name\":\"{suggestion.Command}\",\"help\":\"{suggestion.Example}\",\"params\":\"\"}}}}");
                }

                SetTextChatEnabled(false);
                await Delay(1000);
            }

            if (!chatActive && Game.IsControlJustPressed(0, Control.MpTextChatAll) && UpdateOnscreenKeyboard() != 0)
            {
                chatActive = true;
                SetNuiFocus(true, false);

                SendNuiMessage("{\"type\":\"ON_OPEN\"}");
                await Delay(10);

                if (Game.IsControlJustPressed(0, Control.SkipCutscene))
                {
                    await Delay(10);
                    chatActive = false;
                    SetNuiFocus(false, false);
                }
            }
        }
        #endregion
    }

    public class Character
    {
        public long CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string Gender;
        public string Department;
    }

    public class Data
    {
        public bool Canceled { get; set; }
        public string Message { get; set; }
    }

    public class ChatSuggestion
    {
        public string Command { get; set; }
        public string Example { get; set; }
    }

    internal static class Extensions
    {
        internal static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.TryGetValue(key, out object value) && value is T t)
            {
                return t;
            }

            return defaultVal;
        }
    }
}