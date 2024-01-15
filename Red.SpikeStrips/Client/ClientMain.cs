using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;

namespace Red.SpikeStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected string modelName = "p_ld_stinger_s";
        protected Prop spikeProp;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly List<int> tireIndex = new()
        {
            0,
            1,
            2,
            4,
            5,
            45,
            46
        };

        protected List<int> spikeCount = new();
        #endregion

        #region Commands
        [Command("setspikes")]
        private async void SetSpikes(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int spikeAmount) || spikeAmount < 2 || spikeAmount > 4)
            {
                TriggerEvent("_chat:chatMessage", "[SpikeStrips]", new[] { 255, 0, 0 }, "Invalid spikestrip amount! Usage: /setspikes <spike amouunt>");
                return;
            }

            if (PlayerPed.CannotDoAction() && !PlayerPed.IsOnFoot)
            {
                TriggerEvent("_chat:chatMessage", "[SpikeStrips]", new[] { 255, 0, 0 }, "You can't do this right now!");
                return;
            }

            Screen.ShowNotification("~g~Deploying spikes...");
            await PlayerPed.Task.PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", 3.5f, 3.5f, 1500 * spikeAmount, AnimationFlags.None, 0f);

            await Delay(250);
            TriggerServerEvent("Spikes:Server:spawnSpikes", spikeAmount);

            await Delay(4500);
            Screen.ShowNotification("~g~Deployed spikes!");
            RemoveAnimDict("amb@medic@standing@kneel@idle_a");
        }

        [Command("removeallspikes")]
        private void OnRemoveAllSpikes() => TriggerServerEvent("Spikes:Server:deleteAllSpikes");
        #endregion

        #region Event Handlers
        [EventHandler("Spikes:Client:spawnSpikes")]
        private async void OnSpawnSpikes(int spikeAmount)
        {
            spikeProp = await World.CreateProp(modelName, new(PlayerPed.Position.X, PlayerPed.Position.Y, PlayerPed.Position.Z), Vector3.Zero, true, true);

            spikeProp.Heading = PlayerPed.Heading;
            spikeProp.IsPersistent = true;
            spikeProp.IsInvincible = true;
            spikeProp.IsPositionFrozen = true;
        }

        [EventHandler("Spikes:Client:deleteSpikes")]
        private async void OnDeleteSpikes()
        {
            if (Game.IsControlJustPressed(0, Control.Context) && Vector3.DistanceSquared(PlayerPed.Position, spikeProp.Position) < 5f)
            {
                spikeProp.Delete();
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task SpikeVehicleTick()
        {
            Vehicle currentVehicle = Game.PlayerPed.CurrentVehicle;

            if (spikeProp.GetDistanceToProp(modelName) < 3f && currentVehicle != null)
            {
                foreach (var tire in tireIndex)
                {
                    SetVehicleTyreBurst(currentVehicle.Handle, tire, false, 1000f);
                }
            }
            else
            {
                await Delay(250);
                return;
            }
        }

        [Tick]
        private async Task DeleteSpikesTick() => OnDeleteSpikes();
        #endregion
    }
}