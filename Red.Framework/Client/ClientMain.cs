using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Red.Common.Client.Misc;
using Red.Framework.Client.Misc;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Framework.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected float densityMultiplier = 1f;
        protected string currentAOP = "Statewide";
        public static Character currentCharacter = new();

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
            uint PLAYER = Game.GenerateHashASCII("PLAYER");

            SetRandomTrains(false);
            SetRandomBoats(false);

            StartAudioScene("CHARACTER_CHANGE_IN_SKY_SCENE");
            SetAudioFlag("PoliceScannerDisabled", true);

            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));
            RegisterNuiCallback("disconnect", new Action<IDictionary<string, object>, CallbackDelegate>(Disconnect));
            RegisterNuiCallback("hideUI", new Action<IDictionary<string, object>, CallbackDelegate>(HideNUI));
            RegisterNuiCallback("setCharacterOnline", new Action<IDictionary<string, object>, CallbackDelegate>(SetCharacterOnline));
            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));

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
        private void FrameworkCommand() => DisplayFrameworkNUI();

        [Command("fw")]
        private void FwCommand() => DisplayFrameworkNUI();

        [Command("changecharacter")]
        private void ChangeCharacterCommand() => DisplayFrameworkNUI();

        #endregion

        #region Methods
        private void DisplayFrameworkNUI(bool display = true)
        {
            if (display)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "DISPLAY_UI"
                }));

                SetNuiFocus(true, true);
            }
            else if (display is false)
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "CANCEL_UI"
                }));

                SetNuiFocus(false, false);
            }
        }

        private Character CreateCharacter(string firstName, string lastName, string gender, string dob, string twitterName, string department, string charId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                TwitterName = twitterName,
                Department = department
            };

            return createdCharacter;
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:updateAOP")]
        private void OnUpdateAOP(string newAOP, string aopSetter)
        {
            currentAOP = newAOP;
            AddChatMessage("~r~SYSTEM", $"AOP set to ^5^*{newAOP}^r^7 by ^5^*{aopSetter}^r^7");

            SendNuiMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"Welcome to Red Framework! (AOP: {newAOP}"
            }));
        }
        #endregion

        #region NUI Callbacks
        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result) => ForceSocialClubUpdate();
        
        private void Disconnect(IDictionary<string, object> data, CallbackDelegate result) => TriggerServerEvent("Framework:Server:dropUser", "Dropped via framework.");

        private void HideNUI(IDictionary<string, object> data, CallbackDelegate result) => DisplayFrameworkNUI(false);

        private void SetCharacterOnline(IDictionary<string, object> data, CallbackDelegate result)
        {
            string charId = data.GetValue("charId", "0");
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string twitterName = data.GetValue<string>("twitterName", null);
            string department = data.GetValue<string>("department", null);

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

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, twitterName, department, charId);

            currentCharacter = createdCharacter;

            SendNuiMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNuiFocus(false, false);

            AddChatMessage("", $"You're now playing as {createdCharacter.FirstName} {createdCharacter.LastName} ({createdCharacter.Department})");
            TriggerEvent("Framework:Client:characterSelected", Json.Stringify(currentCharacter));
        }

        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string twitterName = data.GetValue<string>("twitterName", null);
            string department = data.GetValue<string>("department", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department) || string.IsNullOrWhiteSpace(twitterName))
            {
                SendNuiMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    message = "We ran into an unexpected error creating this character, try again."
                }));
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, dob, twitterName,department, "0");

            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(createdCharacter));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                message = $"{firstName} {lastName} ({department}) has been created!"
            }));
        }

        private void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetValue<string>("characterId", null);
            TriggerServerEvent("Framework:Server:deleteCharacter", long.Parse(characterId));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                message = "Character deleted!"
            }));
        }
        private void EditCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string twitterName = data.GetValue<string>("twitterName", null);
            string department = data.GetValue<string>("department", null);
            string charId = data.GetValue("charId", "-1");

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(twitterName) || string.IsNullOrWhiteSpace(department))
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

            TriggerServerEvent("pnw:framework:server:editCharacter", Json.Stringify(editedCharacter));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task MainFrameworkTick()
        {
            DistantCopCarSirens(false);

            foreach (Ped ped in World.GetAllPeds())
            {
                ped.BlockPermanentEvents = true;
            }

            await Delay(250);
        }

        [Tick]
        private async Task SecondaryFrameworkTick()
        {
            SetVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetParkedVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetRandomVehicleDensityMultiplierThisFrame(densityMultiplier);
            SetPedDensityMultiplierThisFrame(densityMultiplier);
            SetScenarioPedDensityMultiplierThisFrame(densityMultiplier, densityMultiplier);

            DisablePlayerVehicleRewards(Game.Player.Handle);
            SetRadarZoom(1100);
        }
        #endregion
    }
}
