using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Red.Common.Client.Misc;
using SharpConfig;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Diagnostics.Log;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Hud.NUI;

namespace Red.Framework.Client
{
    internal class ClientMain : BaseScript
    {
        #region Variables
        protected bool ranSpawnChecker, teleported, usingDiscordPresence;
        protected float densityMultiplier = 1f;
        protected string currentAOP = "Statewide";
        protected string communityName, discordAppId, discordRichPresenceAssetLogo, discordRichPresenceAssetText;
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
            RegisterNUICallback("selectCharacter", SelectCharacter);
            RegisterNUICallback("createCharacter", CreateCharacter);
            RegisterNUICallback("deleteCharacter", DeleteCharacter);
            RegisterNUICallback("editCharacter", EditCharacter);
            RegisterNUICallback("disconnect", Disconnect);
            RegisterNUICallback("quitGame", QuitGame);
            RegisterNUICallback("cancelNUI", CancelNUI);
            RegisterNUICallback("spawnAtPrison", SpawnAtPrison);
            RegisterNUICallback("spawnAtGrapeseed", SpawnAtGrapeseed);
            RegisterNUICallback("spawnAtMotelNew", SpawnAtMotelNew);
            RegisterNUICallback("spawnAtMotel", SpawnAtMotel);
            RegisterNUICallback("spawnAtAbandonedMotel", SpawnAtAbandonedMotel);
            RegisterNUICallback("spawnAtCasino", SpawnAtCasino);
            RegisterNUICallback("spawnAtGroveStreet", SpawnAtGroveStreet);
            RegisterNUICallback("spawnAtMorningwoodHotel", SpawnAtMorningwoodHotel);
            RegisterNUICallback("spawnAtStarLane", SpawnAtStarLane);
            RegisterNUICallback("spawnAtNikolaPlace", SpawnAtNikolaPlace);
            RegisterNUICallback("spawnAtVinewoodPd", SpawnAtVinewoodPd);
            RegisterNUICallback("spawnAtSandyPd", SpawnAtSandyPd);
            RegisterNUICallback("spawnAtDavisPd", SpawnAtDavisPd);
            RegisterNUICallback("spawnAtPaletoPd", SpawnAtPaletoPd);
            RegisterNUICallback("spawnAtMissionRowPd", SpawnAtMissionRowPd);
            RegisterNUICallback("spawnAtRockfordPd", SpawnAtRockfordPd);
            RegisterNUICallback("spawnAtDelPerroPd", SpawnAtDelPerroPd);

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
                SetRelationshipBetweenGroups(1, Game.GenerateHashASCII(relationshipGroup), PLAYER);
            }

            foreach (string suppressedModel in suppressedModels)
            {
                SetVehicleModelIsSuppressed(Game.GenerateHashASCII(suppressedModel), true);
            }

            for (int i = 0; i < 15; i++)
            {
                EnableDispatchService(i, false);
            }

            if (usingDiscordPresence)
            {
                SetDiscordAppId(discordAppId);
                SetDiscordRichPresenceAsset(discordRichPresenceAssetLogo);
                SetDiscordRichPresenceAssetText(discordRichPresenceAssetText);
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
            SendNUIMessage(Json.Stringify(new
            {
                type = "CLOSE_UI"
            }));

            SetNUIFocus(false, false);
            Info("Revoking Nui Callback");

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
                SendNUIMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error choosing this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, charId);

            currentCharacter = createdCharacter;
            TriggerEvent("Framework:Client:characterSelected", Json.Stringify(createdCharacter));

            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "Civ"
                }));
            }

            if (currentCharacter.Department == "LSPD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "LSPD"
                }));
            }

            if (currentCharacter.Department == "SAHP")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "SAHP"
                }));
            }

            if (currentCharacter.Department == "BCSO")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "BCSO"
                }));
            }

            if (currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "DISPLAY_SPAWN",
                    department = "LSFD"
                }));
            }

            Info($"Selected Character [{currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})]");
            TriggerEvent("chat:addMessage", $"{communityName}", new[] { 255, 255, 255 }, $"You are now playing as {currentCharacter.FirstName} {currentCharacter.LastName} ({currentCharacter.Department})");
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
                SendNUIMessage(Json.Stringify(new
                {
                    type = "ERROR",
                    msg = "We ran into an unexpected error creating this character, try again."
                }));
                result(new { success = false, message = "not valid character data" });
                return;
            }

            Character createdCharacter = CreateCharacter(firstName, lastName, gender, department, dob, cash, bank, "0");

            TriggerServerEvent("Framework:Server:createCharacter", Json.Stringify(createdCharacter));

            SendNUIMessage(Json.Stringify(new
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

            SendNUIMessage(Json.Stringify(new
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
                SendNUIMessage(Json.Stringify(new
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

            SendNUIMessage(Json.Stringify(new
            {
                type = "SUCCESS",
                msg = "Character edited!"
            }));

            result(new { success = true, message = "success" });
        }

        private async void SpawnAtPrison(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1848.59f, 2585.88f, 45.67f);
                SetEntityHeading(Game.Player.Character.Handle, 270.0f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtGrapeseed(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1666.2f, 4740.12f, 41.99f);
                SetEntityHeading(Game.Player.Character.Handle, 282.89f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtMotelNew(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1114.75f, 2641.67f, 38.14f);
                SetEntityHeading(Game.Player.Character.Handle, 354.14f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtMotel(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(366.72f, 2625.21f, 44.67f);
                SetEntityHeading(Game.Player.Character.Handle, 26.68f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtAbandonedMotel(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1569.59f, 3607.65f, 35.37f);
                SetEntityHeading(Game.Player.Character.Handle, 26.12f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtCasino(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(885.27f, -2.63f, 78.76f);
                SetEntityHeading(Game.Player.Character.Handle, 414.21f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtGroveStreet(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(90.7f, -1965.5f, 20.75f);
                SetEntityHeading(Game.Player.Character.Handle, 316.16f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtMorningwoodHotel(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(-1221.59f, -184.09f, 39.18f);
                SetEntityHeading(Game.Player.Character.Handle, 119.75f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtStarLane(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1404.22f, 2169.01f, 97.88f);
                SetEntityHeading(Game.Player.Character.Handle, 262.06f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtNikolaPlace(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "Civ")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1395.14f, -574.76f, 74.34f);
                SetEntityHeading(Game.Player.Character.Handle, 107.64f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtVinewoodPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(639.69f, 0.57f, 82.79f);
                SetEntityHeading(Game.Player.Character.Handle, 254.93f);


                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtSandyPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(1864.19f, 3699.68f, 33.61f);
                SetEntityHeading(Game.Player.Character.Handle, 33.61f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtDavisPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(373.76f, -1610.07f, 29.29f);
                SetEntityHeading(Game.Player.Character.Handle, 232.08f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtPaletoPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(-447.72f, 6000.73f, 31.69f);
                SetEntityHeading(Game.Player.Character.Handle, 137.97f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtMissionRowPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(431.46f, -981.35f, 90.71f);
                SetEntityHeading(Game.Player.Character.Handle, 30.71f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtRockfordPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(-560.78f, -133.86f, 38.09f);
                SetEntityHeading(Game.Player.Character.Handle, 197.56f);

                await Delay(0);
                CloseSpawnModals();
            }
        }

        private async void SpawnAtDelPerroPd(IDictionary<string, object> data, CallbackDelegate result)
        {
            if (currentCharacter.Department == "SAHP" || currentCharacter.Department == "LSPD" || currentCharacter.Department == "BCSO" || currentCharacter.Department == "LSFD")
            {
                SendNUIMessage(Json.Stringify(new
                {
                    type = "CLOSE_UI"
                }));

                SetNUIFocus(false, false);
                await Delay(100);

                TeleportToSpawnLocation(-1078.2f, -857.61f, 5.04f);
                SetEntityHeading(Game.Player.Character.Handle, 210.23f);

                await Delay(0);
                CloseSpawnModals();
            }
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

        private async void TeleportToSpawnLocation(float x, float y, float z)
        {
            Vector3 spawnLocation = new(x, y, z);

            Game.Player.Character.IsInvincible = true;
            Game.Player.Character.IsCollisionEnabled = false;
            Game.Player.Character.HasGravity = false;

            SwitchOutPlayer(PlayerPedId(), 0, 1);

            await Delay(3000);

            Game.Player.Character.Position = spawnLocation;
            Function.Call((Hash)0xD8295AF639FD9CB8, PlayerPedId());

            await Delay(3000);

            Game.Player.Character.IsInvincible = false;
            Game.Player.Character.IsCollisionEnabled = true;
            Game.Player.Character.HasGravity = true;
        }

        private void CloseSpawnModals()
        {
            SendNUIMessage(Json.Stringify(new
            {
                type = "HIDE_SPAWN_MODALS"
            }));
        }

        private void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("Framework", "EnableDiscordRickPresence") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                
                communityName = loaded["Framework"]["CommunityName"].StringValue;
                usingDiscordPresence = loaded["Framework"]["EnableDiscordRickPresence"].BoolValue;
                discordAppId = loaded["Framework"]["DiscordAppId"].StringValue;
                discordRichPresenceAssetLogo = loaded["Framework"]["DiscordRichPresenceAsset"].StringValue;
                discordRichPresenceAssetText = loaded["Framework"]["DiscordRichPresenceText"].StringValue;
            }
            else
            {
                TriggerServerEvent("Framework:Server:configError", "Config file wasn't setup correctly!");
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:changeAOP")]
        private void OnChangeAOP(string newAOP, string aopSetter)
        {
            currentAOP = newAOP;
            TriggerEvent("_chat:chatMessage", $"{communityName}", new[] { 255, 255, 255 }, $"Current AOP is ^5^*{currentAOP}^r^7 (Set by: ^5^*{aopSetter}^r^7)");

            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"Welcome to San Andreas! (AOP: {newAOP})"
            }));
        }

        [EventHandler("Framework:Client:syncInfo")]
        private void OnSyncInfo(string aop)
        {
            currentAOP = aop;

            SendNUIMessage(Json.Stringify(new
            {
                type = "UPDATE_AOP",
                aop = $"Welcome to San Andreas! (AOP: {aop})"
            }));
        }

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
            Info($"Returned {characterList.Count} character(s)");

            SetNUIFocus(true, true);

            SendNUIMessage(Json.Stringify(new
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
                CloseSpawnModals();

                Exports["spawnmanager"].spawnPlayer(true);
                await Delay(3000);
                Exports["spawnmanager"].setAutoSpawn(false);

                DisplayNUI();

                ranSpawnChecker = true;
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task MainTick()
        {
            await Delay(55000);
            TriggerServerEvent("Framework:Server:syncInfo", currentAOP);

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

            if (!HUDIsVisible || IsHUDHidden())
            {
                return;
            }
        }
        #endregion
    }
}