using System.Collections.Generic;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Misc.Object;

namespace Red.InteractionMenu.Client.Menus
{
    internal class LEToolbox : BaseScript
    {
        #region Variables
        protected static bool shieldActive, holdingShield, usingShield, conductingCPR;
        protected static string shieldPropName = "prop_riot_shield", shieldAnimDict = "combat@gestures@gang@pistol_1h@beckon", bone;
        protected static int shieldNetworkId, index;
        protected static Prop shieldProp, spikeProp;

        protected static List<int> spawnedSpikes;

        protected static readonly Dictionary<string, int> wheelIndex = new Dictionary<string, int>()
        {
            { "wheel_lf", 0 },
            { "wheel_rf", 1 },
            { "wheel_lm", 2 },
            { "wheel_rm", 3 },
            { "wheel_lr", 4 },
            { "wheel_rr", 5 }
        };
        #endregion

        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Law Enforcement Toolbox");

            MenuItem sceneManangeBtn = new("Scene Management") { Label = Constants.forwardArrow };
            menu.AddMenuItem(sceneManangeBtn);
            MenuController.BindMenuItem(menu, SceneManagement.GetMenu(), sceneManangeBtn);

            menu.AddMenuItem(new MenuListItem("Hands On", new List<string> { "Grab", "Seat", "Unseat" }, 0));
            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Cuff", "Front Cuff", "Rear Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new MenuListItem("Duty Loadout", new List<string> { "Standard", "SWAT" }, 0));

            menu.AddMenuItem(new("Refill Taser Cartridges"));
            menu.AddMenuItem(new MenuListItem("Weapon Retention", new List<string> { "Long Gun", "Shotgun", "Less Lethal Shotgun" }, 0));
            menu.AddMenuItem(new("Breathalyzer"));

            menu.AddMenuItem(new MenuListItem("Set Spike Strips", new List<string> { "2", "3", "4" }, 0));
            menu.AddMenuItem(new("Remove Spike Strips"));

            menu.AddMenuItem(new("Toggle Riot Shield"));
            menu.AddMenuItem(new MenuListItem("Step Out Of Vehicle", new List<string> { "Driver", "Passenger", "Driver Rear", "Passenger Rear" }, 0));
            menu.AddMenuItem(new("Conduct CPR"));

            menu.AddMenuItem(new("~o~Back"));
            menu.AddMenuItem(new("~r~Close"));

            menu.OnItemSelect += Menu_OnItemSelect;
            menu.OnListItemSelect += Menu_OnListItemSelect;

            return menu;
        }

        private static async void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "Refill Taser Cartridges")
            {
                ExecuteCommand("refill");
            }
            else if (item == "Breathalyzer")
            {
                ExecuteCommand("bac");
            }
            else if (item == "Toggle Riot Shield")
            {
                ToggleShield();
            }
            else if (item == "Remove Spike Strips")
            {

            }
            else if (item == "Conduct CPR")
            {
                if (conductingCPR)
                {
                    PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_fail", 8.0f, -1, AnimationFlags.None);
                    conductingCPR = false;

                    await Delay(9500);
                    PlayerPed.Task.ClearAnimation("mini@cpr@char_a@cpr_str", "cpr_fail");
                }
                else
                {
                    PlayAnimation("mini@cpr@char_a@cpr_str", "cpr_pumpchest", 8.0f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.Loop);
                    conductingCPR = true;
                }
            }
            else if (item == "~o~Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
        }

        private static void Menu_OnListItemSelect(Menu menu, MenuListItem listItem, int selectedIndex, int itemIndex)
        {
            string item = listItem.Text;

            if (item == "Hands On")
            {
                switch (selectedIndex)
                {
                    case 0:
                        ExecuteCommand("grab");
                        break;

                    case 1:
                        ExecuteCommand("seat");
                        break;

                    case 2:
                        ExecuteCommand("unseat");
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Restrainment")
            {
                switch (selectedIndex)
                {
                    case 0:
                        ExecuteCommand("cuff");
                        break;

                    case 1:
                        ExecuteCommand("frontcuff");
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Duty Loadout")
            {
                PlayerPed.Weapons.RemoveAll();

                switch (selectedIndex)
                {
                    case 0:
                        PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 60, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.StunGun, 9999, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.Flashlight, 9999, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.Nightstick, 9999, false, true);
                        ExecuteCommand("lidarweapon");
                        PlayerPed.Weapons[WeaponHash.CombatPistol].Ammo = PlayerPed.Weapons[WeaponHash.CombatPistol].MaxAmmoInClip * 3;
                        PlayerPed.Weapons[WeaponHash.CombatPistol].Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        PlayerPed.Weapons.Select(WeaponHash.Unarmed);
                        SuccessNotification("You've equipped the standard duty loadout.", true);
                        break;

                    case 1:
                        PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 80, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.StunGun, 9999, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.CarbineRifle, 210, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.Flashlight, 9999, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.Knife, 9999, false, true);
                        PlayerPed.Weapons[WeaponHash.CombatPistol].Ammo = PlayerPed.Weapons[WeaponHash.CombatPistol].MaxAmmoInClip * 3;
                        PlayerPed.Weapons[WeaponHash.CarbineRifle].Ammo = PlayerPed.Weapons[WeaponHash.CarbineRifle].MaxAmmoInClip * 3;
                        PlayerPed.Weapons[WeaponHash.CombatPistol].Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtArFlsh].Active = true;
                        PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtScopeMedium].Active = true;
                        PlayerPed.Weapons.Select(WeaponHash.Unarmed);
                        SuccessNotification("You've equipped the SWAT duty loadout.", true);
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Weapon Retention")
            {
                switch (selectedIndex)
                {
                    case 0:
                        WeaponSystem(WeaponHash.CarbineRifle);
                        break;

                    case 1:
                        WeaponSystem(WeaponHash.PumpShotgun);
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Set Spike Strips")
            {
                switch (selectedIndex)
                {
                    case 0:
                        break;

                    case 1:
                        break;

                    case 2:
                        break;

                    default:
                        break;
                }
            }
        }

        #region Methods
        /// <summary>
        /// Manages weapon equipping and unequipping for police-specific weapons.
        /// </summary>
        /// <param name="hash">The hash of the weapon to handle.</param>
        private static void WeaponSystem(WeaponHash hash)
        {
            // Get the closest vehicle for proximity checks.
            Vehicle closestVehicle = GetClosestVehicle(1f);

            // Determine the appropriate gun name for notifications.
            string gun = hash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                _ => "gun"
            };

            // Check if the player is in or near a police vehicle.
            if (PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
            {
                // Get the player's current instance of the weapon, if any.
                Weapon playerWeapon = PlayerPed.Weapons[hash];

                // If the player already has the weapon, remove it.
                if (playerWeapon is not null)
                {
                    PlayerPed.Weapons.Remove(playerWeapon);
                    SuccessNotification($"You've unequipped and locked your {gun}.", true);
                }
                else
                {
                    // Give the player the weapon, set its ammo and components, and notify success
                    playerWeapon = PlayerPed.Weapons.Give(hash, 0, true, true);
                    playerWeapon.Ammo = playerWeapon.MaxAmmoInClip * 3;
                    playerWeapon.Components[WeaponComponentHash.AtArFlsh].Active = true;

                    if (hash == WeaponHash.CarbineRifle)
                    {
                        playerWeapon.Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtScopeMedium].Active = true;
                    }

                    SuccessNotification($"You've unlocked and equipped your {gun}", true);
                }
            }
            else
            {
                // Notify the player that they need to be near a police vehicle.
                ErrorNotification("You must be in or near a police cruiser to use this.", true);
            }
        }

        /// <summary>
        /// Toggles the ballistic shield on or off, handling attachment, animations, and constraints.
        /// </summary>
        private static async void ToggleShield()
        {
            // Get the closest vehicle for proximity checks.
            Vehicle closestVehicle = GetClosestVehicle(1.5f);

            // Validate conditions for using the shield.
            if (PlayerPed.IsInPoliceVehicle)
            {
                ErrorNotification("You can't be in a police car!", true);
                shieldActive = false;
                return;
            }

            if (closestVehicle?.ClassType != VehicleClass.Emergency)
            {
                ErrorNotification("You must be near a police cruiser to do this.", true);
                shieldActive = false;
                return;
            }

            if (!PlayerPed.Weapons.HasWeapon(WeaponHash.CombatPistol))
            {
                ErrorNotification("You must have a combat pistol in order to do this.");
                shieldActive = false;
                return;
            }

            // If the shield is not currently held:
            if (!holdingShield)
            {
                // Request necessary models and animations.
                RequestModel(shieldPropName);
                RequestAnim(shieldAnimDict);

                // Create the shield prop and set its properties.
                shieldProp = await CreateProp(shieldPropName, PlayerPed.Position, false, false);
                shieldProp.IsPersistent = true;
                shieldProp.IsInvincible = true;

                // Set network properties for the shield prop
                shieldNetworkId = shieldProp.NetworkId;

                SetNetworkIdExistsOnAllMachines(shieldNetworkId, true);
                NetworkSetNetworkIdDynamic(shieldNetworkId, true);
                SetNetworkIdCanMigrate(shieldNetworkId, false);

                // Play shield animation and attach the prop to the player's left hand.
                PlayerPed.PlayAnim(shieldAnimDict, "0", 8.0f, -8.0f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation, 0.0f, false, false, false);

                AttachEntityToEntity(shieldProp.Handle, PlayerPed.Handle, GetEntityBoneIndexByName(PlayerPed.Handle, "IK_L_Hand"), 0.0f, -0.05f, -0.10f, -30.0f, 180.0f, 40.0f, false, false, true, false, 0, true);
                SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Gang1H"));

                if (PlayerPed.Weapons.HasWeapon(WeaponHash.CombatPistol) && GetSelectedPedWeapon(PlayerPed.Handle) != (int)WeaponHash.CombatPistol)
                {
                    PlayerPed.Weapons.Select(WeaponHash.CombatPistol);
                }

                holdingShield = true;
                shieldActive = true;

                TriggerEvent("Menu:Client:disableShieldControls", shieldActive); // Trigger a event to disable control while holding a shield.
            }
            else
            {
                // Clear all the players tasks 
                ClearAllTasksImmediately();

                // Detach the shield from the ped.
                DetachEntity(shieldProp.Handle, true, true);

                // Delete the entity on the network end
                DeleteEntity(ref shieldNetworkId);
                shieldProp.Delete(); // Delete the prop on client side.

                shieldActive = false;
                usingShield = false;

                TriggerEvent("Menu:Client:disableShieldControls", shieldActive); // Trigger a event to enable control while holding a shield.
            }
        }

        private async Task DisableShieldControls()
        {
            DisableControlAction(1, 23, true); // F to enter the vehicle.
            DisableControlAction(1, 75, true); // F to leave the vehicle.
        }
        #endregion

        #region Event Handlers
        [EventHandler("Menu:Client:disableShieldControls")]
        private void OnDisableShieldControls(bool disableControls)
        {
            if (disableControls)
            {
                Tick += DisableShieldControls;
            }

            Tick -= DisableShieldControls;
        }
        #endregion
    }
}
