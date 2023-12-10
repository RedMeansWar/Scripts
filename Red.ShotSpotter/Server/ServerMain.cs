using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.ShotSpotter.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected Random random = new();
        #endregion

        #region Event Handlers
        [EventHandler("ShotSpotter:Server:shotSpotterNotify")]
        private async void OnShotSpotterNotify([FromSource] Player player, Vector3 playerPos, string postal, string zoneName, string caliber)
        {
            await Delay(random.Next(10000, 25000));
            TriggerClientEvent("ShotSpotter:Client:shotSpotterNotify", playerPos, postal, zoneName, caliber);
        }
        #endregion
    }
}