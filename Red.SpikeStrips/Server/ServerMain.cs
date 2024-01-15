using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.SpikeStrips.Server
{
    public class ServerMain : BaseScript
    {
        [EventHandler("Spikes:Server:spawnSpikes")]
        private void OnSpawnSpikes([FromSource] Player player, int spikeAmount)
        {
            player.TriggerEvent("Spikes:Client:spawnSpikes", spikeAmount);
            Debug.WriteLine($"Player: {player.Name} spawned spikes with an amount of {spikeAmount}");
        }

        [EventHandler("Spikes:Server:deleteAllSpikes")]
        private void OnDeleteSpikes([FromSource] Player player)
        {
            TriggerClientEvent("Spikes:Client:deleteSpikes");
            Debug.WriteLine($"Player: {player.Name} deleted spikes at position: {player.Character.Position}");
        }
    }
}