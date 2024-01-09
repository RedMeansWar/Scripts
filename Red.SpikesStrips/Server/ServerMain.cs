using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.SpikesStrips.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected uint spikeHash = (uint)GetHashKey("p_ld_stinger_s");
        protected int spikeNumber;
        protected Vector3 spikeSpawnCoords;
        protected Prop spikeProp;

        protected readonly Dictionary<int, int> spikeOwners = new();
        private readonly List<int> spawnedSpikeList = new();
        #endregion

        #region Commands
        [Command("removeallspikes")]
        private void RemoveAllSpikesCommand([FromSource] Player player)
        {
            if (IsPlayerAceAllowed(player.Handle, "spikestrips.removeAllSpikes"))
            {
                Debug.WriteLine($"{player.Name} removed all spikes!");
                DeleteAllSpawnedSpikes();
            }
            else
            {
                Debug.WriteLine($"{player.Name} attempted to execute the 'removeallspikes' command but failed due to inefficient permissions.");
                TriggerClientEvent("chat:addMessage", "[SpikeStrips]", new[] { 255, 0, 0 }, "You can't use this command!");
            }
        }
        #endregion

        #region Methods
        private void DeleteAllSpawnedSpikes()
        {
            spikeNumber = spawnedSpikeList.Count;

            for (int i = 0; i < spawnedSpikeList.Count; i++)
            {
                int handle = spawnedSpikeList[i];
                DeleteEntity(handle);
                spawnedSpikeList.RemoveAt(i--);
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (resourceName != GetCurrentResourceName() || spawnedSpikeList.Count == 0)
            {
                return;
            }

            DeleteAllSpawnedSpikes();
        }

        [EventHandler("Spikes:Server:spawnSpikes")]
        private void OnSpawnSpikes([FromSource] Player player, int deploySpikeAmount, Vector3 fowardVector, List<object> heights)
        {
            float heading = player.Character.Heading;

            for (int i = 0; i < deploySpikeAmount; i++)
            {
                spikeSpawnCoords = player.Character.Position + fowardVector * (3.4f + (4.825f * i));
                spikeProp = new(CreateObject((int)spikeHash, spikeSpawnCoords.X, spikeSpawnCoords.Y, spikeSpawnCoords.Z, true, true, true));

                spikeProp.Heading = heading;
                spikeProp.IsPositionFrozen = true;

                spikeOwners.Add(spikeProp.Handle, int.Parse(player.Handle));
            }
        }

        [EventHandler("Spikes:Server:deleteSpikes")]
        private void OnDeleteSpikes([FromSource] Player player)
        {
            List<int> removeHandlers = new();

            foreach (int handle in spawnedSpikeList)
            {
                if (spikeOwners.TryGetValue(handle, out int owner) && owner == int.Parse(player.Handle))
                {
                    DeleteEntity(handle);
                    removeHandlers.Add(handle);
                }
            }

            foreach (int handle in removeHandlers)
            {
                spawnedSpikeList.Remove(handle);
                spikeOwners.Remove(handle);
            }
        }
        #endregion
    }
}