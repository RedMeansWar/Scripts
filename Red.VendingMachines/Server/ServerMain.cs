using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace Red.VendingMachines.Server
{
    public class ServerMain : BaseScript
    {
        #region Variables
        protected readonly ConcurrentDictionary<int, DateTime> resetTimes = new();
        protected readonly bool unlimitedSoda;
        protected readonly int sodaCanCount = 10;
        protected readonly int minutesToReset = 3;
        #endregion

        #region Event Handlers
        [EventHandler("VendingMachine:Server:setUsedVendingMachine")]
        private void OnSetUsedVendingMachine([FromSource] Player player, int networkId)
        {
            Entity vendingMachine = Entity.FromNetworkId(networkId);

            if (vendingMachine is null)
            {
                return;
            }

            Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Initializing statebags for vending machine (^3NetID: {networkId}, Player: {player.Name} ({player.Handle})^0)");
            vendingMachine.State.Set("beingUsed", false, true);

            if (unlimitedSoda)
            {
                return;
            }

            vendingMachine.State.Set("sodaLeft", sodaCanCount, true);
            vendingMachine.State.Set("markedForReset", false, true);
        }

        [EventHandler("VendingMachine:Server:setAsUnused")]
        private void OnSetAsUnused([FromSource] Player player, int networkId)
        {
            Entity vendingMachine = Entity.FromNetworkId(networkId);

            if (vendingMachine is null)
            {
                return;
            }

            Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Setting vending machine as used by non-owner {player.Name} ({player.Handle}) (^3NetID: {networkId}, Owner: {vendingMachine.Owner.Name} ({vendingMachine.Owner.Handle})^0)");
            vendingMachine.State.Set("beingUsed", true, true);
        }

        [EventHandler("VendingMachine:Server:initializeVendingMachine")]
        private void OnInitializeVendingMachine([FromSource] Player player, int networkId)
        {
            Entity vendingMachine = Entity.FromNetworkId(networkId);

            if (vendingMachine is null)
            {
                return;
            }

            Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Initializing statebags for vending machine (^3NetID: {networkId}, Player: {player.Name} ({player.Handle})^0)");
            vendingMachine.State.Set("beingUsed", false, true);

            if (unlimitedSoda)
            {
                return;
            }

            vendingMachine.State.Set("sodaLeft", sodaCanCount, true);
            vendingMachine.State.Set("markedForReset", false, true);
        }

        [EventHandler("VendingMachine:Server:markVendingMachineForReset")]
        private void OnMarkVendingMachineForReset([FromSource] Player player, int networkId)
        {
            Entity vendingMachine = Entity.FromNetworkId(networkId);

            if (vendingMachine is null)
            {
                return;
            }

            Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Marking vending machine for reset (^3NetID: {networkId}, Player: {player.Name} ({player.Handle})^0)");

            DateTime resetTime = DateTime.UtcNow.AddMinutes(minutesToReset);
            resetTimes[networkId] = resetTime;

            vendingMachine.State.Set("markedForReset", true, true);
        }

        [EventHandler("VendingMachine:Server:setAsUsed")]
        private void OnSetAsUnused([FromSource] Player player, int netId, int sodaLeft)
        {
            Entity vendingMachine = Entity.FromNetworkId(netId);

            if (vendingMachine is null)
            {
                return;
            };

            Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Setting vending machine as unused by non-owner {player.Name} ({player.Handle}) (^3NetID: {netId}, Owner: {vendingMachine.Owner.Name} ({vendingMachine.Owner.Handle})^0)");
            vendingMachine.State.Set("beingUsed", false, true);

            if (!unlimitedSoda)
            {
                vendingMachine.State.Set("sodaLeft", sodaLeft, true);
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task ResetTick()
        {
            List<int> networkIds = new(resetTimes.Keys);

            foreach (int networkId in networkIds)
            {
                if (!resetTimes.TryGetValue(networkId, out DateTime resetTime))
                {
                    return;
                }

                if (DateTime.UtcNow < resetTime)
                {
                    continue;
                }

                Entity vendingMachine = Entity.FromNetworkId(networkId);

                if (vendingMachine is null)
                {
                    continue;
                }

                Debug.WriteLine($"[^3{DateTime.Now.ToString("G", CultureInfo.InvariantCulture)}^0] Resetting vending machine (^3NetID: {networkId}^0)");

                vendingMachine.State.Set("sodaLeft", sodaCanCount, true);
                vendingMachine.State.Set("beingUsed", false, true);
                vendingMachine.State.Set("markedForReset", false, true);

                resetTimes.TryRemove(networkId, out resetTime);
            }

            await Delay(20000);
        }
        #endregion
    }
}