using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Essentials.Server
{
    public class ServerMain : BaseScript
    {
        #region GSR
        [EventHandler("Essentials:Server:submitGsrTest")]
        private void OnSubmitGsrTest([FromSource] Player player, int testPlayer)
        {
            Player testedPlayer = Players[testPlayer];
            testedPlayer?.TriggerEvent("Essentials:Client:doGsrTest");
        }

        [EventHandler("Essentials:Server:returnGsrTest")]
        private void OnReturnGsrTest(bool shotRecently, string testerPlayerId)
        {
            Player testerPlayer = Players[testerPlayerId];
            testerPlayer.TriggerEvent("Essentials:Client:showNotification", shotRecently ? "Sample from swab comes back ~g~~h~positive~h~~s~." : "Sample from swab comes back ~o~~h~negative~h~~s~.");
        }
        #endregion

        #region Vehicles
        [EventHandler("Essentials:Server:deleteVehicle")]
        private void OnDeleteVehicle(int netId)
        {
            Entity vehicle = Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                return;
            }

            DeleteEntity(vehicle.Handle);
        }

        [EventHandler("Essentials:Server:doorAction")]
        private void OnDoorAction(int netId, int doorIndex, bool open)
        {
            Entity vehicle = Entity.FromNetworkId(netId);

            if (vehicle is null)
            {
                return;
            }

            vehicle.Owner.TriggerEvent("Essentials:Client:doorAction", netId, doorIndex, open);
        }

        [EventHandler("SlashTires:Server:slashTires")]
        private void OnSlashTires(int networkId, int tireIndex)
        {
            Entity vehicle = Entity.FromNetworkId(networkId);

            if (vehicle is null)
            {
                return;
            }

            TriggerClientEvent("SlashTires:Client:slashTires", networkId, tireIndex);
        }
        #endregion
    }
}