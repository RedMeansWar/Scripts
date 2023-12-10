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
        protected bool hasChatInit, chatActive, toggleTwotter, toggleDarkWeb;
        protected string twotterUsername, darkWebUsername;
        protected Character currentCharacter;
        protected List<ChatSuggestion> suggestionsList;
        #endregion

        #region Constructor
        public ClientMain() => RegisterNuiCallback("chatResult", new Action<IDictionary<string, object>, CallbackDelegate>(OnChatResult));
        #endregion

        #region Commands
        [Command("settwotter")]
        private void OnSetTwotter(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twotter username. Usage: /settwotter [username]");
                return;
            }

            string username = args[0];

            if (username.Length > 18)
            {
                TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twotter username. Username must not exceed 18 characters. Usage: /settwotter [username]");
                return;
            }

            if (!username.All(c => char.IsLetterOrDigit(c) || c.Equals('_')))
            {
                TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twotter username. Username must not include special characters. Usage: /settwotter [username]");
                return;
            }

            SetResourceKvp($"is_chat_twotter_{currentCharacter.CharacterId}", username);
            twotterUsername = username;

            TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 0, 73, 83 }, $"Twotter username set to ^*{username}^r");
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
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:characterSelected")]
        private void OnCharacterSelected(string json)
        {
            currentCharacter = Json.Parse<Character>(json);
            twotterUsername = GetResourceKvpString($"red_chat_twotter_{currentCharacter.CharacterId}");

            if (string.IsNullOrEmpty(twotterUsername))
            {
                twotterUsername = $"{currentCharacter.FirstName}{currentCharacter.LastName}{currentCharacter.DoB:yy}";
            }
        }

        [EventHandler("chat:addMessage")]
        private void AddChatMsg(dynamic msg)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = msg
            }));
        }

        [EventHandler("chat:addTemplate")]
        private void AddChatTemplate(string id, string template)
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

        [EventHandler("_chat:chatMessage")]
        private void ChatMessage(dynamic author, dynamic colors, dynamic msg, Vector3 authorPos)
        {
            string distanceString = "";
            if (!authorPos.IsZero)
            {
                distanceString = $" ({Math.Round(Vector3.DistanceSquared(authorPos, Game.PlayerPed.Position))}m)";
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                template = author == "" ? "defaultAlt" : "default",
                message = new
                {
                    color = new[] { colors[0], colors[1], colors[2] },
                    multiline = true,
                    args = author == "" ? new[] { $"{msg}" } : new[] { $"{author}{distanceString}", $"{msg}" }
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

        [EventHandler("chat:twotterMessage")]
        private void OnTwotterMessage(string name, string message)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = new
                {
                    color = new[] { 0, 172, 238 },
                    multiline = true,
                    args = new[] { $"[Twotter] {name}", message }
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
                    color = new[] { 209, 153, 0 },
                    multiline = true,
                    args = new[] { $"[311] {name}", message }
                }
            }));
        }
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
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^4[OOC]  {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
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
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^7[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/mer":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^1[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 0, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/meb":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^5[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 0, 100, 255 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
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
                            TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 194, 39, 39 }, "You aren't authorized to use the /rt command. You must not be a civilian character.");
                            result(new { success = false, message = "not authorized" });
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:radioMessage", joinedArgs);
                        }
                        break;

                    case "/twt":
                    case "/twotter":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:twotterMessage", $"@{twotterUsername}", joinedArgs);
                        }
                        break;
                    case "/911":
                        uint streetNameAsHash = 0;
                        uint crossingRoadAsHash = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash, ref crossingRoadAsHash);
                        string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                        string crossingRoad = GetStreetNameFromHashKey(crossingRoadAsHash);

                        string location = $"{Exports["postals"].GetClosestPostal(Game.PlayerPed.Position)}, {streetName}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad))
                        {
                            location += $" / {crossingRoad}";
                        }

                        TriggerServerEvent("Chat:Server:911Message", location, joinedArgs);
                        break;
                    case "/311":
                        uint streetNameAsHash311 = 0;
                        uint crossingRoadAsHash311 = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash311, ref crossingRoadAsHash311);
                        string streetName311 = GetStreetNameFromHashKey(streetNameAsHash311);
                        string crossingRoad311 = GetStreetNameFromHashKey(crossingRoadAsHash311);

                        string location311 = $"{Exports["postals"].GetClosestPostal(Game.PlayerPed.Position)}, {streetName311}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad311))
                        {
                            location311 += $" / {crossingRoad311}";
                        }
                        TriggerServerEvent("Chat:Server:311Message", location311, joinedArgs);
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

            result(new { success = true, message = "success" });
        }
        #endregion

        #region Classes
        public class ChatSuggestion
        {
            public string Command { get; set; }
            public string Example { get; set; }
        }

        public class Data
        {
            public bool Canceled { get; set; }
            public string Message { get; set; }
        }

        public class Character
        {
            public long CharacterId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DoB { get; set; }
            public string Gender { get; set; }
            public int Cash { get; set; }
            public int Bank { get; set; }
            public string Department { get; set; }
        }
        #endregion
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