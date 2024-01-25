using System.Threading.Tasks;
using System.Collections.Generic;
using MenuAPI;
using CitizenFX.Core;
using Red.Common.Client;
using Red.InteractionMenu.Client.Menus.SubMenus;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

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
            Menu menu = new($"{Constants.communityName} Menu", "~b~Police Menu");
            MenuItem sceneManageBtn = new("Scene Management");

            menu.AddMenuItem(sceneManageBtn);
            MenuController.BindMenuItem(GetMenu(), SceneManagement.GetMenu(), sceneManageBtn);

            menu.AddMenuItem(new MenuListItem("Restrainment", new List<string> { "Rear Cuff", "Front Cuff", "Rear Ziptie", "Front Ziptie" }, 0));
            menu.AddMenuItem(new MenuListItem("Hands On", new List<string> { "Grab", "Seat", "Unseat"}, 0));
            menu.AddMenuItem(new MenuListItem("Loadout", new List<string> { "Default", "SWAT" }, 0));

            menu.AddMenuItem(new("Refill Taser Cartridges"));
            menu.AddMenuItem(new MenuListItem("Weapon Retention", new List<string> { "Carbine", "Shotgun" }, 0));
            menu.AddMenuItem(new("Conduct CPR"));

            menu.AddMenuItem(new("Toggle Shield"));

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
                PlayerPed.PlayAnim("mini@cpr@char_a@cpr_de", "cpr_success", 8.0f, 1.0f, -1, AnimationFlags.StayInEndFrame, 0.0f, false, false, false);
            }
            else if (item == "Toggle Shield")
            {
                ToggleShield();
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
            PlayerPed.PlayAnim(animDict, "0", 8.0f, 8.0f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation, 0.0f, false, false, false);

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
            TriggerEvent("Menu:Client:enableShield");

            if (!hadPistolForShield)
            {
                DeleteEntity(ref propEntity);
                ClearAllTasksImmediately();

                SetWeaponAnimationOverride(PlayerPed.Handle, (uint)GetHashKey("Default"));
                SetCurrentPedWeapon(PlayerPed.Handle, (uint)WeaponHash.Unarmed, true);

                PlayerPed.Weapons.Remove(WeaponHash.CombatPistol);
                SetEnableHandcuffs(PlayerPed.Handle, false);

                hadPistolForShield = false;
                shieldEnabled = false;
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Menu:Client:enableShield")]
        private void OnEnabledShield()
        {
            if (shieldEnabled)
            {
                Tick += PoliceShield_Tick;
            }
            else
            {
                Tick -= PoliceShield_Tick;
            }
        }
        #endregion

        #region Ticks
        private async Task PoliceShield_Tick()
        {
            Game.DisableControlThisFrame(0, Control.Enter);
            Game.DisableControlThisFrame(0, Control.VehicleExit);
        }
        #endregion
    }
}
