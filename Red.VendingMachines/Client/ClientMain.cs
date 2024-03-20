using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using Red.Common.Client;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Controls;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;

namespace Red.VendingMachines.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected Prop closestVendingMachineProp;
        protected readonly bool unlimitedSoda;

        protected static readonly Dictionary<int, int> vendingMachines = new()
        {
            { GetHashKey("prop_vend_soda_01"), GetHashKey("prop_ecola_can") },
            { GetHashKey("prop_vend_soda_02"), GetHashKey("prop_ld_can_01b") }
        };
        #endregion

        #region Methods
        private async Task LoadModel(uint model)
        {
            RequestModel(model);
            while (!HasModelLoaded(model))
            {
                await Delay(0);
            }
        }

        private async Task LoadAmbientAudioBank(string bank)
        {
            while (!RequestAmbientAudioBank(bank, false))
            {
                await Delay(0);
            }
        }

        private async Task BuySoda(Entity vendingMachine)
        {
            ClearAllHelpMessages();
            ClearAllTasks();

            bool owner = NetworkGetEntityOwner(vendingMachine.NetworkId) == Game.Player.Handle;

            if (!owner)
            {
                TriggerServerEvent("VendingMachine:Server:setUsedVendingMachine", vendingMachine.NetworkId);
            }
            else
            {
                vendingMachine.State.Set("beingUsed", true, true);
            }

            Vector3 offset = vendingMachine.GetOffsetPosition(new(0f, -0.97f, 0.05f));

            PlayerPed.SetConfigFlag(48, true);
            SetPedStealthMovement(PlayerPed.Handle, false, "DEFAULT_ACTION");
            SetPedResetFlag(PlayerPed.Handle, 322, true);

            PlayerPed.IsInvincible = true;
            PlayerPed.CanBeTargetted = false;
            PlayerPed.CanRagdoll = false;

            if (vendingMachines.TryGetValue(vendingMachine.Model.Hash, out int canModel))
            {
                await LoadModel((uint)canModel);
            }
            else
            {
                PlayerPed.SetConfigFlag(48, false);
                PlayerPed.IsInvincible = false;
                
                PlayerPed.CanBeTargetted = true;
                PlayerPed.CanRagdoll = true;

                return;
            }

            await LoadAmbientAudioBank("VENDING_MACHINE");
            LoadAnimDict("MINI@SPRUNK");

            PlayerPed.Task.LookAt(vendingMachine, 2000);
            TaskGoStraightToCoord(PlayerPed.Handle, offset.X, offset.Y, offset.Z, 1.0f, 20000, vendingMachine.Heading, 0.1f);

            while (GetScriptTaskStatus(PlayerPed.Handle, (uint)GetHashKey("SCRIPT_TASK_GO_STRAIGHT_TO_COORD")) != 7) await Delay(0);

            await PlayAnimation("MINI@SPRUNK", "PLYR_BUY_DRINK_PT1", 2f, -4f, -1, AnimationFlags.None, 0f);

            while (GetEntityAnimCurrentTime(PlayerPed.Handle, "MINI@SPRUNK", "PLYR_BUY_DRINK_PT1") < 0.31f) await Delay(0);

            Prop canProp = await World.CreatePropNoOffset(canModel, offset, new(0f, 0f, 0f), true);
            canProp.IsInvincible = true;
            canProp.AttachTo(PlayerPed.Bones[Bone.PH_R_Hand], new(0f, 0f, 0f), new(0f, 0f, 0f));

            while (GetEntityAnimCurrentTime(PlayerPed.Handle, "MINI@SPRUNK", "PLYR_BUY_DRINK_PT1") < 0.98f) await Delay(0);

            await PlayAnimation("MINI@SPRUNK", "PLYR_BUY_DRINK_PT2", 4f, -1000f, -1, AnimationFlags.None, 0f);
            ForcePedAiAndAnimationUpdate(PlayerPed.Handle, false, false);

            await PlayAnimation("MINI@SPRUNK", "PLYR_BUY_DRINK_PT3", 1000f, -4f, -1, AnimationFlags.UpperBodyOnly | AnimationFlags.AllowRotation, 0f);
            ForcePedAiAndAnimationUpdate(PlayerPed.Handle, false, false);

            while (GetEntityAnimCurrentTime(PlayerPed.Handle, "MINI@SPRUNK", "PLYR_BUY_DRINK_PT3") < 0.306f) await Delay(0);

            canProp.Detach();
            canProp.ApplyForce(new(6f, 10f, 2f), new(0f, 0f, 0f), ForceType.MaxForceRot);
            canProp.MarkAsNoLongerNeeded();

            RemoveAnimDict("MINI@SPRUNK");
            ReleaseAmbientAudioBank();
            SetModelAsNoLongerNeeded((uint)canModel);

            PlayerPed.SetConfigFlag(48, false);
            ClearAllTasks();

            PlayerPed.IsInvincible = false;
            PlayerPed.CanBeTargetted = true;
            PlayerPed.CanRagdoll = true;

            if (!owner)
            {
                if (!unlimitedSoda)
                {
                    int sodaLeft = vendingMachine.State.Get("sodaLeft");
                    TriggerServerEvent("VendingMachine:Server:setAsUnused", vendingMachine.NetworkId, sodaLeft - 1);
                }
                else
                {
                    TriggerServerEvent("VendingMachine:Server:setAsUnused", vendingMachine.NetworkId);
                }
            }
            else
            {
                vendingMachine.State.Set("beingUsed", false, true);

                if (!unlimitedSoda)
                {
                    int sodaLeft = vendingMachine.State.Get("sodaLeft");
                    vendingMachine.State.Set("sodaLeft", sodaLeft - 1, true);
                }
            }
        }
        #endregion

        #region Ticks
        [Tick]
        private async Task VendingMachineTick()
        {
            Prop prop = World.GetAllProps()
                .Where(p => vendingMachines.ContainsKey(p.Model))
                .OrderBy(p => PlayerPed.DistanceTo(p.Position))
                .FirstOrDefault();

            if (PlayerPed.CannotDoAction())
            {
                await Delay(2500);
                return;
            }

            if (prop is null || HasObjectBeenBroken(prop.Handle))
            {
                await Delay(4000);
                return;
            }

            if (!NetworkGetEntityIsNetworked(prop.Handle))
            {
                NetworkRegisterEntityAsNetworked(prop.Handle);
            }

            if (prop.State.Get("beingUsed") is null)
            {
                TriggerServerEvent("VendingMachine:Server:initializeVendingMachine", prop.NetworkId);
                await Delay(3000);
            }

            if (prop.State.Get("beingUsed") is null || prop.State.Get("beingUsed"))
            {
                await Delay(3000);
                return;
            }

            float distance = PlayerPed.DistanceTo(prop.Position);

            if (distance > 5.0f)
            {
                await Delay(1500);
                return;
            }

            if ((unlimitedSoda && prop.State.Get("sodaLeft") > 0) || unlimitedSoda)
            {
                if (!IsPauseMenuActive() && distance < 1.5f)
                {
                    DisplayHelpText("Press ~INPUT_CONTEXT~ to buy a soda for $1.");

                    if (IsControlJustReleased(Control.Context))
                    {
                        await BuySoda(prop);
                    }
                }
            }
            else
            {
                if (!unlimitedSoda && prop.State.Get("markedForReset") is null || !prop.State.Get("markedForReset"))
                {
                    TriggerServerEvent("VendingMachine:Server:markVendingMachineForReset", prop.NetworkId);
                    await Delay(500);
                }

                DisplayHelpText("Vending Machine has run out of sodas.");
            }
        }
        #endregion
    }
}