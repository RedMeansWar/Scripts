using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client
{
    public class Variables : BaseScript
    {
        public static string menuName, subMenuPoliceName, subMenuFireName, subMenuVehicleName, subMenuCivilianName, subMenuSceneManagementName;
        public static string ForwardArrow = "→→→";
        public static string BackArrow = "←←←";
        public static int openKey;

        public static void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("InteractionMenu", "MenuName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                openKey = loaded["InteractionMenu"]["OpenMenuKey"].IntValue;
                menuName = loaded["InteractionMenu"]["MenuName"].StringValue;
                subMenuPoliceName = loaded["InteractionMenu"]["PoliceMenuName"].StringValue;
                subMenuFireName = loaded["InteractionMenu"]["FireMenuName"].StringValue;
                subMenuVehicleName = loaded["InteractionMenu"]["VehicleMenuName"].StringValue;
                subMenuCivilianName = loaded["InteractionMenu"]["CivilianMenuName"].StringValue;
                subMenuSceneManagementName = loaded["InteractionMenu"]["SceneMenuName"].StringValue;
            }
            else
            {
                Debug.WriteLine($"[InteractionMenu]: Config file has not been configured correctly.");
            }
        }
        // Forked from vMenu Common Functions (https://github.com/TomGrobbe/vMenu/blob/master/vMenu/CommonFunctions.cs#L1755)
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title string.
            var spacer = "\t";
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");

            // Display the input box.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0);
            // Wait for a result.
            while (true)
            {
                var keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 3: // not displaying input field anymore somehow
                    case 2: // cancelled
                        return null;
                    case 1: // finished editing
                        return GetOnscreenKeyboardResult();
                    default:
                        await Delay(0);
                        break;
                }
            }
        }

        public static async Task<string> GetUserInput(string text, int maxInputLength) => await GetUserInput(null, text, maxInputLength);

        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(Game.PlayerPed.Position, Game.PlayerPed.Position, radius, (IntersectOptions)10, Game.PlayerPed);

            return raycast.HitEntity as Vehicle;
        }

        public static void WeaponSystem(WeaponHash hash)
        {
            Vehicle closestVeh = GetClosestVehicleToPlayer(1f);

            string gun = hash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                _ => "gun"
            };

            if (Game.PlayerPed.IsInPoliceVehicle || closestVeh?.ClassType == VehicleClass.Emergency)
            {
                Weapon playerWeapon = Game.PlayerPed.Weapons[hash];

                if (playerWeapon is not null)
                {
                    Game.PlayerPed.Weapons.Remove(playerWeapon);
                    Screen.ShowNotification($"~g~~h~Success~h~~s~: You've unequipped and locked your {gun}.", true);
                }
                else
                {
                    playerWeapon = Game.PlayerPed.Weapons.Give(hash, 0, true, true);
                    playerWeapon.Ammo = playerWeapon.MaxAmmoInClip * 3;
                    playerWeapon.Components[WeaponComponentHash.AtArFlsh].Active = true;

                    if (hash == WeaponHash.CarbineRifle)
                    {
                        playerWeapon.Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtScopeMedium].Active = true;
                    }

                    Screen.ShowNotification($"~g~~h~Success~h~~s~: You've unlocked and equipped your {gun}", true);
                }
            }
            else
            {
                Screen.ShowNotification($"~r~~h~Error~h~~s~: You must be in or near a police cruiser to use this.", true);
            }
        }
    }
}
