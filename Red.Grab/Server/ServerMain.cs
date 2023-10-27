using System;
using CitizenFX.Core;

namespace Red.Grab.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("Grab:Server:grabClosestPlayer")]
        private void OnGrabClosestPlayer([FromSource] Player sender, int target)
        {
            Player targetPlayer = Players[target];

            targetPlayer?.TriggerEvent("Grab:Client:getGrabbed", sender.Handle);
        }

        [EventHandler("Seat:Server:seatAction")]
        private void OnSeatClosestPlayer([FromSource] Player sender, int target, int netId, int seat, bool unseat = false)
        {
            Player targetPlayer = Players[target];
            Entity vehicle = Entity.FromNetworkId(netId);

            if (vehicle is null || targetPlayer is null)
            {
                return;
            }

            targetPlayer.TriggerEvent("Seat:Client:seatAction", netId, seat, unseat);
        }
    }
}