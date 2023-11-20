using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.Chat.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool hasChatInit, chatActive;
        protected string twitterName, darkWebName, communityName;
        protected Character currentCharacter;
        protected List<ChatSuggestion> suggestionsList;
        #endregion

        #region Constructor
        public ClientMain()
        {
            RegisterNuiCallback("chatResult", new Action<IDictionary<string, object>, CallbackDelegate>(ChatResult));
            ReadConfigFile();
        }
        #endregion

        #region Commands
        [Command("settwitter")]
        private void SetTwitterCommand(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                TriggerEvent("chat:addMessage", communityName, new[] { 0, 73, 84 }, "Invalid Twitter username. Usage: /settwitter [username]");
            }

            string username = args[0];

            if (args.Length > 18)
            {
                TriggerEvent("chat:addMessage", communityName, new[] { 0, 73, 83 }, "Invalid Twitter username. Username must not exceed 18 characters. Usage: /settwitter [username]");
                return;
            }

            if (!username.All(c => char.IsLetterOrDigit(c) || c.Equals('_')))
            {
                TriggerEvent("chat:addMessage", communityName, new[] { 0, 73, 83 }, "Invalid Twitter username. Username must not include special characters. Usage: /settwitter [username]");
                return;
            }

            SetResourceKvp($"red_chat_twitter_{currentCharacter.CharacterId}", username);
            twitterName = username;

            TriggerEvent("chat:addMessage", communityName, new[] { 0, 73, 83 }, $"Twitter username set to ^*{username}^r");
        }
        #endregion

        #region NUI Callbacks
        private void ChatResult(IDictionary<string, object> data, CallbackDelegate result)
        {
            chatActive = false;
            SetNuiFocus(false, false);
            string message = data.GetVal<string>("message", null);
            bool cancel = data.GetVal("canceled", false);

            if (!cancel && !string.IsNullOrWhiteSpace(message))
            {
                Vector3 plyrPos = Game.PlayerPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArgs = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^4[OOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyrPos) < 20f).Select(p => p.ServerId).ToList(), plyrPos);
                        }
                        break;

                    case "/gooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:messageEntered", $"^* ^4[OOC] {Game.Player.Name} (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyrPos) < 20f).Select(p => p.ServerId).ToList(), plyrPos);
                        }
                        break;

                    case "/me":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^* ^2[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyrPos) < 20f).Select(p => p.ServerId).ToList(), plyrPos);
                        }
                        break;

                    case "/mer":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^*[ME] {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 255, 0, 0 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyrPos) < 20f).Select(p => p.ServerId).ToList(), plyrPos);
                        }
                        break;

                    case "/meb":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("Chat:Server:chatNearby", $"^*[ME] {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{Game.Player.ServerId})^r", new[] { 0, 100, 255 }, joinedArgs, Players.Where(p => Vector3.DistanceSquared(p.Character.Position, plyrPos) < 20f).Select(p => p.ServerId).ToList(), plyrPos);
                        }
                        break;

                    case "/rt":
                        if (currentCharacter?.Department == "Civ")
                        {
                            TriggerEvent("chat:addMessage", "PNW", new[] { 194, 39, 39 }, "You aren't authorized to use the /rt command. You must not be a civilian character.");
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
                            TriggerServerEvent("Chat:Server:twitterMessage", $"@{twitterName}", joinedArgs);
                        }
                        break;

                    case "/911":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            uint streetHash = 0;
                            uint crossingRoadHash = 0;

                            GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetHash, ref crossingRoadHash);
                            string streetName = GetStreetNameFromHashKey(streetHash);
                            string crossingRoad = GetStreetNameFromHashKey(crossingRoadHash);

                            string location = $"{Exports["nearestpostal"].getClosestPostal(Game.PlayerPed.Position)}, {streetName}";

                            if (string.IsNullOrWhiteSpace(crossingRoad))
                            {
                                location += $" / {crossingRoad}";
                            }

                            TriggerServerEvent("Chat:Server:911Message", location, joinedArgs);
                        }
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

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Chat", "CommunityName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);

                communityName = loaded["Chat"]["CommunityName"].StringValue; 
            }
            else
            {
                Debug.WriteLine($"[Chat]: Config file has not been configured correctly.");
            }
        }

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
        private void CharacterSelected(string json)
        {
            currentCharacter = Json.Parse<Character>(json);
            twitterName = GetResourceKvpString($"red_chat_twitter_{currentCharacter.CharacterId}");
            darkWebName = GetResourceKvpString($"red_chat_darkweb_{currentCharacter.CharacterId}");

            if (string.IsNullOrEmpty(twitterName))
            {
                twitterName = $"{currentCharacter.FirstName}{currentCharacter.LastName}{currentCharacter.DoB:yy}";
            }

            if (string.IsNullOrEmpty(darkWebName))
            {
                darkWebName = $"{currentCharacter.FirstName}{currentCharacter.LastName}{currentCharacter.DoB:yy}";
            }
        }

        [EventHandler("chat:addChatMessage")]
        private void OnChatAddMessage(dynamic msg)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "ON_MESSAGE",
                message = msg
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

        [EventHandler("chat:addMessage")]
        private void OnChatMessage(dynamic author, dynamic colors, dynamic msg, Vector3 authorPos)
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
            
            if (!hasChatInit && Game.IsControlJustPressed(0, Control.MpTextChatAll) && UpdateOnscreenKeyboard() != 0)
            {
                chatActive = true;
                SetNuiFocus(true, false);

                SendNuiMessage("{\"type\":\"ON_OPEN\"}");
                await Delay(10);
            }
        }
        #endregion
    }

    #region Classes
    public class Data
    {
        public bool Cancel { get; set; }
        public string Message { get; set; }
    }

    public class ChatSuggestion
    {
        public string Command { get; set; }
        public string Example { get; set; }
    }
    #endregion

    #region Extensions
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
    #endregion
}