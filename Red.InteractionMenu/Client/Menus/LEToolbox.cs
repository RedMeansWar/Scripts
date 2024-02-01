using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.InteractionMenu.Client.Menus
{
    internal class LEToolbox
    {
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

        private void ToggleShield()
        {

        }
        #endregion
    }
}
