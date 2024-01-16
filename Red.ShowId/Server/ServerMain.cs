using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.ShowId.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("ShowId:Server:showId")]
        private void OnShowId([FromSource] Player player, int targetedId)
        {
            Player targetPlayer = Players[targetedId];
            targetPlayer?.TriggerEvent("ShowId:Client:showId", player.Handle);
        }
    }
}