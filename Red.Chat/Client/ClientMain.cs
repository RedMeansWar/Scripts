using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpConfig;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Chat.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool chatInit, chatActive, usingFramework;
        protected string twitterName, systemName;
        protected List<Suggestion> suggestionList;
        protected Character currentCharacter;
        #endregion

        #region Constructor
        public ClientMain()
        {
            ReadConfigFile();
            RegisterNuiCallback("chatResult", new Action<IDictionary<string, object>, CallbackDelegate>(ChatResult));
        }
        #endregion

        #region Commands
        [Command("settwitter")]
        private void OnSetTwitter(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                TriggerEvent("chat:addMessage", systemName, new[] { 0, 73, 83 }, "Invalid twitter name. Usage: /settwitter [username]");
                return;
            }

            string username = args[0];

            if (username.Length > 18)
            {
                TriggerEvent("chat:addMessage", systemName, new[] { 0, 73, 83 }, "Invalid twitter name. Username must not exceed 18 characters. Usage: /settwitter [username]");
                return;
            }

            if (username.All(c => char.IsLetterOrDigit(c) || c.Equals("_")))
            {
                TriggerServerEvent("chat:addMessage", systemName, new[] { 0, 73, 83 }, "Invalid twitter name. Username must not include special characters. Usage: /settwitter [username]");
                return;
            }

            if (usingFramework)
            {
                twitterName = username;
                SetResourceKvp($"chat_twitter_{currentCharacter.CharacterId}", username);
                TriggerEvent("chat:addMessage", systemName, new[] { 0, 73, 83 }, $"Twitter username set to ^*{username}^r");
            }

            twitterName = username;
            SetResourceKvp($"chat_twitter_{Game.Player.Character.NetworkId}", username);
            TriggerEvent("chat:addMessage", systemName, new[] { 0, 73, 83 }, $"Twitter username set to ^*{username}^r");
        }
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Chat", "UsingFramework") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                usingFramework = loaded["Chat"]["UsingFramework"].BoolValue;
                systemName = loaded["Chat"]["ChatName"].StringValue;
            }
            else
            {
                Debug.WriteLine($"[Death]: Config file has not been configured correctly.");
            }
        }

        private void LoadChatSuggestion()
        {
            try
            {
                string json = LoadResourceFile(GetCurrentResourceName(), "suggestions.json");

                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.WriteLine("'suggestions.json' is null or whitespace! Won't be able to populate chat suggestions list.");
                    suggestionList = new();
                    return;
                }

                List<Suggestion> suggestions = Json.Parse<List<Suggestion>>(json);
                
                if (suggestions is null)
                {
                    Debug.WriteLine("Couldn't populate suggestion list!");
                    suggestionList = new();
                    return;
                }

                suggestionList = suggestions;
                Debug.WriteLine($"Loaded {suggestionList.Count} chat suggestion(s)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message, ex.StackTrace);
            }
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
                Vector3 playerPos = Game.PlayerPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArg = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        break;

                    case "/gooc":
                        break;

                    case "/me":
                        break;

                    case "/mer":
                        break;

                    case "/meb":
                        break;

                    case "/rt":
                        break;

                    case "/911":
                        uint streetNameAsHash = 0;
                        uint crossingRoadHash = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash, ref crossingRoadHash);
                        string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                        string crossingRoad = GetStreetNameFromHashKey(crossingRoadHash);

                        string location = $"{Exports["nearestpostal"].getClosestPostal(Game.PlayerPed.Position)}, {streetName}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad))
                        {
                            location += $" / {crossingRoad}";
                        }

                        TriggerServerEvent("chat:911Message", location, joinedArg);
                        break;

                    case "/311":
                        uint streetNameAsHash2 = 0;
                        uint crossingRoadHash2 = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash2, ref crossingRoadHash2);
                        string streetName2 = GetStreetNameFromHashKey(streetNameAsHash2);
                        string crossingRoad2 = GetStreetNameFromHashKey(crossingRoadHash2);

                        string location2 = $"{Exports["nearestpostal"].getClosestPostal(Game.PlayerPed.Position)}, {streetName2}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad2))
                        {
                            location2 += $" / {crossingRoad2}";
                        }

                        TriggerServerEvent("chat:311Message", location2, joinedArg);
                        break;

                    case "/twt":
                        break;

                    case "/twitter":
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

            if (!cancel && !string.IsNullOrWhiteSpace(message) && !usingFramework)
            {
                Vector3 playerPos = Game.PlayerPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArg = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        break;

                    case "/gooc":
                        break;

                    case "/me":
                        break;

                    case "/mer":
                        break;

                    case "/meb":
                        break;

                    case "/rt":
                        break;

                    case "/911":
                        uint streetNameAsHash = 0;
                        uint crossingRoadHash = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash, ref crossingRoadHash);
                        string streetName = GetStreetNameFromHashKey(streetNameAsHash);
                        string crossingRoad = GetStreetNameFromHashKey(crossingRoadHash);

                        string location = $"{Exports["nearestpostal"].getClosestPostal(Game.PlayerPed.Position)}, {streetName}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad))
                        {
                            location += $" / {crossingRoad}";
                        }

                        TriggerServerEvent("chat:911Message", location, joinedArg);
                        break;

                    case "/311":
                        uint streetNameAsHash2 = 0;
                        uint crossingRoadHash2 = 0;

                        GetStreetNameAtCoord(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, ref streetNameAsHash2, ref crossingRoadHash2);
                        string streetName2 = GetStreetNameFromHashKey(streetNameAsHash2);
                        string crossingRoad2 = GetStreetNameFromHashKey(crossingRoadHash2);

                        string location2 = $"{Exports["nearestpostal"].getClosestPostal(Game.PlayerPed.Position)}, {streetName2}";

                        if (!string.IsNullOrWhiteSpace(crossingRoad2))
                        {
                            location2 += $" / {crossingRoad2}";
                        }

                        TriggerServerEvent("chat:311Message", location2, joinedArg);
                        break;

                    case "/twt":
                        break;

                    case "/twitter":
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

            result(new { success = false, message = "success" });
        }
        #endregion

        #region Event Handlers
        [EventHandler("chat:addMessage")]
        private void OnAddMessage()
        {

        }

        [EventHandler("chat:addTemplate")]
        private void OnAddTemplate()
        {

        }

        [EventHandler("chat:addSuggestion")]
        private void OnAddSuggestion()
        {

        }

        [EventHandler("chat:clear")]
        private void OnClear()
        {

        }
        // Internal Events
        [EventHandler("__cfx_internal:serverPrint")]
        private void OnInternalServerPrint()
        {

        }

        [EventHandler("_chat:messageEntered")]
        private void OnInternatMessageEntered()
        {

        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ChatTick()
        {
            if (!chatInit)
            {
                chatInit = true;

                LoadChatSuggestion();
            }
        }
        #endregion
    }

    #region Extensions / Classes
    public class Suggestion
    {
        public string Command { get; set; }
        public string Description { get; set; }
    }

    public class Data
    {
        public bool Canceled { get; set; }
        public string Message { get; set; }
    }

    public class Character
    {
        public long CharacterId { get; set; }
        public long FirstName { get; set; }
        public long LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DoB { get; set; }
        public string TwitterName { get; set; }
        public string Department { get; set; }
    }

    public static class Extensions
    {
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.ContainsKey(key))
                if (dict[key] is T)
                    return (T)dict[key];
            return defaultVal;
        }
    }
    #endregion
}