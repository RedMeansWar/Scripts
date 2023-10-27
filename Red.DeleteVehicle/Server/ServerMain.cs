using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.DeleteVehicle.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("DeleteVehicle:Server:deleteVehicle")]
        private void OnDeleteVehicle(int netId)
        {
            Entity vehicle = Entity.FromHandle(netId);

            if (vehicle is null)
            {
                return;
            }

            DeleteEntity(vehicle.Handle);
        }
    }
}