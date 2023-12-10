using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Irp.Essentials.Client.Police
{
    internal class Gsr : BaseScript
    {
        #region Variables
        protected int lastShot;
        protected bool shotRecently;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<WeaponHash> whitelistedWeapons = new List<WeaponHash>
        {
            WeaponHash.FireExtinguisher, WeaponHash.Snowball, WeaponHash.PetrolCan, WeaponHash.Ball, WeaponHash.StunGun, WeaponHash.Molotov, WeaponHash.Flare
        };
        #endregion

        #region Commands
        [Command("gsrclear")]
        private void GsrClearCommand()
        {
            if (shotRecently)
            {
                CleanOffGsr();
                AlertNotification("You've wiped your hands and arms on your clothes.");
            }
            else
            {
                AlertNotification("You haven't shot recently.");
            }
        }

        [Command("gsrwhip")]
        private void GsrWhipeCommand() => GsrClearCommand();

        [Command("gsr")]
        private void GsrCommand() => GsrTestCommand();

        [Command("testgsr")]
        private void TestGsrCommand() => GsrTestCommand();

        [Command("gsrtest")]
        private void GsrTestCommand()
        {
            Player closestPlayer = GetClosestPlayer();

            if (closestPlayer is null)
            {
                ErrorNotification("You must be closer to the player you wish to test.");
                return;
            }

            TriggerServerEvent("Essentials:Server:submitGsrTest", closestPlayer.ServerId);
        }
        #endregion

        #region Methods
        private void CleanOffGsr()
        {
            PlayerPed.ClearBloodDamage();
            ClearPedEnvDirt(PlayerPed.Handle);
            PlayerPed.ResetVisibleDamage();
        }
        #endregion

        #region Event Handlers
        [EventHandler("Essentials:Client:doGsrTest")]
        private void OnDoGsrTest(string testerPlayerId) => TriggerServerEvent("Essentials:Server:returnGsrTest", shotRecently, testerPlayerId);
        #endregion

        #region Ticks
        [Tick]
        private async Task GsrTick()
        {
            if (PlayerPed.IsShooting && !whitelistedWeapons.Contains(PlayerPed.Weapons.Current.Hash))
            {
                lastShot = Game.GameTime;
                shotRecently = true;
            }

            if (shotRecently && Game.GameTime - lastShot > 900000)
            {
                shotRecently = false;
            }

            if (PlayerPed.IsInWater && shotRecently)
            {
                CleanOffGsr();
                AlertNotification("You've ~b~~h~washed off~h~~s~ the evidence of gunshot residue on yourself.");
            }
        }
        #endregion
    }
}