using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;

namespace Red.WeaponControl.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected int taserCartridges = 2;
        protected readonly List<WeaponHash> weaponsWithSafetyEnabled = new();
        protected readonly Dictionary<WeaponHash, int> weaponsCurrentFireMode = new();

        protected readonly IReadOnlyList<WeaponGroup> weaponGroupsWithSafeties = new List<WeaponGroup>
        {
            WeaponGroup.Pistol, WeaponGroup.SMG, WeaponGroup.AssaultRifle, WeaponGroup.MG, WeaponGroup.Shotgun, WeaponGroup.Sniper, WeaponGroup.Heavy
        };
        #endregion

        #region Constructor
        public ClientMain() => RequestTextureDictionary("mpweaponsgang0");
        #endregion
    }
}