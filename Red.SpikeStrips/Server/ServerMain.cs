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
        #region Variables
        protected string modelName = "p_ld_stinger_s";
        protected Prop spikeProp;

        protected List<int> spawnSpikesList = new();
        protected Dictionary<int, int> spikeOwners = new();
        #endregion

        #region Event Handlers
        [EventHandler("Spikes:Server:spawnSpikes")]
        private void OnSpawnSpikes([FromSource] Player player, int spikeAmount)
        {
            for (int i = 0; i < spikeAmount; i++)
            {
                player.TriggerEvent("Spikes:Client:spawnSpikes", spikeAmount);
                spawnSpikesList.Add(i);
            }
        }

        [EventHandler("Spikes:Server:deleteSpikes")]
        private void OnDeleteSpikes([FromSource] Player player)
        {

        }

        [EventHandler("playerDropped")]
        private async void OnPlayerDropped([FromSource] Player player, string reason)
        {
            Debug.WriteLine($"{player.Name} dropped for {reason}. Spikes will be deleted in 5 minutes");

            if (player is null)
            {
                await Delay(300000);
                Debug.WriteLine($"{player.Name} has been offline for 5 minutes, deleting...");
                TriggerEvent("Spikes:Server:deleteSpikes", player);
            }
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (resourceName != GetCurrentResourceName() || spawnSpikesList.Count == 0)
            {
                return;
            }

            OnDeleteAllSpikes();
        }
        #endregion

        #region Methods
        [EventHandler("Spikes:Server:removeAllSpikes")]
        private void OnDeleteAllSpikes()
        {
            for (int i = 0; i < spawnSpikesList.Count; i++)
            {
                int handleSpikeDelete = spawnSpikesList[i];
                DeleteEntity(handleSpikeDelete);
                spawnSpikesList.RemoveAt(i--);
            }
        }
        #endregion

        #region Ticks
        private async Task CleanUpAllSpikes()
        {
            await Delay(2700000);

            if (Players.Count() == 0 && spawnSpikesList.Count > 0)
            {
                OnDeleteAllSpikes();
            }
        }
        #endregion
    }
}