using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.GsrTest.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected int lastShot;
        protected bool shotRecently;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly IReadOnlyList<WeaponHash> blacklistedWeapons = new List<WeaponHash>
        {
            WeaponHash.FireExtinguisher, WeaponHash.Snowball, WeaponHash.PetrolCan, WeaponHash.Ball, WeaponHash.StunGun, WeaponHash.Molotov, WeaponHash.Flare
        };
        #endregion

        #region Commands
        [Command("gsrtest")]
        private void GsrTestCommand()
        {
            Player closestPlayer = GetClosestPlayer();

            if (closestPlayer is null)
            {
                ErrorNotification("You must be closer to the player you wish to test.");
                return;
            }

            TriggerServerEvent("GsrTest:Server:doGsrTest", closestPlayer.ServerId);
        }

        [Command("gsrclear")]
        private void GsrClearCommand()
        {
            if (shotRecently)
            {
                CleanOffGSR();
                AlertNotification("You've wiped your hands and arms on your clothes.");
            }
            else
            {
                AlertNotification("You haven't shot recently.");
            }
        }

        [Command("gsrwipe")]
        private void GsrWhipeCommand() => ExecuteCommand("gsrclear");
        #endregion

        #region Methods
        private void CleanOffGSR()
        {
            PlayerPed.ClearBloodDamage();
            ClearPedEnvDirt(PlayerPed.Handle);
            PlayerPed.ResetVisibleDamage();
            shotRecently = false;
        }
        #endregion

        #region Event Handlers
        [EventHandler("GsrTest:Client:doGsrTest")]
        private void OnDoGsrTest(string testerId) => TriggerServerEvent("GsrTest:Server:returnGsrResult", shotRecently, testerId);

        [EventHandler("GsrTest:Client:showClientNotification")]
        private void OnShowClientNotification(string notification) => Screen.ShowNotification(notification, true);
        #endregion

        #region Ticks
        [Tick]
        private async Task GsrTestTick()
        {
            if (PlayerPed.IsShooting && !blacklistedWeapons.Contains(PlayerPed.Weapons.Current.Hash))
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
                CleanOffGSR();
                AlertNotification("You've ~b~~h~washed off~h~~s~ the evidence of gunshot residue on yourself.");
            }
        }
        #endregion
    }
}