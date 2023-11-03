using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Client.Misc;
using Red.Framework.Client.Misc;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Framework.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool ran;
        public static Character currentCharacter;
        protected ISet<string> allowedDepartments = new HashSet<string>();
        protected string currentAOP = "Not Set";

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

            // NUI
            RegisterNuiCallback("cancelNUI", new Action<IDictionary<string, object>, CallbackDelegate>(CancelNUI));
            RegisterNuiCallback("disconnect", new Action<IDictionary<string, object>, CallbackDelegate>(DisconnectServer));
            RegisterNuiCallback("quitGame", new Action<IDictionary<string, object>, CallbackDelegate>(QuitGame));
            RegisterNuiCallback("createCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(CreateCharacter));
            RegisterNuiCallback("deleteCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(DeleteCharacter));
            RegisterNuiCallback("editCharacter", new Action<IDictionary<string, object>, CallbackDelegate>(EditCharacter));
            RegisterNuiCallback("setCharacterOnline", new Action<IDictionary<string, object>, CallbackDelegate>(SetCharacterOnline));
        }
        #endregion

        #region Commands
        [Command("framework")]
        private void FrameworkCommand() => DisplayNUI();

        [Command("fw")]
        private void FwCommand() => DisplayNUI();

        [Command("changecharacter")]
        private void ChangeCharacter() => DisplayNUI();
        
        [Command("dob")]
        private void DobCommand()
        {
            if (currentCharacter is null)
            {
                AddChatMessage("SYSTEM", "You must choose a character in the framework before using this command!", 0, 73, 83);
            }
            else
            {
                AddChatMessage("SYSTEM", $"{currentCharacter.FirstName} {currentCharacter.LastName}'s date of birth is {currentCharacter.DoB:MM/dd/yyyy}", 0, 73, 83);
            }
        }
        #endregion

        #region Methods
        private void AddIfNotExists(string dept)
        {
            if (!allowedDepartments.Contains(dept))
            {
                allowedDepartments.Add(dept);
            }
        }

        private Character CreateCharacter(string firstName, string lastName, string gender, string dob, string dept, string charId)
        {
            Character createdCharacter = new()
            {
                CharacterId = int.Parse(charId),
                FirstName = firstName,
                LastName = lastName,
                Gender = gender,
                DoB = DateTime.Parse(dob),
                Department = dept
            };

            return createdCharacter;
        }
        #endregion

        #region NUI Callbacks
        private void DisplayNUI()
        {
            SetNuiFocus(true, true);
            
            SendNuiMessage(Json.Stringify(new
            {
                type = "DISPLAY_NUI"
            }));

            FrameworkLog.Info("Invoking Framework NUI");
        }

        private void CancelNUI(IDictionary<string, object> data, CallbackDelegate result)
        {
            SetNuiFocus(false, false);

            SendNuiMessage(Json.Stringify(new
            {
                type = "CANCEL_NUI"
            }));

            FrameworkLog.Info("Revoking Framework NUI");
        }

        private void DisconnectServer(IDictionary<string, object> data, CallbackDelegate result)
        {
            TriggerServerEvent("Framework:Server:DropUser", "Disconnected via framework.");
        }

        private void QuitGame(IDictionary<string, object> data, CallbackDelegate result)
        {
            ForceSocialClubUpdate();
        }

        private void CreateCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string firstName = data.GetValue<string>("firstName", null);
            string lastName = data.GetValue<string>("lastName", null);
            string gender = data.GetValue<string>("gender", null);
            string dob = data.GetValue<string>("dob", null);
            string department = data.GetValue<string>("department", null);

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department))
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
            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(createdCharacter));

            SendNuiMessage(Json.Stringify(new
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

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department))
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

        private async void DeleteCharacter(IDictionary<string, object> data, CallbackDelegate result)
        {
            string characterId = data.GetValue<string>("characterId", null);

            TriggerServerEvent("Framework:Server:deleteCharacter", long.Parse(characterId));

            SendNuiMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character deleted!"
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

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(gender) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(department))
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

            currentCharacter = createdCharacter;

            SendNuiMessage(Json.Stringify(new
            {
                type = "CANCEL_NUI"
            }));
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:updateAOP")]
        private void OnUpdateAOP(string newAOP, string setter)
        {
            currentAOP = newAOP;
            AddChatMessage("Framework", $"AOP set to ^5^*{newAOP}^r^7 by ^5^*{setter}^r^7");

            SendNuiMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"Welcome to San Andreas! (AOP: {newAOP})"
            }));
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
