using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpConfig;
using Newtonsoft.Json;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Framework.Client.Utils.Client;
using static Red.Framework.Events;
using static Red.Framework.Client.Utils.HUD;

namespace Red.Framework.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool spawnCheck, usingDiscordButtons, usingDiscord;
        protected bool displayFrameworkUI = true;
        protected string CurrentAOP = "AOP: Not Set";
        protected string discordAppId, discordPresenceLogo, actionName1, actionUrl1, actionName2, actionUrl2, actionName3, actionUrl3, frameworkName;
        protected string discordAssetText = $"{playerCount} player(s) in San Andreas!";
        protected int amountOfDiscordActions;
        protected ISet<string> allowedDepts = new HashSet<string>();
        protected Ped PlayerPed = Game.PlayerPed;
        protected static int playerCount = 1;
        private static Character CurrentCharacter;

        protected readonly IReadOnlyList<string> SuppressedModels = new List<string>()
        {
            "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "sheriff", "sheriff2", 
            "stockade3", "buffalo3", "fbi", "fbi2", "firetruk", "lguard", "ambulance", "riot", "shamal", "luxor", "luxor2", "jet", "lazer", "titan", 
            "barracks", "barracks2", "crusader", "rhino", "airtug", "ripley", "cargobob", "cargobob2", "cargobob3", "cargobob4", "cargobob5", "buzzard", 
            "besra", "volatus"
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            SetRandomTrains(false);
            SetRandomBoats(false);
            DistantCopCarSirens(false);
            StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");

            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));
            RegisterNuiCallback("setOnlineCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(SetCharacterOnline));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));

            ReadConfigFile();

            TriggerServerEvent(ServerSyncInformation);

            SetDiscordAppId(discordAppId);
            SetDiscordRichPresenceAsset(discordPresenceLogo);
            SetDiscordRichPresenceAssetText(discordAssetText);

            if (usingDiscordButtons is true)
            {
                SetDiscordRichPresenceAction(0, actionName1, actionUrl1);
                SetDiscordRichPresenceAction(1, actionName2, actionUrl2);

                switch (amountOfDiscordActions)
                {
                    case 1:
                        SetDiscordRichPresenceAction(0, actionName1, actionUrl1);
                        break;
                    case 2:
                        SetDiscordRichPresenceAction(1, actionName2, actionUrl2);
                        break;
                    case 3:
                        SetDiscordRichPresenceAction(2, actionName3, actionUrl3);
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => OpenFrameworkNUI(true);

        [Command("fw")]
        private void FwCommand() => OpenFrameworkNUI(true);

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => OpenFrameworkNUI(true);

        [Command("dob")]
        private void DobCommand() => DateOfBirthHandler();
        #endregion

        #region Methods
        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "framework.ini");

            if (Configuration.LoadFromString(data).Contains("Framework", "FrameworkName"))
            {
                Configuration loaded = Configuration.LoadFromString(data);

                usingDiscord = loaded["Framework"]["UsingDiscord"].BoolValue;
                frameworkName = loaded["Framework"]["FrameworkName"].StringValue;
                discordAppId = loaded["Discord"]["ApplicationID"].StringValue;
                discordPresenceLogo = loaded["Discord"]["ApplicationLogo"].StringValue;
                discordAssetText = loaded["Discord"]["AssestText"].StringValue;

                usingDiscordButtons = loaded["Framework"]["UsingDiscordButtons"].BoolValue;
                actionName1 = loaded["DiscordButtons"]["Button1Text"].StringValue;
                actionUrl1 = loaded["DiscordButtons"]["Button1URL"].StringValue;
                actionName2 = loaded["DiscordButtons"]["ButtonText2"].StringValue;
                actionUrl2 = loaded["DiscordButtons"]["Button2URL"].StringValue;
                actionName3 = loaded["DiscordButtons"]["ButtonText3"].StringValue;
                actionUrl3 = loaded["DiscordButtons"]["Button3URL"].StringValue;
            }
        }

        private void DateOfBirthHandler()
        {
            if (CurrentCharacter is null)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, args = new[] { "SYSTEM", "You must choose a character in the framework before using this command!" } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, args = new[] { "SYSTEM", $"{CurrentCharacter.FirstName} {CurrentCharacter.LastName}'s date of birth is {CurrentCharacter.DoB:MM/dd/yyyy}" } });
            }
        }

        private void AddDepartmentIfNotExists(string department)
        {
            if (!allowedDepts.Contains(department))
            {
                allowedDepts.Add(department);
            }
        }

        private async void OpenFrameworkNUI(bool visable = false)
        {
            displayFrameworkUI = visable;

            if (displayFrameworkUI is true)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "DISPLAY_UI"
                }));

                SetNuiFocus(true, true);
                DisableMovementThisFrame(true, true);
                DisableAttackControlsThisFrame();

                await Delay(2000);
                Log.Info("Invoking framework ui");
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);
            DisableMovementThisFrame(false, false);

            Log.Info("Revoking framework nui");
        }

        private Character CreateCharacter(string firstName, string lastName, string gender, string dob, string department, string characterId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(characterId),
                DoB = DateTime.Parse(dob),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Department = department
            };

            return createdCharacter;
        }
        #endregion

        #region NUI Callbacks
        private async void CloseUI(IDictionary<string, object> data, CallbackDelegate result) => OpenFrameworkNUI();
        private async void DisconnectUser(IDictionary<string, object> data, CallbackDelegate result) => TriggerServerEvent(ServerDropUserFromFramework, "Disconnected via framework");
        
        private async void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            ForceSocialClubUpdate();
            result(new { success = true, message = "success" });
        }

        private async void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string department = data.GetValue<string>("department", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error creating this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }


            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, department, "0");

            TriggerServerEvent(ServerCreateCharacter, Json.Stringify(createdCharacter));
            
            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = $"{firstName} {lastName} ({department}) has been created!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetValue<string>("characterId", null);

            TriggerServerEvent(ServerDeleteCharacter, long.Parse(characterId));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character deleted!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string department = data.GetValue<string>("department", null);
            string charId = data.GetValue("charId", "-1");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error editing this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character editedCharacter = new()
            {
                CharacterId = long.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                Department = department
            };

            TriggerServerEvent(ServerEditCharacter, Json.Stringify(editedCharacter));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void SetCharacterOnline(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetValue("charId", "0");
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string department = data.GetValue<string>("department", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error choosing this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, department, charId);
            CurrentCharacter = createdCharacter;

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);

            TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"You're now playing as {createdCharacter.FirstName} {createdCharacter.LastName} ({createdCharacter.Department})" } });
            TriggerEvent(ClientSelectedCharacter, Json.Stringify(CurrentCharacter));

            SetRichPresence($"{(CurrentCharacter.Department == "Civ" ? "Exploring" : "Patrolling")} {(Game.PlayerPed.Position.Y > 1000f ? "Blaine County" : "Los Santos")} as {CurrentCharacter.FirstName} {CurrentCharacter.LastName} ({CurrentCharacter.Department})");
            result(new { success = true, message = "success" });
        }
        #endregion

        #region Event Handlers
        [EventHandler(ClientReturnCharacters)]
        private void OnReturnCharacters(dynamic characters)
        {
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(JsonConvert.SerializeObject(characters));

            foreach (Character character in characterList)
            {
                Log.Info($"CharacterId: {character.CharacterId}");
                Log.Info($"FirstName: {character.FirstName}");
                Log.Info($"LastName: {character.LastName}");
                Log.Info($"DoB: {character.DoB}");
                Log.Info($"Gender: {character.Gender}");
                Log.Info($"Department: {character.Department}");
            }

            Log.Info($"Returned {characterList.Count} character(s).");

            SetNuiFocus(true, true);

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_UI",
                characters = characterList,
                departments = allowedDepts,
                firstLoad = CurrentCharacter is null,
                aop = $"AOP: {CurrentAOP}"
            }));
        }

        [EventHandler(PlayerSpawned)]
        private async void OnPlayerSpawn()
        {
            if (!spawnCheck)
            {
                OpenFrameworkNUI(true);

                Exports["spawnmanager"].spawnPlayer(true);
                await Delay(3000);
                Exports["spawnmanager"].setAutoSpawn(false);

                spawnCheck = true;
            }
        }

        [EventHandler(ClientUpdateAreaOfPatrol)]
        private void OnUpdateAOP(string newAOP, string setter)
        {
            CurrentAOP = newAOP;
            TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"AOP set to ^5^*{newAOP}^r^7 by ^5^*{setter}^r^7" } });

            SendNuiMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = newAOP.Replace("\"", "").Trim(),
            }));
        }

        [EventHandler(ClientAreaOfPatrolErrorNotify)]
        private void OnErrorAOP()
        {
            ErrorNotification("Only Staff+ can use this command!");
        }
        #endregion
    }
}