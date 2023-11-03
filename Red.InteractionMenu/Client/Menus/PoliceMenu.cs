using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MenuAPI;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.InteractionMenu.Client.Variables;
using Red.InteractionMenu.Client.Submenus;
using CitizenFX.Core.UI;

namespace Red.InteractionMenu.Client.Menus
{
    public class PoliceMenu : BaseScript
    {
        protected static Vehicle closestVehicle = GetClosestVehicleToPlayer(1f); 

        public static Menu GetMenu()
        {
            ReadConfigFile();
            Menu menu = new(menuName, subMenuPoliceName);

            MenuItem sceneButton = new(subMenuSceneManagementName) { Label = ForwardArrow };
            MenuController.BindMenuItem(menu, PropSpawnerMenu.GetMenu(), sceneButton);
            
            menu.AddMenuItem(sceneButton);
            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Cuff", "Front Cuff" }, 0));
            menu.AddMenuItem(new MenuListItem("Hands On", new List<string> { "Grab", "Seat", "Unseat" }, 0));
            menu.AddMenuItem(new MenuListItem("Deploy Spikes Strips", new List<string> { "2", "3", "4" }, 0));
            menu.AddMenuItem(new("Remove Spikes Strips"));
            menu.AddMenuItem(new MenuListItem("Loadout", new List<string> { "Default", "SWAT" }, 0));
            menu.AddMenuItem(new MenuListItem("Weapon Retention", new List<string> { "Toggle Carbine", "Toggle Shotgun", "Toggle Beanbag Shotgun", "Toggle Shield" }, 0));
            menu.AddMenuItem(new("Toggle LiDAR"));
            menu.AddMenuItem(new("Toggle Radar Remove"));
            menu.AddMenuItem(new("Refill Taser Cartridges"));
            menu.AddMenuItem(new("~o~Back") { Label = BackArrow, Description = "Goes back one menu." });
            menu.AddMenuItem(new("~r~Close") { Description = $"Closes all {menuName} menus." });

            menu.OnListItemSelect += Menu_OnListItemSelect;
            menu.OnItemSelect += Menu_OnItemSelect;
            return menu;
        }

        private static void Menu_OnItemSelect(Menu menu, MenuItem menuItem, int itemIndex)
        {
            string item = menuItem.Text;

            if (item == "~o~Back")
            {
                menu.GoBack();
            }
            else if (item == "~r~Close")
            {
                MenuController.CloseAllMenus();
            }
            else if (item == "Remove Spikes Strips")
            {
                // Remove Spikes Event
            }
            else if (item == "Toggle LiDAR")
            {
                if (Game.PlayerPed.IsInPoliceVehicle || closestVehicle?.ClassType == VehicleClass.Emergency)
                {
                    if (Game.PlayerPed.Weapons.HasWeapon((WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4")))
                    {
                        Screen.ShowNotification("~g~~h~Success~h~~s~: You've put the LiDAR gun back on the passenger seat.", true);
                    }
                    else
                    {
                        Screen.ShowNotification("~g~~h~Success~h~~s~: You've taken the LiDAR gun off the passenger seat.", true);
                    }

                    ExecuteCommand("lidarweapon");
                }
            }
            else if (Game.PlayerPed.IsInPoliceVehicle)
            {
                if (Game.PlayerPed.IsInPoliceVehicle)
                {
                    TriggerEvent("wk:openRemote");
                }
                else
                {
                    Screen.ShowNotification($"~r~~h~Error~h~~s~: You must be in a police cruiser to use this.", true);
                }
            }
            else if (item == "Refill Taser Cartridges")
            {
                ExecuteCommand("refill");
            }
            else
            {
                Screen.ShowNotification($"~r~~h~Error~h~~s~: You must be in or near a police cruiser to use this.", true);
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
            else if (item == "Deploy Spikes Strips")
            {
                // Deploy Spikes with count.
            }
            else if (item == "Loadout")
            {
                switch (selectedIndex)
                {
                    case 0:
                        Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 60, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 9999, false, false);
                        Game.PlayerPed.Weapons.Give(WeaponHash.Flashlight, 9999, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.Nightstick, 9999, false, true);
                        ExecuteCommand("lidarweapon");
                        Game.PlayerPed.Weapons[WeaponHash.CombatPistol].Ammo = Game.PlayerPed.Weapons[WeaponHash.CombatPistol].MaxAmmoInClip * 3;
                        Game.PlayerPed.Weapons[WeaponHash.CombatPistol].Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
                        Screen.ShowNotification("~g~~h~Success~h~~s~: You've equipped the standard duty loadout.", true);
                        break;

                    case 1:
                        Game.PlayerPed.Weapons.Give(WeaponHash.CombatPistol, 80, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.StunGun, 9999, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.CarbineRifle, 210, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.Flashlight, 9999, false, true);
                        Game.PlayerPed.Weapons.Give(WeaponHash.Knife, 9999, false, true);
                        Game.PlayerPed.Weapons[WeaponHash.CombatPistol].Ammo = Game.PlayerPed.Weapons[WeaponHash.CombatPistol].MaxAmmoInClip * 3;
                        Game.PlayerPed.Weapons[WeaponHash.CarbineRifle].Ammo = Game.PlayerPed.Weapons[WeaponHash.CarbineRifle].MaxAmmoInClip * 3;
                        Game.PlayerPed.Weapons[WeaponHash.CombatPistol].Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        Game.PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        Game.PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtArFlsh].Active = true;
                        Game.PlayerPed.Weapons[WeaponHash.CarbineRifle].Components[WeaponComponentHash.AtScopeMedium].Active = true;
                        Game.PlayerPed.Weapons.Select(WeaponHash.Unarmed);
                        Screen.ShowNotification("~g~~h~Success~h~~s~: You've equipped the SWAT duty loadout.", true);
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
    }
}
