using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Holster.Client
{
    public class ClientMain : BaseScript
    {
        #region Varibles
        protected bool holster;
        protected bool weaponHolstered = true;
        protected WeaponHash previousWeapon = WeaponHash.Unarmed;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<WeaponHash> holsterableWeapons = new List<WeaponHash>
        {
             WeaponHash.Pistol, WeaponHash.PistolMk2, WeaponHash.CombatPistol, (WeaponHash)Game.GenerateHashASCII("WEAPON_GLOCK20"), WeaponHash.HeavyPistol, WeaponHash.Revolver
        };

        protected readonly IReadOnlyList<Control> selectWeaponControls = new List<Control>
        {
            Control.SelectWeapon, Control.SelectWeaponUnarmed, Control.SelectWeaponMelee, Control.SelectWeaponHandgun, Control.SelectWeaponShotgun, Control.SelectWeaponSmg, Control.SelectWeaponAutoRifle,  Control.SelectWeaponSniper, Control.SelectWeaponHeavy,  Control.SelectWeaponSpecial
        };
        #endregion

        #region Constructor
        public ClientMain() => RegisterKeyMapping("handonholster", "Hand On Holster", "keyboard", "z");
        #endregion

        #region Commands
        [Command("handonholster")]
        private void HandOnHolsterCommand()
        {
            if (CannotDoAction(PlayerPed) || PlayerPed.IsInVehicle())
            {
                return;
            }

            if (PlayerPed.Style[PedComponents.Special1].Index is not 5 and not 6)
            {
                return;
            }

            holster = !holster;

            if (holster)
            {
                if (!weaponHolstered)
                {
                    ToggleWeaponHolstered();
                    return;
                }

                Tick += DisableShooting;

                SetFirstHolsterableWeapon();

                SetPedCurrentWeaponVisible(PlayerPed.Handle, false, false, false, false);

                PlayerPed.Task.PlayAnimation("reaction@intimidation@cop@unarmed", "intro", 2f, -1, (AnimationFlags)50);
            }
            else
            {
                if (weaponHolstered)
                {
                    PlayerPed.Weapons.Select(WeaponHash.Unarmed, true);
                }

                Tick -= DisableShooting;

                PlayerPed.Task.ClearAnimation("reaction@intimidation@cop@unarmed", "intro");
            }
        }
        #endregion

        #region Methods
        private async void ToggleWeaponHolstered(bool animation = true)
        {
            if (PlayerPed.Style[PedComponents.Special1].Index is not 6 and not 5)
            {
                return;
            }

            weaponHolstered = !weaponHolstered;

            Tick += DisableShooting;

            if (weaponHolstered)
            {
                if (animation)
                {
                    PlayerPed.Task.PlayAnimation("rcmjosh4", "josh_leadout_cop2", 2f, 2.5f, -1, (AnimationFlags)48, 0f);

                    for (float i = 0.26f; i >= 0.0f; i -= 0.1f)
                    {
                        SetEntityAnimSpeed(PlayerPed.Handle, "rcmjosh4", "josh_leadout_cop2", 0.3f);
                        SetEntityAnimCurrentTime(PlayerPed.Handle, "rcmjosh4", "josh_leadout_cop2", i);

                        await Delay(0);
                    }

                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "holster", 0.1f);
                }

                holster = false;

                if (animation)
                {
                    await Delay(500);
                }

                PlayerPed.Style[PedComponents.Special1].Index = 6;

                if (animation)
                {
                    PlayerPed.Weapons.Select(WeaponHash.Unarmed, true);
                    PlayerPed.Task.ClearAnimation("rcmjosh4", "josh_leadout_cop2");
                }
            }
            else
            {
                if (animation)
                {
                    PlayerPed.Task.PlayAnimation("rcmjosh4", "josh_leadout_cop2", 2f, 2.5f, -1, (AnimationFlags)48, 0f);

                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "unholster", 0.1f);

                    await Delay(200);
                }

                SetFirstHolsterableWeapon();

                if (animation)
                {
                    await Delay(50);
                }

                SetPedComponentVariation(PlayerPed.Handle, 7, 5, 0, 2);

                SetPedCurrentWeaponVisible(PlayerPed.Handle, true, false, false, false);

                if (animation)
                {
                    await Delay(150);
                }

                if (holster)
                {
                    HandOnHolsterCommand();
                }

                if (animation)
                {
                    PlayerPed.Task.ClearAnimation("rcmjosh4", "josh_leadout_cop2");
                }
            }

            Tick -= DisableShooting;
        }

        private void SetFirstHolsterableWeapon()
        {
            foreach (WeaponHash weaponHash in holsterableWeapons)
            {
                if (PlayerPed.Weapons.HasWeapon(weaponHash))
                {
                    PlayerPed.Weapons.Select(weaponHash, true);
                    break;
                }
            }
        }

        private async Task WaitForWeaponToSwitch()
        {
            WeaponHash previousWeaponHash = PlayerPed.Weapons.Current.Hash;

            int tempTimer = Game.GameTime;

            while (previousWeaponHash == PlayerPed.Weapons.Current.Hash)
            {
                if (Game.GameTime - tempTimer > 250)
                {
                    break;
                }

                await Delay(0);
            }
        }

        private async Task DisableShooting() => Game.Player.DisableFiringThisFrame();

        #endregion

        #region Ticks
        [Tick]
        private async Task HolsterTick()
        {
            if (PlayerPed.IsInVehicle())
            {
                return;
            }

            if (holster)
            {
                Game.DisableControlThisFrame(0, Control.Reload);

                if ((Game.IsControlJustPressed(0, Control.Aim) || Game.IsControlJustPressed(0, Control.Attack)) && holsterableWeapons.Contains(PlayerPed.Weapons.Current))
                {
                    ToggleWeaponHolstered();
                    await Delay(1000);
                    return;
                }
            }

            if (selectWeaponControls.Any(c => Game.IsControlJustReleased(0, c)))
            {
                await WaitForWeaponToSwitch();

                if (weaponHolstered)
                {
                    if (holsterableWeapons.Contains(PlayerPed.Weapons.Current.Hash))
                    {
                        ToggleWeaponHolstered();
                    }
                }
                else if (PlayerPed.Weapons.Current.Hash == WeaponHash.Unarmed || holsterableWeapons.Contains(PlayerPed.Weapons.Current.Hash))
                {
                    ToggleWeaponHolstered();
                }
                else
                {
                    if (holster)
                    {
                        HandOnHolsterCommand();
                    }

                    ToggleWeaponHolstered(false);

                    PlayerPed.Task.ClearSecondary();
                    weaponHolstered = true;
                }
            }

            previousWeapon = PlayerPed.Weapons.Current;
        }
        #endregion
    }
}