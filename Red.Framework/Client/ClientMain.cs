using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Red.Common;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.NUI;
using static Red.Common.Client.Extensions;

namespace Red.Framework.Client
{
    #pragma warning disable
    internal class ClientMain : BaseScript
    {
        #region Variables
        protected bool ran, teleported;
        protected string aopSetter = "SYSTEM";
        protected string currentAOP = "Statewide";
        protected float densityMultiplier = 1f;
        protected int plyrCount = 1;
        protected Vector3 spawnLocation;
        protected ISet<string> allowedDepartments = new HashSet<string>();
        public static Character currentCharacter;
        #endregion

        #region Constructor
        public ClientMain()
        {
            TriggerServerEvent("Framework:Server:getDiscordRoles");

            #region Register NUI Callbacks
            RegisterNUICallback("selectCharacter", SelectCharacter);
            RegisterNUICallback("createCharacter", CreateCharacter);
            RegisterNUICallback("deleteCharacter", DeleteCharacter);
            RegisterNUICallback("editCharacter", EditCharacter);
            RegisterNUICallback("closeNUI", CloseNUI);
            RegisterNUICallback("spawnAtLocation", SpawnAtLocation);
            RegisterNUICallback("doNotTeleport", DoNotTeleport);
            #endregion

            Tick += ChatTemplateTick;
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => DisplayNUI();

        [Command("fw")]
        private void FwCommand() => DisplayNUI();

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => DisplayNUI();

        [Command("selectcharacter")]
        private void SelectCharacterCommand() => DisplayNUI();

        [Command("dob")]
        private void DobCommand()
        {
            if (currentCharacter is null)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "INRP", "You must choose a character in the framework before using this command!" } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "INRP", $"{currentCharacter.FirstName} {currentCharacter.LastName}'s date of birth is {currentCharacter.DoB:MM/dd/yyyy}" } });
            }
        }
        #endregion

        #region NUI Callbacks
        /// <summary>
        /// Handles character selection for the framework.
        /// </summary>
        /// <param name="data">A dictionary containing character data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private async void SelectCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetVal("charId", "0");
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string cash = data.GetVal("cash", "-1");
            string bank = data.GetVal("bank", "-1");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(dob) || cash == "-1" || bank == "-1")
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error choosing this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, charId);
            currentCharacter = createdCharacter;
            TriggerEvent("Framework:Client:characterSelected", Json.Stringify(currentCharacter));


            // Send spawn information to client based on character's department
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "civ"
                }));
            }
            else if (currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "SAHP")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "police"
                }));
            }
            else if (currentCharacter.Department == "SAFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "fire"
                }));
            }

            SetRichPresence($"{(currentCharacter.Department == "Civ" ? "Exploring" : "Patrolling")} {(PlayerPed.Position.Y > 1000f ? "Blaine County" : "Los Santos")} as {currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})");
            result(new { success = true, message = "success" });
        }
        /// <summary>
        /// Handles creating a character based on provided HTML and JavaScript data.
        /// </summary>
        /// <param name="data">A dictionary containing character data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            // Extract character information from the data dictionary with default values
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string cash = data.GetVal("cash", "-1");
            string bank = data.GetVal("bank", "-1");

            // Validate character data
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(dob) || cash == "-1" || bank == "-1")
            {
                Log.FrameworkInfo("An attempt was made trying to create a character but couldn't because there wasn't valid character input data.");

                // Display error modal to client and return failure result
                SendNUIMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error creating this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            // Create the character object
            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, "0");

            // Trigger server event to handle server-sided character creation.
            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(createdCharacter));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = $"{firstName} {lastName} ({department}) has been created!"
            }));
            result(new { success = true, message = "success" });
        }

        /// <summary>
        /// Handles deleting a character in the framework.
        /// </summary>
        /// <param name="data">A dictionary containing character data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            Log.FrameworkInfo("Successfully deleted character.");

            // Extract character information from the data dictionary with default values
            string characterId = data.GetVal<string>("characterId", null);

            // Trigger server event to handle server-sided character deletion.
            TriggerServerEvent("Framework:Server:deleteCharacter", long.Parse(characterId));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character deleted!"
            }));
            result(new { success = true, message = "success" });
        }

        /// <summary>
        /// Handles editing a character in the framework.
        /// </summary>
        /// <param name="data">A dictionary containing character data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            // Extract character information from the data dictionary with default values
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string charId = data.GetVal("charId", "-1");

            // Validate character data
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(dob))
            {
                Log.FrameworkInfo($"Attempted to edit character: {charId} but couldn't because of invalid character data.");

                // Display error modal to client and return failure result
                SendNUIMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error editing this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            // Create the character object
            Character editedCharacter = new()
            {
                CharacterId = long.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                Department = department
            };

            // Trigger server event to handle server-sided character edit.
            TriggerServerEvent("Framework:Server:editCharacter", Json.Stringify(editedCharacter));

            // Display success modal to client and return success result
            SendNUIMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));
            result(new { success = true, message = "success" });
        }

        /// <summary>
        /// Handles the "Quit Game" button on the UI.
        /// </summary>
        /// <param name="data">A dictionary containing data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            // Force the game to exit and return success result
            ForceSocialClubUpdate();
            result(new { success = true, message = "success" });
        }

        /// <summary>
        /// Hands the "Close" button on the UI.
        /// </summary>
        /// <param name="data">A dictionary containing data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private void CloseNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNUIFocus(false, false);
            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            result(new { success = true, message = "success" });
        }

        /// <summary>
        /// Handles the "Do Not Teleport" button in the spawn modal.
        /// </summary>
        /// <param name="data">A dictionary containing data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private async void DoNotTeleport(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNUIFocus(false, false);
            SendNUIMessage(Json.Stringify(new
            {
                type = "DO_NOT_TELEPORT"
            }));

            result(new { success = true, message = "success" });

            await Delay(100);
            TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"You're now playing as {currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})" } });
        }

        /// <summary>
        /// Handlers the Spawn At Location buttons in the spawn modal.
        /// </summary>
        /// <param name="data">A dictionary containing data.</param>
        /// <param name="result">A callback delegate to handle the selection result.</param>
        private async void SpawnAtLocation(IDictionary<string, object> data, CallbackDelegate result)
        {
            string x = data.GetVal("xPos", "0.0");
            string y = data.GetVal("yPos", "0.0");
            string z = data.GetVal("zPos", "0.0");
            string h = data.GetVal("hPos", "0.0");

            float locX = float.Parse(x);
            float locY = float.Parse(y);
            float locZ = float.Parse(z);
            float heading = float.Parse(h);

            SetNUIFocus(false, false);
            SendNUIMessage(Json.Stringify(new { type = "CLOSE_NUI" }));

            await Delay(1500);

            SpawnAt(new(locX, locY, locZ), heading);

            result(new { success = true, message = "success" });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a character that can be used in the framework.
        /// </summary>
        /// <param name="firstName">Character's First Name</param>
        /// <param name="lastName">Character's Last Name</param>
        /// <param name="gender">Character's Gender</param>
        /// <param name="department">Character's Department</param>
        /// <param name="dob">Character's Date of Birth</param>
        /// <param name="cash">Character's Starting Cash Amount</param>
        /// <param name="bank">Character's Starting Bank Amount</param>
        /// <param name="charId">Character's Unique Id</param>
        /// <returns>A character that is newly created</returns>
        private Character CreateCharacter(string firstName, string lastName, string gender, string department, string dob, string cash, string bank, string charId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(charId),
                DoB = DateTime.Parse(dob),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                Department = department,
                Cash = float.Parse(cash),
                Bank = float.Parse(bank)
            };

            return createdCharacter;
        }

        /// <summary>
        /// Handles spawning the player character at a specified location with a given heading.
        /// </summary>
        /// <param name="location">The position to spawn the character at.</param>
        /// <param name="spawnHeading">The heading to set for the character.</param>
        private async void SpawnAt(Vector3 location, float spawnHeading)
        {
            Game.Player.Character.IsInvincible = true;
            Game.Player.Character.IsCollisionEnabled = false;
            Game.Player.Character.HasGravity = false;

            SwitchOutPlayer(PlayerPedId(), 0, 1);

            await Delay(3000);

            Game.Player.Character.Position = location;

            Function.Call((Hash)0xD8295AF639FD9CB8, PlayerPedId());

            await Delay(3000);

            Game.Player.Character.HasGravity = true;
            Game.Player.Character.IsCollisionEnabled = true;
            Game.Player.Character.IsInvincible = false;

            TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"You're now playing as {currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})" } });
        }

        private void DisplayNUI()
        {
            if (IsNUIFocused())
            {
                return;
            }

            TriggerServerEvent("Framework:Server:getCharacters");
        }

        private void CloseSpawnNUI()
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "HIDE_SPAWN_MODALS"
            }));
        }

        /// <summary>
        /// Adds a department to the list of allowed departments if it doesn't already exist.
        /// </summary>
        /// <param name="department">The name of the department to potentially add.</param>
        private void AddDeptIfNotExists(string department)
        {
            // Check if the department is already in the allowed list
            if (!allowedDepartments.Contains(department))
            {
                // Add the department to the allowed list
                allowedDepartments.Add(department);
            }
        }

        /// <summary>
        /// Checks if the player has a high-level administrative Discord role.
        /// </summary>
        /// <param name="roles">A dictionary containing Discord role data.</param>
        /// <returns>True if the player has a high-level admin role, False otherwise.</returns>
        private bool HasDevOrSeniorAdmin(Dictionary<string, string> roles)
        {
            return roles.ContainsValue("Development") || roles.ContainsValue("Senior Administration");
        }

        /// <summary>
        /// Adds a department to the allowed list if it doesn't already exist, with a condition based on a Discord role.
        /// </summary>
        /// <param name="roles">A dictionary containing Discord role data.</param>
        /// <param name="deptName">The name of the department to potentially add.</param>
        private void AddDeptIfRoleIsPresent(Dictionary<string, string> roles, string departmentName)
        {
            if (roles.ContainsValue(departmentName))
            {
                AddDeptIfNotExists(departmentName);
            }
        }

        /// <summary>
        /// Adds a department to the allowed list if it doesn't already exist, with a condition based on Civilian role terms.
        /// </summary>
        /// <param name="roles">A dictionary containing Discord role data.</param>
        private void AddDeptIfNotExistIfCivilian(Dictionary<string, string> roles)
        {
            if (roles.ContainsValue("CIV") || roles.ContainsValue("Civ") || roles.ContainsValue("Civilian"))
            {
                AddDeptIfNotExists("CIV");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:returnDiscordRoles")]
        private void OnReturnDiscords(dynamic rolesJson)
        {
            // Deserialize Discord role data from JSON string
            Dictionary<string, string> roles = JsonConvert.DeserializeObject<Dictionary<string, string>>(rolesJson);

            // Update allowed departments based on Discord roles
            if (HasDevOrSeniorAdmin(roles)) // Check for high-level admin or development roles
            {
                AddDeptIfNotExists("LSPD");
                AddDeptIfNotExists("SAHP");
                AddDeptIfNotExists("BCSO");
                AddDeptIfNotExists("SAFD");
                AddDeptIfNotExists("CIV");
            }

            // Grant access to specific departments based on individual Discord roles
            AddDeptIfRoleIsPresent(roles, "LSPD");
            AddDeptIfRoleIsPresent(roles, "SAHP");
            AddDeptIfRoleIsPresent(roles, "BCSO");
            AddDeptIfRoleIsPresent(roles, "SAFD");

            // Grant access to Civilian department based on various Civilian role terms
            AddDeptIfNotExistIfCivilian(roles);
        }

        [EventHandler("Framework:Client:returnCharacters")]
        private void OnReturnCharacters(dynamic characters)
        {
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(JsonConvert.SerializeObject(characters));
            Log.FrameworkInfo($"Returned {characterList.Count} character(s).");

            SetNUIFocus(true, true);
            SendNUIMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI",
                characters = characterList,
                departments = allowedDepartments,
                firstLoad = currentCharacter is null,
                aop = $"AOP: {currentAOP}"
            }));
        }

        [EventHandler("playerSpawned")]
        private async void OnPlayerSpawned()
        {
            TriggerServerEvent("Framework:Server:getCharacters");

            Exports["spawnmanager"].spawnPlayer(true);
            await Delay(3000);
            Exports["spawnmanager"].setAutoSpawn(false);
        }

        [EventHandler("Framework:Client:changeAOP")]
        private void OnChangeAOP(string newAop)
        {
            currentAOP = newAop;

            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"AOP: {newAop}"
            }));
        }

        [EventHandler(Events.EVENT_C_SEND_FRAMEWORK_MESSAGE)]
        private void OnSendFrameworkMessage(string message) => TriggerEvent("chat:addTemplate", "TemplateGrey", "<div style='background-color: rgba(34, 34, 34, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");

        [EventHandler(Events.EVENT_C_SEND_GREEN_FRAMEWORK_MESSAGE)]
        private void OnSendGreenFrameworkMessage(string message) => TriggerEvent("chat:addMessage", new { templateId = "TemplateGreen", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"{message}" } });

        [EventHandler(Events.EVENT_C_SEND_RED_FRAMEWORK_MESSAGE)]
        private void OnSendRedFrameworkMessage(string message) => TriggerEvent("chat:addMessage", new { templateId = "TemplateRed", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"{message}" } });

        [EventHandler(Events.EVENT_C_SEND_BLUE_FRAMEWORK_MESSAGE)]
        private void OnSendBlueFrameworkMessage(string message) => TriggerEvent("chat:addMessage", new { templateId = "TemplateBlue", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"{message}" } });
        #endregion

        #region Ticks
        [Tick]
        private async Task FrameworkTick()
        {
            await Delay(55000);
            densityMultiplier = plyrCount < 31 ? 1.0f : plyrCount is > 32 and < 50 ? 0.8f : plyrCount >= 50 ? 0.6f : 1.0f;
        }

        private async Task ChatTemplateTick()
        {
            if (!ran)
            {
                TriggerEvent("chat:addTemplate", "TemplateGreen", "<div style='background-color: rgba(0, 153, 0, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateGrey", "<div style='background-color: rgba(34, 34, 34, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateRed", "<div style='background-color: rgba(255, 0, 0, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");
                TriggerEvent("chat:addTemplate", "TemplateBlue", "<div style='background-color: rgba(0, 128, 255, 0.4); padding-top: 10px; padding-bottom: 10px; border-radius: 10px; text-align: center;'>{1}</div");

                ran = true;
            }

            await Delay(10);
        }
        #endregion
    }
}