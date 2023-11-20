using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;
using static Red.Common.Client.Hud.HUD;
using static Red.Common.Client.Misc.Object;
using static Red.Common.Client.Diagnostics.Log;

namespace Red.Cuff.Client
{
    public class ClientMain : BaseScript
    {
        #region Variables
        protected bool isCuffed, isFrontCuffed;
        protected Prop cuffsProp;
        protected Ped PlayerPed = Game.PlayerPed;
        #endregion

        #region Commands
        [Command("cuffme")]
        private void OnCuffMeCommand() => HandleCuffMe();

        [Command("frontcuffme")]
        private void OnFrontCuffMeCommand() => HandleCuffMe(true);

        [Command("ziptieme")]
        private void OnZiptieMeCommand() => HandleCuffMe(false, true);

        [Command("frontziptieme")]
        private void OnFrontZiptieMeCommand() => HandleCuffMe(true, true);

        [Command("cuff")]
        private void OnCuffCommand() => CuffClosestPlayer(false, false);

        [Command("frontcuff")]
        private void OnFrontCuffCommand() => CuffClosestPlayer(true, false);

        [Command("ziptie")]
        private void OnZiptieCommand() => CuffClosestPlayer(false, true);

        [Command("frontziptie")]
        private void OnFrontZiptieCommand() => CuffClosestPlayer(true, true);

        private async void HandleCuffMe(bool isFront = false, bool isZiptie = false)
        {
            isCuffed = !isCuffed;
            isFrontCuffed = isFront;

            if (isCuffed)
            {
                if (!isZiptie)
                {
                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "cuff", 0.2f);
                }

                PlayerPed.Task.ClearAll();
                PlayerPed.Task.PlayAnimation(isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);

                cuffsProp = await World.CreateProp(new($"{(isZiptie ? "hei_prop_zip_tie_positioned" : "p_cs_cuffs_02_s")}"), Vector3.Zero, false, false);

                if (isFrontCuffed)
                {
                    Vector3 pos;
                    Vector3 rot;

                    if (isZiptie)
                    {
                        pos = new(-0.012f, 0f, 0.08f);
                        rot = new(340f, 95f, 120f);
                    }
                    else
                    {
                        pos = new(-.058f, .005f, .09f);
                        rot = new(290f, 95f, 120f);
                    }

                    AttachEntityToEntity(cuffsProp.Handle, PlayerPed.Handle, GetPedBoneIndex(PlayerPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);
                }
                else
                {
                    Vector3 pos;
                    Vector3 rot;

                    if (isZiptie)
                    {
                        pos = new(-0.020f, 0.035f, 0.06f);
                        rot = new(0.04f, 155f, 80f);
                    }
                    else
                    {
                        pos = new(-.055f, .06f, .04f);
                        rot = new(265f, 155f, 80f);
                    }

                    AttachEntityToEntity(cuffsProp.Handle, PlayerPed.Handle, GetPedBoneIndex(PlayerPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);
                }

                SetPedDropsWeapon(PlayerPed.Handle);
                SetPedCanPlayGestureAnims(PlayerPed.Handle, false);

                Tick += DoStuffWhileCuffed;
            }
            else
            {
                if (!isZiptie)
                {
                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "uncuff", 0.2f);
                }

                PlayerPed.Task.ClearAnimation("mp_arresting", "idle");
                PlayerPed.Task.ClearAnimation("anim@move_m@prisoner_cuffed", "idle");

                cuffsProp?.Delete();
                cuffsProp = null;

                SetPedCanPlayGestureAnims(PlayerPed.Handle, true);

                Tick -= DoStuffWhileCuffed;
            }
        }
        #endregion

        #region Event Handlers
        [EventHandler("Cuff:Client:playCuffAnimation")]
        private void OnPlayCuffAnimation(bool uncuff) => PlayerPed.Task.PlayAnimation(uncuff ? "mp_arresting" : "rcmpaparazzo_3", uncuff ? "a_uncuff" : "poppy_arrest_cop", 4f, 4f, 3000, AnimationFlags.UpperBodyOnly, 0.595f);

        [EventHandler("Cuff:Client:getCuffed")]
        private void OnGetCuffed(int cuffer, bool isFront, bool isZiptie)
        {
            isCuffed = !isCuffed;
            isFrontCuffed = isFront;
            PlayCuffedAnimation(cuffer, isZiptie);
        }
        #endregion

        #region Methods
        private void CuffClosestPlayer(bool isFront, bool isZiptie)
        {
            
            if (CannotDoAction(PlayerPed))
            {
                ErrorNotification("You can't do this right now.");
                return;
            }

            Player closestPlayer = GetClosestPlayer();

            if (closestPlayer is null)
            {
                ErrorNotification($"You must be closer to the person you wish to {(isZiptie ? "ziptie" : "cuff")}.");
                return;
            }

            TriggerServerEvent("Cuff:Server:cuffClosestPlayer", closestPlayer.ServerId, isFront, isZiptie);
        }

        private async void PlayCuffedAnimation(int cuffer, bool isZiptie)
        {
            TriggerServerEvent("Cuff:Server:playCuffAnimation", cuffer, !isCuffed);

            if (isCuffed)
            {
                if (!isZiptie)
                {
                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "cuff", 0.2f);
                }

                PlayerPed.Task.ClearAll();
                PlayerPed.Task.PlayAnimation(isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);

                await Delay(1000);
                cuffsProp = await World.CreateProp(new($"{(isZiptie ? "hei_prop_zip_tie_positioned" : "p_cs_cuffs_02_s")}"), Vector3.Zero, false, false);

                Vector3 pos;
                Vector3 rot;

                if (isFrontCuffed)
                {
                    pos = isZiptie ? new(-0.012f, 0f, 0.08f) : new(-.058f, .005f, .09f);
                    rot = isZiptie ? new(340f, 95f, 120f) : new(290f, 95f, 120f);
                }
                else
                {
                    pos = isZiptie ? new(-0.020f, 0.035f, 0.06f) : new(-.055f, .06f, .04f);
                    rot = isZiptie ? new(0.04f, 155f, 80f) : new(265f, 155f, 80f);
                }

                AttachEntityToEntity(cuffsProp.Handle, PlayerPed.Handle, GetPedBoneIndex(PlayerPed.Handle, 60309), pos.X, pos.Y, pos.Z, rot.X, rot.Y, rot.Z, true, false, false, false, 0, true);

                SetPedDropsWeapon(PlayerPed.Handle);
                SetPedCanPlayGestureAnims(PlayerPed.Handle, false);

                Tick += DoStuffWhileCuffed;
            }
            else
            {
                await Delay(3000);

                if (!isZiptie)
                {
                    TriggerServerEvent("Server:SoundToRadius", PlayerPed.NetworkId, 5f, "uncuff", 0.2f);
                }

                PlayerPed.Task.ClearAnimation("mp_arresting", "idle");
                PlayerPed.Task.ClearAnimation("anim@move_m@prisoner_cuffed", "idle");

                cuffsProp?.Delete();
                cuffsProp = null;

                SetPedCanPlayGestureAnims(PlayerPed.Handle, true);

                Tick -= DoStuffWhileCuffed;
            }
        }

        private async Task DoStuffWhileCuffed()
        {
            Game.DisableControlThisFrame(0, Control.Attack);
            Game.DisableControlThisFrame(0, Control.Attack2);
            Game.DisableControlThisFrame(0, Control.MeleeAttack1);
            Game.DisableControlThisFrame(0, Control.MeleeAttack2);
            Game.DisableControlThisFrame(0, Control.MeleeAttackAlternate);
            Game.DisableControlThisFrame(0, Control.MeleeAttackHeavy);
            Game.DisableControlThisFrame(0, Control.MeleeAttackLight);
            Game.DisableControlThisFrame(0, Control.Aim);
            Game.DisableControlThisFrame(0, Control.SelectWeapon);
            Game.DisableControlThisFrame(0, Control.VehicleMoveLeftRight);
            Game.DisableControlThisFrame(0, Control.VehicleMoveLeftOnly);
            Game.DisableControlThisFrame(0, Control.VehicleMoveRightOnly);
            Game.DisableControlThisFrame(0, Control.VehicleHandbrake);
            Game.DisableControlThisFrame(0, Control.VehicleSubTurnLeftRight);
            Game.DisableControlThisFrame(0, Control.VehicleSubTurnLeftOnly);
            Game.DisableControlThisFrame(0, Control.VehicleSubTurnRightOnly);
            Game.DisableControlThisFrame(0, Control.VehicleSubTurnHardLeft);
            Game.DisableControlThisFrame(0, Control.VehicleSubTurnHardRight);

            if (!IsEntityPlayingAnim(PlayerPed.Handle, isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 3))
            {
                PlayerPed.Task.PlayAnimation(isFrontCuffed ? "anim@move_m@prisoner_cuffed" : "mp_arresting", "idle", 8f, -1, AnimationFlags.StayInEndFrame | AnimationFlags.AllowRotation | AnimationFlags.UpperBodyOnly);
            }

            PlayerPed.Weapons.Select(WeaponHash.Unarmed);
            SetPedStealthMovement(PlayerPed.Handle, false, "DEFUALT_ACTION");
        }

        #endregion

    }
}