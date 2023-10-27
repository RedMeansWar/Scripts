using System;
using CitizenFX.Core;

namespace Red.VehicleControl.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("VehicleControl:Server:doorAction")]
        private void OnDoorAction(int netId, int doorIndex, bool open)
        {
            Entity vehicle = Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                return;
            }

            vehicle.Owner.TriggerEvent("VehicleControl:Client:doorAction", netId, doorIndex, open);
        }
    }
}