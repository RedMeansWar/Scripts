using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.SpikeStrips.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected string modelName = "p_ld_stinger_s";
        protected Prop spikeProp;
        protected Ped PlayerPed = Game.PlayerPed;

        protected readonly Dictionary<string, int> vehicleWheels = new()
        {
            { "wheel_lf", 0 },
            { "wheel_rf", 1 },
            { "wheel_lm", 2 },
            { "wheel_rm", 3 },
            { "wheel_lr", 4 },
            { "wheel_rr", 5 }
        };
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

            if (PlayerPed.IsCuffed || PlayerPed.IsDead || PlayerPed.IsBeingStunned || PlayerPed.IsClimbing || PlayerPed.IsDiving || PlayerPed.IsFalling || PlayerPed.IsGettingIntoAVehicle || PlayerPed.IsJumping || PlayerPed.IsJumpingOutOfVehicle || PlayerPed.IsRagdoll || PlayerPed.IsSwimmingUnderWater || PlayerPed.IsVaulting)
            {
                TriggerEvent("_chat:chatMessage", "[SpikeStrips]", new[] { 255, 0, 0 }, "You can't do this right now!");
                return;
            }

            Screen.ShowNotification("~g~Deploying spikes...");
            await PlayerPed.Task.PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", 3.5f, 3.5f, 1500, AnimationFlags.None, 0f);

            await Delay(250);
            TriggerServerEvent("Spikes:Server:spawnSpikes", spikeAmount);

            await Delay(1500);
            RemoveAnimDict("amb@medic@standing@kneel@idle_a");
            Screen.ShowNotification("~g~Deployed spikes!");
        }

        [Command("removeallspikes")]
        private void OnRemoveAllSpikes() => TriggerServerEvent("Spikes:Server:deleteAllSpikes");
        #endregion

        #region Event Handlers
        [EventHandler("Spikes:Client:spawnSpikes")]
        private async void OnSpawnSpikes(int spikeAmount)
        {
            for (int i = 0; i < spikeAmount; i++)
            {
                Vector3 spawnPosition = new Vector3(PlayerPed.Position.X, PlayerPed.Position.Y, PlayerPed.Position.Z) + PlayerPed.ForwardVector * (5f + spikeAmount); 

                spikeProp = await World.CreateProp(modelName, spawnPosition, true, true);
                spikeProp.Heading = PlayerPed.Heading;

                spikeProp.IsPositionFrozen = true;
                spikeProp.IsPersistent = true;
                spikeProp.IsInvincible = true;
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task SecondaryTick()
        {
            spikeProp = World.GetAllProps().Where(prop => prop.Model == "p_ld_stinger_s").OrderBy(prop => Vector3.DistanceSquared(prop.Position, PlayerPed.Position)).FirstOrDefault();

            if (spikeProp is null || NetworkGetEntityOwner(spikeProp.Handle) != Game.Player.Handle)
            {
                await Delay(3000);
                return;
            }

            float distance = Vector3.DistanceSquared(PlayerPed.Position, spikeProp.Position);

            if (distance > 30.0f)
            {
                await Delay(2000);
                return;
            }

            if (distance > 5f)
            {
                await Delay(1000);
                return;
            }

            if (!PlayerPed.IsGettingIntoAVehicle && !PlayerPed.IsClimbing && !PlayerPed.IsVaulting && PlayerPed.IsOnFoot && !PlayerPed.IsRagdoll && !PlayerPed.IsSwimming && distance <= 4.5f)
            {
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_CHARACTER_WHEEL~ + ~INPUT_CONTEXT~ to remove the spikestrips");

                if (IsControlPressed(0, 19) && IsControlPressed(0, 51))
                {
                    float heading = GetHeadingFromVector_2d(spikeProp.Position.X - PlayerPed.Position.X, spikeProp.Position.X - PlayerPed.Position.Y);
                    SetEntityHeading(PlayerPed.Handle, heading);

                    await PlayerPed.Task.PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", 2.5f, 2.5f, 3500, AnimationFlags.None, 0.0f);
                    await Delay(1500);

                    RemoveAnimDict("amb@medic@standing@kneel@idle_a");
                    TriggerServerEvent("Spikes:Server:deleteSpikes");
                }
            }
        }
        #endregion
    }
}