using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.ShotSpotter.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Character currentCharacter;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly List<WeaponGroup> whitelistedWeapons = new()
        {
            WeaponGroup.Pistol, WeaponGroup.SMG, WeaponGroup.AssaultRifle, WeaponGroup.MG, WeaponGroup.Shotgun, WeaponGroup.Sniper
        };

        protected readonly IReadOnlyList<Vector3> ignoreShotLocations = new List<Vector3>
        {
            new(13.35f, -1097.08f, 29.83f),
            new(821.51f, -2163.73f, 29.66f)
        };
        #endregion

        #region Methods
        protected bool IsInIgnoredLocations()
        {
            foreach (Vector3 pos in ignoreShotLocations)
            {
                if (Vector3.DistanceSquared(PlayerPed.Position, pos) < 20f)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Event Handlers
        [EventHandler("Framework:Client:characterSelected")]
        private void OnCharacterSelect(string json) => currentCharacter = Json.Parse<Character>(json);

        [EventHandler("ShotSpotter:Client:shotSpotterNotify")]
        private async void OnShotSpotterNotify(Vector3 plyPos, string postal, string zoneName, string caliber)
        {
            if (currentCharacter is not null && currentCharacter.Department == "Civ" || currentCharacter is not null && currentCharacter.Department == "LSFD")
            {
                return;
            }

            Blip blip = World.CreateBlip(plyPos);
            blip.Sprite = (BlipSprite)161;
            SetBlipDisplay(blip.Handle, 3);
            blip.Color = (BlipColor)1;
            blip.Name = "ShotSpotter Alert";

            Screen.ShowNotification($"~o~~h~ShotSpotter~h~~s~: {(caliber is not null ? caliber + " arms fire detected" : "Gunfire detected")} near {postal}, {zoneName}.");

            await Delay(90000);

            blip.Delete();
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ShotSpotterTick()
        {
            if (PlayerPed.IsAlive || currentCharacter is not null && currentCharacter.Department != "Civ")
            {
                await Delay(1000);
                return;
            }

            if (!IsPedArmed(PlayerPed.Handle, 4))
            {
                await Delay(500);
                return;
            }

            if (whitelistedWeapons.Contains(PlayerPed.Weapons.Current.Group) && PlayerPed.IsShooting)
            {
                if (PlayerPed.Position.Y > 1000f || IsInIgnoredLocations())
                {
                    await Delay(3000);
                    return;
                }

                Vector3 plyrPos = PlayerPed.Position;
                string postal = Exports["postals"].GetClosestPostal(plyrPos);
                string zoneName = GetLabelText(GetNameOfZone(plyrPos.X, plyrPos.Y, plyrPos.Z));
                string caliber = PlayerPed.Weapons.Current.Group == WeaponGroup.Pistol ? "Small" : "Large";

                TriggerServerEvent("ShotSpotter:Server:shotSpotterNotify", plyrPos, postal, zoneName, caliber);
                await Delay(15000);
            }
        }
        #endregion
    }

    public class Character
    {
        public long CharacterId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DoB { get; set; }
        public string Gender { get; set; }
        public int Cash { get; set; }
        public int Bank { get; set; }
        public string Department { get; set; }
    }
}