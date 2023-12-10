using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static CitizenFX.Core.UI.Screen;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.WeaponControl.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool safetyEnabled;
        protected int taserCartridges = 2;
        protected readonly List<WeaponHash> weaponsWithSafetyEnabled = new();
        protected readonly Dictionary<WeaponHash, int> weaponsCurrentFireMode = new();

        protected readonly IReadOnlyList<WeaponGroup> weaponGroupsWithSafeties = new List<WeaponGroup>
        {
            WeaponGroup.Pistol, WeaponGroup.SMG, WeaponGroup.AssaultRifle, WeaponGroup.MG, WeaponGroup.Shotgun, WeaponGroup.Sniper, WeaponGroup.Heavy
        };

        protected readonly IReadOnlyList<WeaponHash> automaticWeapons = new List<WeaponHash>
        {
            WeaponHash.MicroSMG, WeaponHash.MachinePistol, WeaponHash.MiniSMG, WeaponHash.SMG, WeaponHash.SMGMk2, WeaponHash.AssaultSMG, WeaponHash.CombatPDW, WeaponHash.MG, WeaponHash.CombatMG, WeaponHash.CombatMGMk2, WeaponHash.Gusenberg, WeaponHash.AssaultRifle, WeaponHash.AssaultRifleMk2, WeaponHash.CarbineRifle, WeaponHash.CarbineRifleMk2, WeaponHash.AdvancedRifle, WeaponHash.SpecialCarbine, WeaponHash.SpecialCarbineMk2, WeaponHash.BullpupRifle, WeaponHash.BullpupRifleMk2, WeaponHash.CompactRifle
        };
        #endregion

        #region Constructor
        public ClientMain() => RequestTextureDict("mpweaponsgang0");
        #endregion

        #region Ticks
        [Tick]
        private async Task ChangeFireModeTick()
        {
            Weapon weapon = Game.PlayerPed.Weapons.Current;

            safetyEnabled = true;

            if (!CanSafetyWeapon(weapon) || weapon == (WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4"))
            {
                return;
            }

            if (weaponsWithSafetyEnabled.Contains(weapon.Hash))
            {
                Game.Player.DisableFiringThisFrame();

                if (Game.IsControlJustPressed(0, Control.VehicleAttack) || Game.IsControlJustPressed(0, Control.Attack))
                {
                    ShowNotification("You still have the safety on! Press ~o~K~s~ to flick it off.", true);
                    safetyEnabled = true;
                }
            }

            SetPlayerCanDoDriveBy(Game.Player.Handle, !weaponsWithSafetyEnabled.Contains(weapon.Hash) && (weapon.Hash != WeaponHash.StunGun || taserCartridges > 0));

            if (IsInputDisabled(2) && Game.IsControlJustPressed(0, Control.ReplayShowhotkey))
            {
                if (!weaponsWithSafetyEnabled.Contains(weapon.Hash))
                {
                    weaponsWithSafetyEnabled.Add(weapon.Hash);
                }
                else
                {
                    weaponsWithSafetyEnabled.Remove(weapon.Hash);
                }
            }

            if (IsWeaponAutomatic(weapon) && !weaponsWithSafetyEnabled.Contains(weapon.Hash))
            {
                if (!weaponsCurrentFireMode.ContainsKey(weapon.Hash))
                {
                    weaponsCurrentFireMode.Add(weapon.Hash, 0);
                }

                if (IsInputDisabled(2) && Game.IsControlJustPressed(0, Control.CinematicSlowMo))
                {
                    weaponsCurrentFireMode[weapon.Hash] = (weaponsCurrentFireMode[weapon.Hash] + 1) % 3;
                }

                if (weaponsCurrentFireMode[weapon.Hash] > 0 && Game.IsControlJustPressed(0, Control.Attack))
                {
                    int delayTime = weaponsCurrentFireMode[weapon.Hash] == 1 ? 300 : 0;

                    await Delay(delayTime);

                    while (Game.IsControlPressed(0, Control.Attack))
                    {
                        Game.Player.DisableFiringThisFrame();

                        await Delay(0);
                    }
                }
            }
        }

        [Tick]
        private async Task ShowCurrentModeTick()
        {
            Weapon weapon = Game.PlayerPed.Weapons.Current;

            if (weapon == (WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4"))
            {
                await Delay(1000);
                return;
            }

            if (!Hud.IsVisible || (!IsWeaponAutomatic(weapon) && !CanSafetyWeapon(weapon)))
            {
                await Delay(1000);
                return;
            }

            SetScriptGfxAlign(0, 84);
            SetScriptGfxAlignParams(0f, 0f, 0f, 0f);

            if (weapon != WeaponHash.StunGun && weapon != (WeaponHash)Game.GenerateHashASCII("WEAPON_PROLASER4"))
            {
                if (weaponsWithSafetyEnabled.Contains(weapon.Hash))
                {
                    DrawText2d(0.205f, 2.25f, 0.37f, "~r~X", 250, 250, 120);
                }
                else if (IsWeaponAutomatic(weapon))
                {
                    int fireMode = weaponsCurrentFireMode.TryGetValue(weapon.Hash, out int mode) ? mode : 0;

                    switch (fireMode)
                    {
                        case 1:
                            DrawText2d(0.105f, 2.25f, 0.37f, "BURST", 250, 250, 120);
                            break;
                        case 2:
                            DrawText2d(0.105f, 2.25f, 0.37f, "SINGLE", 250, 250, 120);
                            break;
                        default:
                            DrawText2d(0.105f, 2.25f, 0.37f, "AUTO", 250, 250, 120);
                            break;
                    }
                }
                // mpweaponsgang0", "w_ar_carbinerifle_mag1"
                DrawSprite("mpweaponsgang0", "w_ar_carbinerifle_mag1", (1 / GetSafeZoneSize() / 3.0f) - 0.348f + 0.19f, GetSafeZoneSize() - GetTextScaleHeight(1.3f, 4) - 0.0965f, 0.034f, 0.034f, 0.0f, 200, 200, 200, 255);
            }
            else
            {
                DrawText2d(1.27f, 0.105f, 0.37f, weaponsWithSafetyEnabled.Contains(weapon.Hash) ? "~r~X" : $" {taserCartridges}", 250, 250, 120);
                DrawSprite("mpweaponsgang0", "w_pi_stungun", (1 / GetSafeZoneSize() / 3.0f) - 0.348f + 0.19f, GetSafeZoneSize() - GetTextScaleHeight(1.3f, 4) - 0.0965f, 0.034f, 0.034f, 0.0f, 200, 200, 200, 255);
            }

            ResetScriptGfxAlign();
        }

        [Tick]
        private async Task TaserTick()
        {
            Weapon taser = Game.PlayerPed.Weapons.Current;
            SetPedMinGroundTimeForStungun(Game.PlayerPed.Handle, 10000);

            if (taser is null || taser != WeaponHash.StunGun)
            {
                await Delay(500);
                return;
            }

            if (Game.PlayerPed.IsShooting)
            {
                taserCartridges--;
            }

            if (taserCartridges <= 0)
            {
                Game.Player.DisableFiringThisFrame();

                if (Game.IsControlPressed(0, Control.VehicleMouseControlOverride))
                {
                    ShowNotification("You've ran out of taser cartridges! Use ~o~~h~/refill~h~~s~ near an emergency vehicle to refill.", true);
                }
            }
        }

        #endregion

        #region Commands

        [Command("refill")]
        private void OnRefillCommand()
        {
            Vehicle closestVeh = GetClosestVehicleToPlayer(1f);

            if (Game.PlayerPed.IsInPoliceVehicle || closestVeh?.ClassType == VehicleClass.Emergency)
            {
                taserCartridges = 2;
                ShowNotification("~g~~h~Success~h~~s~: Refilled taser cartridges.", true);
            }
            else
            {
                ShowNotification("~r~~h~Error~h~~s~: You must be in or near a police cruiser to use this.", true);
            }
        }

        #endregion

        #region Methods
        private bool CanSafetyWeapon(Weapon weapon) => weapon.Hash == WeaponHash.StunGun || weaponGroupsWithSafeties.Contains(weapon.Group);
        private bool IsWeaponAutomatic(Weapon weapon) => automaticWeapons.Contains(weapon);
        #endregion
    }
}