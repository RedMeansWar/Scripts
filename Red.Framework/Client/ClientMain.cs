using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Client.Misc;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.Framework.Client
{
    internal class ClientMain : BaseScript
    {
        #region Variables
        protected bool ranSpawnChecker;
        protected float densityMultiplyer = 1f;
        protected string currentAOP = "Statewide";
        protected Ped PlayerPed = Game.PlayerPed;
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

            // NUI Callbacks
            RegisterNuiCallback("selectCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(SelectCharacter));
            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));
            RegisterNuiCallback("disconnect", new Action<IDictionary<string, object>, CallbackDelegate>(Disconnect));
            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));

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
        private void FrameworkCommand() => DisplayNUI(true);

        [Command("fw")]
        private void FwCommand() => DisplayNUI(true);

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => DisplayNUI(true);
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

        private void SelectCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetVal("charId", "0");
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string dob = data.GetVal<string>("dob", null);
            string department = data.GetVal<string>("dept", null);
            string twitterName = data.GetVal<string>("twitterName", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(twitterName))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an eunexpected error choosing this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, department, twitterName, charId);

            currentCharacter = createdCharacter;

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_NUI"
            }));

            SetNuiFocus(false, false);

            TriggerEvent("Framework:Client:selectCharacter", Json.Stringify(currentCharacter));
        }

        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string dob = data.GetVal<string>("dob", null);
            string department = data.GetVal<string>("dept", null);
            string twitterName = data.GetVal<string>("twitterName", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(twitterName))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error creating this character, try again."
                }));

                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, department, twitterName, "0");

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
                msg = "Deleted character!"
            }));
        }

        private void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetVal("charId", "-1");
            string firstName = data.GetVal<string>("firstName", null);
            string lastName = data.GetVal<string>("lastName", null);
            string gender = data.GetVal<string>("gender", null);
            string dob = data.GetVal<string>("dob", null);
            string department = data.GetVal<string>("dept", null);
            string twitterName = data.GetVal<string>("twitterName", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department) ||  string.IsNullOrWhiteSpace(twitterName)) 
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error edit this character, try again."
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
        private Character CreateCharacter(string firstName, string lastName, string gender, string dob, string department, string twitterName, string characterId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(characterId),
                FirstName = firstName,
                LastName = lastName,
                DoB = DateTime.Parse(dob),
                Gender = gender,
                Department = department,
                TwitterName = twitterName,
            };

            return createdCharacter;
        }

        private void DisplayNUI(bool display)
        {
            display = true;

            if (!display)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "CANCEL_NUI"
                }));

                SetNuiFocus(false, false);
            }

            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI"
            }));

            SetNuiFocus(true, true);
        }
        #endregion

        #region Ticks
        #endregion
    }
}