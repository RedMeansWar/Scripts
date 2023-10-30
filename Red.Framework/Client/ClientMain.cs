﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Red.Framework.Client.Misc;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using Red.Common.Client.Misc;

namespace Red.Framework.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool ran;
        public static Character currentCharacter;
        protected ISet<string> allowedDepartments = new HashSet<string>();
        protected string currentAOP = "Statewide";

        protected readonly IReadOnlyList<string> scenarioTypes = new List<string>
        {
            "WORLD_VEHICLE_MILITARY_PLANES_SMALL", "WORLD_VEHICLE_MILITARY_PLANES_BIG", "WORLD_VEHICLE_AMBULANCE", "WORLD_VEHICLE_POLICE_NEXT_TO_CAR", "WORLD_VEHICLE_POLICE_CAR", "WORLD_VEHICLE_POLICE_BIKE", "WORLD_VEHICLE_DRIVE_PASSENGERS_LIMITED"
        };

        protected readonly IReadOnlyList<string> scenarioGroups = new List<string>
        {
            "MP_POLICE", "ARMY_HELI", "POLICE_POUND1", "POLICE_POUND2", "POLICE_POUND3", "POLICE_POUND4", "POLICE_POUND5", "SANDY_PLANES", "ALAMO_PLANES", "GRAPESEED_PLANES", "LSA_PLANES", "NG_PLANES"
        };

        protected readonly IReadOnlyList<string> relationshipGroups = new List<string>
        {
            "AMBIENT_GANG_HILLBILLY", "AMBIENT_GANG_BALLAS", "AMBIENT_GANG_MEXICAN", "AMBIENT_GANG_FAMILY", "AMBIENT_GANG_MARABUNTE", "AMBIENT_GANG_SALVA", "GANG_1", "GANG_2", "GANG_9", "GANG_10", "FIREMAN", "MEDIC", "COP"
        };

        protected readonly IReadOnlyList<string> suppressedModels = new List<string>
        {
            "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "polmav", "pranger", "sheriff", "sheriff2", "stockade3", "buffalo3", "fbi", "fbi2", "firetruk", "lguard", "ambulance", "riot", "shamal", "luxor", "luxor2", "jet", "lazer", "titan", "barracks", "barracks2", "crusader", "rhino", "airtug", "ripley", "cargobob", "cargobob2", "cargobob3", "cargobob4", "cargobob5", "buzzard", "besra", "volatus"
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
            SetAudioFlag("PoliceScannerDisabled", true);
            SetRandomTrains(false);
            SetRandomBoats(false);

            RegisterNuiCallback("cancelNui", new Action<IDictionary<string, object>, CallbackDelegate>(CancelNUI));
            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));
            RegisterNuiCallback("setCharacterOnline", new Action<IDictionary<string, object>, CallbackDelegate>(SetCharacterOnline));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => ShowFrameworkNUI();

        [Command("fw")]
        private void FwCommand() => ShowFrameworkNUI();

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => ShowFrameworkNUI();

        [Command("dob")]
        private void DobCommand()
        {
            if (currentCharacter is null)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, args = new[] { "SYSTEM", "You must choose a character in the framework before using this command!" } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 0, 73, 83 }, args = new[] { "SYSTEM", $"{currentCharacter.FirstName} {currentCharacter.LastName}'s date of birth is {currentCharacter.DoB:MM/dd/yyyy}" } });
            }
        }
        #endregion

        #region Methods
        private void ShowFrameworkNUI()
        {
            if (IsNuiFocused())
            {
                return;
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_UI"
            }));

            TriggerServerEvent("Framework:Server:getCharacters");
        }

        private Character CreateCharacter(string firstName, string lastName, string gender, string dob, string department, string charId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                Department = department
            };

            return createdCharacter;
        }
        #endregion

        #region Event Handlers
        [EventHandler("playerSpawned")]
        private async void OnPlayerSpawn()
        {
            if (!ran)
            {
                TriggerServerEvent("Framework:Server:getCharacters");

                Exports["spawnmanager"].spawnPlayer(true);
                await Delay(3000);
                Exports["spawnmanager"].setAutoSpawn(false);

                ran = true;
            }
        }

        [EventHandler("Framework:Client:returnCharacters")]
        private async void OnReturnCharacters(dynamic characters)
        {
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(JsonConvert.SerializeObject(characters));

            foreach (Character character in characterList)
            {
                FrameworkLog.Info($"CharacterId: {character.CharacterId}");
                FrameworkLog.Info($"FirstName: {character.CharacterId}");
                FrameworkLog.Info($"LastName: {character.CharacterId}");
                FrameworkLog.Info($"DoB: {character.CharacterId}");
                FrameworkLog.Info($"Gender: {character.CharacterId}");
                FrameworkLog.Info($"Department: { character.Department}");
            }

            FrameworkLog.Info($"Returned {characterList.Count} character(s).");

            SetNuiFocus(true, true);
            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_UI",
                characters = characterList,
                departments = allowedDepartments,
                firstLoad = currentCharacter is null,
                aop = $"Welcome to San Andreas! (AOP: {currentAOP})"
            }));
        }
        #endregion

        #region NUI Callbacks
        private async void CancelNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);
            result(new { success = true, message = "success" });
        }

        private async void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetVal<string>("characterId", null);

            TriggerServerEvent("Framework:Server:deleteCharacter", long.Parse(characterId));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character Deleted!"
            }));

            result(new { success = true, message = "success" });
        }

        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            ForceSocialClubUpdate();
            result(new { success = true, message = "success" });
        }

        private async void SetCharacterOnline(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetVal("charId", "0");
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string dob = data.GetVal<string>("dob", null);
            string dept = data.GetVal<string>("department", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender)  || string.IsNullOrWhiteSpace(dob)  || string.IsNullOrWhiteSpace(dept) || string.IsNullOrWhiteSpace(gender))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error choosing this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, dept, charId);

            currentCharacter = createdCharacter;

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);

            TriggerEvent("chat:addMessage", new { templateId = "TemplateGrey", color = new[] { 255, 255, 255 }, multiline = true, args = new[] { "", $"You're now playing as {createdCharacter.FirstName} {createdCharacter.LastName} ({createdCharacter.Department})" } });
            TriggerEvent("Framework:Client:characterSelected", Json.Stringify(currentCharacter));

            result(new { success = true, message = "success" });
        }

        private async void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);

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

            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(new
            {
                type = "SUCCESS",
                msg = $"{firstName} {lastName} ({department}) has been created!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string charId = data.GetVal("charId", "-1");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(charId))
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

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));

            result(new { success = true, message = "success" });
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task PrimaryTick()
        {
            await Delay(55000);
            foreach (Vehicle vehicle in World.GetAllVehicles().Where(v => suppressedModels.Contains(v.DisplayName.ToLower()) && !v.PreviouslyOwnedByPlayer))
            {
                vehicle.Delete();
            }
        }

        [Tick]
        private async Task SecondaryTick()
        {
            DisablePlayerVehicleRewards(Game.PlayerPed.Handle);
            SetRadarZoom(1100);
        }
        #endregion
    }
}
