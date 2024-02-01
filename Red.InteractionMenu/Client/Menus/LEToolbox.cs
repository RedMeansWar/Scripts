using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Misc.Object;
using System.Threading.Tasks;

namespace Red.InteractionMenu.Client.Menus
{
    internal class LEToolbox : BaseScript
    {
        #region Variables
        protected static bool shieldActive;
        protected static int shieldEnt;
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

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
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
        }

        #region Methods
        private static void WeaponSystem(WeaponHash hash)
        {
            Vehicle closestVehicle = GetClosestVehicle(1f);

            string gun = hash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                _ => "gun"
            };

            if (PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
            {
                Weapon playerWeapon = PlayerPed.Weapons[hash];

                if (playerWeapon is not null)
                {
                    PlayerPed.Weapons.Remove(playerWeapon);
                    SuccessNotification($"You've unequipped and locked your {gun}.", true);
                }
                else
                {
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
                ErrorNotification("You must be in or near a police cruiser to use this.", true);
            }
        }

        private static async void ToggleShield()
        {
            shieldActive = true;

            Vector3 playerPos = PlayerPed.Position;
            Vehicle closestVehicle = GetClosestVehicle(1f);

            if (closestVehicle is null)
            {
                shieldActive = false;

                ErrorNotification("You must be near a police cruiser to do this.", true);
                return;
            }

            if (!PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
            {
                RequestAnimation("combat@gestures@gang@pistol_1h@beckon");
                PlayerPed.PlayAnim("combat@gestures@gang@pistol_1h@beckon", "0", 8.0f, -8.0f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly, 0.0f, false, false, false);

                await LoadModel("prop_ballistic_shield");
                var shield = CreateObject(GetHashKey("prop_ballistic_shield"), playerPos.X, playerPos.Y, playerPos.Z, true, true, true);

                int shieldEnt = shield;

                AttachEntityToEntity(shieldEnt, PlayerPed.Handle, GetEntityBoneIndexByName(PlayerPed.Handle, "IK_L_Hand"), 0.0f, -0.05f, -0.10f, -30.0f, 180.0f, 40.0f, false, false, true, false, 0, true);
                SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Gang1H"));

                if (HasPedGotWeapon(PlayerPed.Handle, (uint)WeaponHash.CombatPistol, false) || GetSelectedPedWeapon(PlayerPed.Handle) == (int)WeaponHash.CombatPistol)
                {
                    SetCurrentPedWeapon(PlayerPed.Handle, (uint)WeaponHash.CombatPistol, true);
                }
                else
                {
                    GiveWeaponToPed(PlayerPed.Handle, (uint)WeaponHash.CombatPistol, 80, false, true);
                    SetCurrentPedWeapon(PlayerPed.Handle, (uint)WeaponHash.CombatPistol, true);
                }

                SetEnableHandcuffs(PlayerPed.Handle, true);

                if (shieldActive)
                {
                    TriggerEvent("Menu:Client:disableRiotShieldControls", true);
                }
            }

            if (PlayerPed.Weapons.Current == WeaponHash.Unarmed && shieldActive)
            {
                DeleteEntity(ref shieldEnt);
                ClearPedTasksImmediately(PlayerPed.Handle);

                SetCurrentPedWeapon(PlayerPed.Handle, (uint)WeaponHash.Unarmed, true);
                SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Default"));

                shieldActive = false;
            }
            else
            {
                ErrorNotification("You need combat pistol and have it selected to do this!", true);
            }
        }

        [EventHandler("Menu:Client:disableRiotShieldControls")]
        private void OnDisableWhileShieldActive(bool shieldActive)
        {
            if (shieldActive)
            {
                Tick += DisableWhileShieldActive;
            }
            else
            {
                Tick -= DisableWhileShieldActive;
            }
        }

        private async Task DisableWhileShieldActive()
        {
            DisableControlAction(1, 23, true);
            DisableControlAction(1, 75, true);
        }
        #endregion
    }
}
