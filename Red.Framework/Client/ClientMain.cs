using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Red.Common.Client.Misc;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Hud.HUD;

namespace Red.Framework.Client
{
    internal class ClientMain : BaseScript
    {
        #region Variables
        protected bool ranSpawnChecker;
        protected float densityMultiplier = 1f;
        protected string currentAOP = "Statewide";
        protected Ped PlayerPed = Game.PlayerPed;
        protected ISet<string> allowedDepartments = new HashSet<string>();
        protected int plyrCount = 1;

        public static Character currentCharacter;

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
            SetWeaponsNoAutoreload(true);
            SetWeaponsNoAutoswap(true);
            NetworkSetFriendlyFireOption(true);
            SetPlayerHealthRechargeMultiplier(PlayerPed.Handle, 0f);
            SetWeaponDamageModifier((uint)WeaponHash.Nightstick, 0.1f);
            SetWeaponDamageModifier((uint)WeaponHash.Unarmed, 0.1f);
            SetFlashLightKeepOnWhileMoving(true);

            SetRandomBoats(false);
            SetRandomTrains(false);

            StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
            SetAudioFlag("PoliceScannerDisabled", true);

            TriggerServerEvent("Framework:Server:syncInfo");
            TriggerServerEvent("Framework:Server:getDiscordRoles");

            // NUI Callbacks
            RegisterNuiCallback("selectCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(SelectCharacter));
            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));
            RegisterNuiCallback("disconnect", new Action<IDictionary<string, object>, CallbackDelegate>(Disconnect));
            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));
            RegisterNuiCallback("cancelNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CancelNUI));

            uint PLAYER = Game.GenerateHashASCII("PLAYER");

            foreach (string scenarioType in scenarioTypes)
            {
                SetScenarioTypeEnabled(scenarioType, false);
            }

            foreach (string scenarioGroup in scenarioGroups)
            {
                SetScenarioGroupEnabled(scenarioGroup, false);
            }

            foreach (string relationshipGroup in relationshipGroups)
            {
                SetRelationshipBetweenGroups(1, Game.GenerateHashASCII(relationshipGroup), (uint)PLAYER);
            }

            foreach (string suppressedModel in suppressedModels)
            {
                SetVehicleModelIsSuppressed(Game.GenerateHashASCII(suppressedModel), true);
            }

            for (int i = 0; i < 15; i++)
            {
                EnableDispatchService(i, false);
            }
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => DisplayNUI();

        [Command("fw")]
        private void FwCommand() => DisplayNUI();

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => DisplayNUI();

        [Command("dob")]
        private void CharacterDoBCommand()
        {
            if (currentCharacter is null)
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "SYSTEM", "You must choose a character in the framework before using this command!" } });
            }
            else
            {
                TriggerEvent("chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { "SYSTEM", $"{currentCharacter.FirstName} {currentCharacter.LastName}'s date of birth is {currentCharacter.DoB:MM/dd/yyyy}" } });
            }
        }
        #endregion

        #region NUI Callbacks
        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            ForceSocialClubUpdate();
            result(new { success = true, message = "success" });
        }

        private void Disconnect(IDictionary<string, object> data, CallbackDelegate result)
        {
            TriggerServerEvent("Framework:DropUser");
            result(new { success = true, message = "success" });
        }

        public void CancelNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);
            Info("[Framework]: Revoking Nui Callback");

            result(new { success = true, message = "success" });
        }

        private void SelectCharacter(IDictionary<string, object> data, CallbackDelegate result)
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

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            Info($"[Framework]: Selected Character [{currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})]");
            SetNuiFocus(false, false);

            TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 255, 255 }, $"You are now playing as {currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})");
            TriggerEvent("Framework:Client:characterSelected", Json.Stringify(createdCharacter));
        }

        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
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
                    msg = "We ran into an unexpected error creating this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, "0");

            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(createdCharacter));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = $"{firstName} {lastName} ({department}) has been created!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetVal<string>("characterId", null);

            TriggerServerEvent("Framework:Server:deleteCharacter", long.Parse(characterId));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character deleted!"
            }));

            result(new { success = true, message = "success" });
        }

        private void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string department = data.GetVal<string>("department", null);
            string dob = data.GetVal<string>("dob", null);
            string charId = data.GetVal("charId", "-1");

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

            TriggerServerEvent("Framework:Server:editCharacter", Json.Stringify(editedCharacter));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));

            result(new { success = true, message = "success" });
        }
        #endregion

        #region Methods
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
                Cash = int.Parse(cash),
                Bank = int.Parse(bank)
            };

            return createdCharacter;
        }

        private void DisplayNUI()
        {
            if (IsNuiFocused())
            {
                return;
            }

            TriggerServerEvent("Framework:Server:getCharacters");
        }

        private void AddIfNotExist(string department)
        {
            if (!allowedDepartments.Contains(department))
            {
                allowedDepartments.Add(department);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:changeAOP")]
        private void OnChangeAOP(string newAOP)
        {
            currentAOP = newAOP;
            TriggerEvent("_chat:chatMessage", "SYSTEM", new[] { 255, 255, 255 }, $"Current AOP is ^5^*{currentAOP}^r^7");

            SendNuiMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"Welcome to San Andreas! (AOP: {newAOP})"
            }));
        }

        [EventHandler("Framework:Client:syncInfo")]
        private void OnSyncInfo(string aop) => currentAOP = aop;

        [EventHandler("Framework:Client:returnDiscordRoles")]
        private void OnReturnDiscordRoles(dynamic rolesJson)
        {
            Dictionary<string, string> roles = JsonConvert.DeserializeObject<Dictionary<string, string>>(rolesJson);

            if (roles.ContainsValue("Development") || roles.ContainsValue("Head Administration"))
            {
                AddIfNotExist("LSPD");
                AddIfNotExist("SAHP");
                AddIfNotExist("BCSO");
                AddIfNotExist("LSFD");
                AddIfNotExist("CIV");
            }

            if (roles.ContainsValue("LSPD"))
            {
                AddIfNotExist("LSPD");
            }

            if (roles.ContainsValue("SAHP"))
            {
                AddIfNotExist("SAHP");
            }

            if (roles.ContainsValue("BCSO"))
            {
                AddIfNotExist("BCSO");
            }

            if (roles.ContainsValue("LSFD"))
            {
                AddIfNotExist("LSFD");
            }

            if (roles.ContainsValue("CIV"))
            {
                AddIfNotExist("CIV");
            }
        }

        [EventHandler("Framework:Client:returnCharacters")]
        private void OnReturnCharacters(dynamic characters)
        {
            List<Character> characterList = JsonConvert.DeserializeObject<List<Character>>(JsonConvert.SerializeObject(characters));

            /*
            foreach (Character character in characterList)
            {
                Info($"CharacterId: {character.CharacterId}");
                Info($"FirstName: {character.FirstName}");
                Info($"LastName: {character.LastName}");
                Info($"DoB: {character.DoB}");
                Info($"Gender: {character.Gender}");
                Info($"Cash: {character.Cash}");
                Info($"Bank: {character.Bank}");
                Info($"Department: {character.Department}");
            }
            */
            Info($"[Framework] - Returned {characterList.Count} character(s)");

            SetNuiFocus(true, true);

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI",
                characters = characterList,
                departments = allowedDepartments,
                firstLoad = currentCharacter is null,
                aop = $"Welcome to San Andreas! (AOP: {currentAOP})"
            }));
        }

        [EventHandler("playerSpawned")]
        private async void OnPlayerSpawn()
        {
            if (!ranSpawnChecker)
            {
                DisplayNUI();

                Exports["spawnmanager"].spawnPlayer(true);
                await Delay(3000);
                Exports["spawnmanager"].setAutoSpawn(false);

                ranSpawnChecker = true;
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task MainTick()
        {
            await Delay(55000);
            TriggerServerEvent("Framework:Server:syncInfo");

            foreach (Vehicle vehicle in World.GetAllVehicles().Where(v => suppressedModels.Contains(v.DisplayName.ToLower()) && !v.PreviouslyOwnedByPlayer))
            {
                vehicle.Delete();
            }
        }

        [Tick]
        private async Task SecondaryTick()
        {
            SetVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetParkedVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetRandomVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetPedDensityMultiplierThisFrame(densityMultiplier);
            SetScenarioPedDensityMultiplierThisFrame(densityMultiplier, densityMultiplier);

            DisablePlayerVehicleRewards(Game.Player.Handle);
            SetRadarZoom(1100);
        }

        [Tick]
        private async Task TeriaryTick()
        {
            if (!HUDIsVisible || IsHudHidden())
            {
                return;
            }

            TriggerEvent("Framework:Client:changeAOP", currentAOP);
        }
        #endregion
    }
}