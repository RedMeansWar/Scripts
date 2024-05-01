using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Chat.Client
{
    #pragma warning disable
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool hasChatInit, chatActive;
        protected string twitterUsername;
        protected Character currentCharacter;
        protected List<ChatSuggestion> suggestionsList;
        #endregion

        #region Constructor
        public ClientMain() => RegisterNuiCallback("chatResult", new Action<IDictionary<string, object>, CallbackDelegate>(OnChatResult));
        #endregion

        #region Commands
        [Command("settwitter")]
        private void OnSetTwitter(string[] args)
        {
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
            {
                TriggerEvent("_chat:messageEntered", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twitter username. Usage: /settwitter [username]");
                return;
            }

            string username = args[0];

            if (username.Length > 18)
            {
                TriggerEvent("_chat:messageEntered", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twitter username. Username must not exceed 18 characters. Usage: /settwitter [username]");
                return;
            }

            if (!username.All(c => char.IsLetterOrDigit(c) || c.Equals('_')))
            {
                TriggerEvent("_chat:messageEntered", "SYSTEM", new[] { 0, 73, 83 }, "Invalid Twitter username. Username must not include special characters. Usage: /settwitter [username]");
                return;
            }

            SetResourceKvp($"irp_chat_twitter_{currentCharacter.CharacterId}", username);
            twitterUsername = username;

            TriggerEvent("_chat:messageEntered", "SYSTEM", new[] { 0, 73, 83 }, $"Twitter username set to ^*{username}^r");
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
        [EventHandler(Events.EVENT_C_SELECTED_CHARACTER)]
        private void OnSelectedCharacter(string json)
        {
            currentCharacter = Json.Parse<Character>(json);
            twitterUsername = GetResourceKvpString($"red_chat_twitter_{currentCharacter.CharacterId}");

            if (string.IsNullOrEmpty(twitterUsername))
            {
                twitterUsername = $"{currentCharacter.FirstName}{currentCharacter.LastName}{currentCharacter.DoB:yy}";
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

        [EventHandler("_chat:messageEntered")]
        private void ChatMessage(dynamic author, dynamic colors, dynamic msg, Vector3 authorPos)
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
                Vector3 plyPos = PlayerPed.Position;

                message = message.Trim();
                string[] args = message.Split(' ');
                string firstArg = args[0].ToLower();
                string joinedArgs = string.Join(" ", args.Skip(1));

                switch (firstArg)
                {
                    case "/ooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chatNearby", $"^* ^4[OOC] {ClientPlayer.Name} (#{ClientPlayer.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 20f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/gooc":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:messageEntered", $"^*[GOOC] {ClientPlayer.Name} (#{ClientPlayer.ServerId})^r", new[] { 255, 140, 0 }, joinedArgs);
                        }
                        break;

                    case "/me":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chatNearby", $"^* ^7[ME] {currentCharacter?.FirstName} {currentCharacter?.LastName} [{currentCharacter?.Department}] (#{ClientPlayer.ServerId})^r", new[] { 255, 255, 255 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;

                    case "/mer":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chatNearby", $"^*[ME] {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{ClientPlayer.ServerId})^r", new[] { 255, 0, 0 }, joinedArgs, Players.Where(p => Vector3.Distance(p.Character.Position, plyPos) < 35f).Select(p => p.ServerId).ToList(), plyPos);
                        }
                        break;


                    case "/rt":
                        if (currentCharacter?.Department == "Civ")
                        {
                            TriggerEvent("_chat:messageEntered", "INRP", new[] { 194, 39, 39 }, "You aren't authorized to use the /rt command. You must not be a civilian character.");
                            result(new { success = false, message = "not authorized" });
                            return;
                        }

                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:radioMessage", joinedArgs);
                        }
                        break;

                    case "/twt":
                    case "/twitter":
                        if (!string.IsNullOrWhiteSpace(joinedArgs))
                        {
                            TriggerServerEvent("_chat:twitterMessage", $"@{twitterUsername}", joinedArgs);
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

                            string location = $"{Exports["nearestpostal"].getClosestPostal(PlayerPed.Position)}, {streetName}";

                            if (!string.IsNullOrWhiteSpace(crossingRoad))
                            {
                                location += $" / {crossingRoad}";
                            }

                            TriggerServerEvent("_chat:911Message", location, joinedArgs);
                        }
                        break;
                    default:
                        if (message.StartsWith("/"))
                        {
                            ExecuteCommand(message.Substring(1));
                            CancelEvent();
                        }
                        else
                        {
                            TriggerEvent("_chat:messageEntered", "", new[] { 3, 107, 252 }, $"^* {currentCharacter?.FirstName} {currentCharacter.LastName} [{currentCharacter?.Department}] (#{ClientPlayer.ServerId}): {joinedArgs}");
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
        #endregion
    }
}