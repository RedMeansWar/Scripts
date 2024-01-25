using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using Red.InteractionMenu.Client.Menus.SubMenus;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Constants;

namespace Red.InteractionMenu.Client.Menus
{
    internal class PoliceMenu : BaseScript
    {
        #region Variables
        protected static Prop shieldProp;
        protected static bool hadPistolForShield, shieldEnabled;
        #endregion

        public static Menu GetMenu()
        {
            Menu menu = new("Red Menu", "~b~Police Menu");
            MenuItem sceneManageBtn = new("Scene Management");

            menu.AddMenuItem(sceneManageBtn);
            MenuController.BindMenuItem(GetMenu(), SceneManagement.GetMenu(), sceneManageBtn);

            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Cuff", "Front Cuff", "Rear Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new MenuListItem("Hands On", new List<string> { "Grab", "Seat", "Unseat"}, 0));
            menu.AddMenuItem(new("Breathalyzer"));
            
            menu.AddMenuItem(new("Toggle LiDAR Gun"));
            menu.AddMenuItem(new("Search Vehicle"));
            menu.AddMenuItem(new("Search Ped"));

            menu.AddMenuItem(new("Refill Taser Cartridges"));
            menu.AddMenuItem(new MenuListItem("Weapon Retention", new List<string> { "Carbine", "Shotgun", "SMG" }, 0));
            menu.AddMenuItem(new("Conduct CPR"));

            menu.AddMenuItem(new MenuListItem("Step Out Of Vehicle", new List<string> { "Driver", "Passanger", "Rear Driver", "Rear Passanger" }, 0));
            menu.AddMenuItem(new("Toggle Shield"));
            menu.AddMenuItem(new MenuListItem("Loadout", new List<string> { "Default", "SWAT" }, 0));

            menu.AddMenuItem(new("Toggle Beanbag Shotgun"));
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
            else if (item == "Conduct CPR")
            {
                TaskPlayAnim(PlayerPed.Handle, "mini@cpr@char_a@cpr_de", "cpr_success", 8.0f, 1.0f, -1, 2, 0.0f, false, false, false);
            }
            else if (item == "Toggle Shield")
            {
                ToggleShield();
            }
            else if (item == "Toggle LiDAR Gun")
            {
                Vehicle closestVehicle = GetClosestVehicle(1f);

                if (PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
                {
                    if (PlayerPed.Weapons.HasWeapon((WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4")))
                    {
                        SuccessNotification("You've put the LiDAR gun back on the passenger seat.", true);
                    }
                    else
                    {
                        SuccessNotification("You've taken the LiDAR gun off the passenger seat.", true);
                    }

                    ExecuteCommand("lidarweapon");
                }
                else
                {
                    ErrorNotification("You must be in or near a police cruiser to use this.", true);
                }
            }
            else if (item == "Toggle Beanbag Shotgun")
            {
                Vehicle closestVehicle = GetClosestVehicle(1f);

                if (PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
                {
                    if (PlayerPed.Weapons.HasWeapon((WeaponHash)Game.GenerateHashASCII("WEAPON_BEANBAGSHOTGUN")))
                    {
                        SuccessNotification("You have equipped your beanbag shotgun.", true);
                    }
                    else
                    {
                        SuccessNotification("You have unequipped your beanbag shotgun.", true);
                    }

                    PlayerPed.Weapons.Give((WeaponHash)Game.GenerateHashASCII("WEAPON_BEANBAGSHOTGUN"), 60, true, true);
                }
                else
                {
                    ErrorNotification("You must be in or near a police cruiser to use this.", true);
                }
            }
            else if (item == "Toggle Radar Remote")
            {
                if (PlayerPed.IsInPoliceVehicle)
                {
                    TriggerEvent("wk:openRemote");
                }
                else
                {
                    ErrorNotification("You must be in or near a police cruiser to use this.", true);
                }
            }
            else if (item == "Breathalyzer")
            {
                ExecuteCommand("bac");
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

            if (item == "Restrainment")
            {
                switch (selectedIndex)
                {
                    case 0:
                        ExecuteCommand("cuff");
                        break;

                    case 1:
                        ExecuteCommand("frontcuff");
                        break;

                    case 2:
                        ExecuteCommand("ziptie");
                        break;

                    case 3:
                        ExecuteCommand("frontziptie");
                        break;

                    default:
                        break;
                }
            }
            else if (item == "Hands On")
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
            else if (item == "Loadout")
            {
                switch (selectedIndex)
                {
                    case 0:
                        PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 60, false, true);
                        PlayerPed.Weapons.Give(WeaponHash.StunGun, 9999, false, false);
                        PlayerPed.Weapons.Give(WeaponHash.Flashlight, 9999, false, false);

                        PlayerPed.Weapons.Give(WeaponHash.Nightstick, 9999, false, false);
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
                        RetainWeaponSystem(WeaponHash.CarbineRifle);
                        break;

                    case 1:
                        RetainWeaponSystem(WeaponHash.PumpShotgun);
                        break;

                    case 2:
                        RetainWeaponSystem(WeaponHash.MicroSMG);
                        break;

                    default:
                        break;
                }
            }
        }
        #region Methods
        private static void RetainWeaponSystem(WeaponHash weaponHash)
        {
            Vehicle closestVehicle = GetClosestVehicle(1f);

            string gun = weaponHash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                WeaponHash.MicroSMG => "9mm open-bolt submachine gun",
                _ => "gun"
            };

            if (PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
            {
                Weapon playerWeapon = PlayerPed.Weapons[weaponHash];

                if (playerWeapon is not null)
                {
                    PlayerPed.Weapons.Remove(playerWeapon);
                    SuccessNotification($"You've unequipped and locked your {gun}.", true);
                }
                else
                {
                    playerWeapon = PlayerPed.Weapons.Give(weaponHash, 0, true, true);
                    playerWeapon.Ammo = playerWeapon.MaxAmmoInClip * 3;
                    playerWeapon.Components[WeaponComponentHash.AtArFlsh].Active = true;

                    if (weaponHash == WeaponHash.CarbineRifle)
                    {
                        playerWeapon.Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtScopeMedium].Active = true;
                    }

                    SuccessNotification($"You've unlocked and equipped your {gun}.", true);
                }
            }
            else
            {
                ErrorNotification("You must be in or near a police cruiser to use this.", true);
            }
        }

        private static async void ToggleShield()
        {
            Vehicle closestVehicle = GetClosestVehicle(1f);
            string animDict = "combat@gestures@gang@pistol_1h@beckon";
            int propEntity = shieldProp.Handle;

            shieldEnabled = !shieldEnabled;

            if (closestVehicle is null || closestVehicle?.ClassType != VehicleClass.Emergency)
            {
                shieldEnabled = false;

                ErrorNotification("You must be near a police cruiser to do this");
                return;
            }

            if (PlayerPed.IsGettingIntoAVehicle || PlayerPed.IsInVehicle())
            {
                shieldEnabled = false;

                ErrorNotification("You can't be in a vehicle to get a shield out.");
                return;
            }

            RequestAnimDict(animDict);
            TaskPlayAnim(PlayerPed.Handle, animDict, "0", 8.0f, 8.0f, -1, 2 + 16 + 32, 0.0f, false, false, false);

            shieldProp = await World.CreateProp("prop_ballistic_shield", PlayerPed.Position, true, true);
            
            AttachEntityToEntity(shieldProp.Handle, PlayerPed.Handle, GetPedBoneIndex(PlayerPed.Handle, 6286), 0.0f, -0.05f, -0.10f, -30.0f, 180.0f, 40.0f, false, false, true, false, 0, true);
            SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Gang1H"));

            if (HasPedGotWeapon(PlayerPed.Handle, (uint)WeaponHash.Pistol, false) || GetSelectedPedWeapon(PlayerPed.Handle) == (int)WeaponHash.Pistol)
            {
                PlayerPed.Weapons.Select(WeaponHash.Pistol);
                hadPistolForShield = true;
            }
            else
            {
                PlayerPed.Weapons.Give(WeaponHash.Pistol, 60, true, false);
                hadPistolForShield = false;
            }

            SetEnableHandcuffs(PlayerPed.Handle, true);
            if (!hadPistolForShield)
            {
                DeleteEntity(ref propEntity);
                PlayerPed.Task.ClearAllImmediately();

                SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Default"));
                SetCurrentPedWeapon(PlayerPed.Handle, (uint)WeaponHash.Unarmed, true);

                PlayerPed.Weapons.Remove(WeaponHash.CombatPistol);
                SetEnableHandcuffs(PlayerPed.Handle, false);

                hadPistolForShield = false;
                shieldEnabled = false;
            }
        }
        #endregion
    }
}
