using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;

namespace Red.SpikesStrips.Client
{
    public class ClientMain : BaseScript
    {
        // PlayAnimation("amb@medic@standing@kneel@idle_a", "idle_a", 2.5f, 2.5f, deploying ? 1500 : 1500, AnimationFlags.None, 0.0f);
        #region Variables
        protected string spikeModel = "p_ld_stinger_s";
        protected Ped PlayerPed = Game.PlayerPed;
        protected Prop spikeProp;
        protected List<float> heights = new();
        protected Vector3 minimumVector, maximumVector, size;
        protected float height, length, width;

        protected readonly IReadOnlyDictionary<string, int> vehicleWheels = new Dictionary<string, int>()
        {
            { "wheel_lf", 0 },
            { "wheel_rf", 1 },
            { "wheel_lm", 2 },
            { "wheel_rm", 3 },
            { "wheel_lr", 4 },
            { "wheel_rr", 5 }
        };
        #endregion

        #region Constructor
        public ClientMain()
        {
            GetModelDimensions((uint)GetHashKey(spikeModel), ref minimumVector, ref maximumVector);
            size = maximumVector - minimumVector;

            width = size.X;
            length = size.Y;
            height = size.Z;
        }
        #endregion

        #region Commands
        [Command("setspikes")]
        private async void SetSpikes(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int numberToDeploy) || numberToDeploy < 2 || numberToDeploy > 4)
            {
                Screen.ShowNotification("~r~You can't do this right now");
                return;
            }

            if (PlayerPed.IsAlive && !PlayerPed.IsInVehicle() && !PlayerPed.IsGettingIntoAVehicle && !PlayerPed.IsClimbing && !PlayerPed.IsVaulting && PlayerPed.IsOnFoot && !PlayerPed.IsRagdoll && !PlayerPed.IsSwimming)
            {
                Screen.ShowNotification("~r~You can't do this right now");
                return;
            }

            Screen.ShowNotification("");
            PlaySettingAnimation(true);

            await Delay(1500);
            RemoveAnimDict("amb@medic@standing@kneel@idle_a");

            for (int i = 0; i < numberToDeploy; i++)
            {
                Vector3 spikeSpawnCoords = PlayerPed.Position + PlayerPed.ForwardVector * (3.4f * (4.825f * i));
                spikeProp = await World.CreateProp(spikeModel, spikeSpawnCoords, Vector3.Zero, true, true);

                float groundHeight = World.GetGroundHeight(spikeSpawnCoords);
                heights.Add(groundHeight);

                spikeProp.IsPositionFrozen = true;
                spikeProp.IsPersistent = true;
                spikeProp.IsInvincible = true;
            }

            TriggerServerEvent("Spikes:Server:spawnSpikes", numberToDeploy, PlayerPed.Position, heights);
            Screen.ShowNotification("~g~Deployed Spikes");
        }
        #endregion

        #region Methods
        private void PlaySettingAnimation(bool deploying) => TaskPlayAnim(PlayerPed.Handle, "amb@medic@standing@kneel@idle_a", "idle_a", 2.5f, 2.5f, deploying ? 1500 : 1500, 0, 0.0f, false, false, false);

        private bool VehicleTouchingSpike(Vector3 coords, int strip)
        {
            return false;
        }
        #endregion

        #region Event Handlers
        [EventHandler("Spikes:Client:spawnSpikes")]
        private void OnSpawnSpikes(int number)
        {

        }
        #endregion

        #region Ticks
        [Tick]
        private async Task PickupSpikesTick()
        {
            Prop prop = World.GetAllProps().Where(p => p.Model == spikeModel).OrderBy(p => Vector3.DistanceSquared(p.Position, PlayerPed.Position)).FirstOrDefault();

            if (prop is null || NetworkGetEntityOwner(prop.Handle) != Game.Player.Handle)
            {
                await Delay(3000);
                return;
            }

            float distance = Vector3.DistanceSquared(PlayerPed.Position, prop.Position);

            if (distance > 30.0f)
            {
                await Delay(2000);
                return;
            }

            if (distance > 5.0f)
            {
                await Delay(1000);
                return;
            }

            if (!PlayerPed.IsGettingIntoAVehicle && !PlayerPed.IsClimbing && !PlayerPed.IsVaulting && PlayerPed.IsOnFoot && !PlayerPed.IsRagdoll && !PlayerPed.IsSwimming && distance <= 4.5f)
            {
                Screen.DisplayHelpTextThisFrame("Press ~INPUT_CHARACTER_WHEEL~ + ~INPUT_CONTEXT~ to remove the spikestrips");
                
                if (IsControlPressed(0, 19) && IsControlPressed(0, 51))
                {
                    float heading = GetHeadingFromVector_2d(prop.Position.X - PlayerPed.Position.X, prop.Position.X - PlayerPed.Position.Y);
                    SetEntityHeading(PlayerPed.Handle, heading);

                    PlaySettingAnimation(false);
                    await Delay(1500);

                    RemoveAnimDict("amb@medic@standing@kneel@idle_a");
                    TriggerServerEvent("Spikes:Server:deleteSpikes");
                }
            }
        }

        [Tick]
        private async Task CheckedSpikedTick()
        {
            Vehicle vehicle = PlayerPed.CurrentVehicle;

            if (vehicle is null || vehicle.Driver != PlayerPed)
            {
                await Delay(3000);
                return;
            }

            int closestSpikeStrip = GetClosestObjectOfType(PlayerPed.Position.X, PlayerPed.Position.Y, PlayerPed.Position.Z, 30.0f, (uint)GetHashKey(spikeModel), false, false, false);

            if (closestSpikeStrip == 0)
            {
                await Delay(500);
                return;
            }

            foreach (KeyValuePair<string, int> wheel in vehicleWheels)
            {
                if (!IsVehicleTyreBurst(vehicle.Handle, wheel.Value, false))
                {
                    if (VehicleTouchingSpike(GetWorldPositionOfEntityBone(vehicle.Handle, GetEntityBoneIndexByName(vehicle.Handle, wheel.Key)), closestSpikeStrip))
                    {

                    }
                }
            }
        }
        #endregion
    }
}