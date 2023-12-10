using System;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.Grab.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("Grab:Server:grabClosestPlayer")]
        private void OnGrabClosestPlayer([FromSource] Player player, int target)
        {
            Player targetPlayer = Players[target];
            targetPlayer?.TriggerEvent("Grab:Client:getGrabbed", player.Handle);
        }

        [EventHandler("Seat:Server:seatAction")]
        private void OnSeatClosestPlayer([FromSource] Player player, int target, int netId, int seat, bool unseat = false)
        {
            Player targetPlayer = Players[target];
            Entity vehicle = Entity.FromNetworkId(netId);

            if (vehicle is null || targetPlayer is null)
            {
                return;
            }

            targetPlayer.TriggerEvent("Seat:Client:seatAction", netId, seat, unseat);
        }

        [EventHandler("Grab:Server:escapeNotify")]
        private void OnEscapeNotify(int serverId, string message)
        {
            Player grabber = Players[serverId];
            grabber?.TriggerEvent("Grab:Client:showClientNotification", message);
        }
    }
}