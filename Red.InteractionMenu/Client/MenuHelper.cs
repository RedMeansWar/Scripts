using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using SharpConfig;
using static CitizenFX.Core.Native.API;

namespace Red.InteractionMenu.Client
{
    public class MenuHelper : BaseScript
    {
        public static string menuName, subMenuPoliceName, subMenuFireName, subMenuVehicleName, subMenuCivilianName, subMenuSceneManagementName;
        public static string ForwardArrow = "→→→";
        public static string BackArrow = "←←←";
        public static int openKey;
        protected bool isCuffed, isFrontCuffed;
        protected Prop cuffsProp;
        protected Ped PlayerPed = Game.PlayerPed;
        #region Config
        public static void ReadConfigFile()
        {
            var data = LoadResourceFile(GetCurrentResourceName(), "config.ini");

            if (Configuration.LoadFromString(data).Contains("InteractionMenu", "MenuName") == true)
            {
                Configuration loaded = Configuration.LoadFromString(data);
                openKey = loaded["InteractionMenu"]["OpenMenuKey"].IntValue;
                menuName = loaded["InteractionMenu"]["MenuName"].StringValue;
                subMenuPoliceName = loaded["InteractionMenu"]["PoliceMenuName"].StringValue;
                subMenuFireName = loaded["InteractionMenu"]["FireMenuName"].StringValue;
                subMenuVehicleName = loaded["InteractionMenu"]["VehicleMenuName"].StringValue;
                subMenuCivilianName = loaded["InteractionMenu"]["CivilianMenuName"].StringValue;
                subMenuSceneManagementName = loaded["InteractionMenu"]["SceneMenuName"].StringValue;
            }
            else
            {
                Debug.WriteLine($"[InteractionMenu]: Config file has not been configured correctly.");
            }
        }
        #endregion

        #region Methods
        // Forked from vMenu Common Functions (https://github.com/TomGrobbe/vMenu/blob/master/vMenu/CommonFunctions.cs#L1755)
        public static async Task<string> GetUserInput(string windowTitle, string defaultText, int maxInputLength)
        {
            // Create the window title string.
            var spacer = "\t";
            AddTextEntry($"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", $"{windowTitle ?? "Enter"}:{spacer}(MAX {maxInputLength} Characters)");

            // Display the input box.
            DisplayOnscreenKeyboard(1, $"{GetCurrentResourceName().ToUpper()}_WINDOW_TITLE", "", defaultText ?? "", "", "", "", maxInputLength);
            await Delay(0);
            // Wait for a result.
            while (true)
            {
                var keyboardStatus = UpdateOnscreenKeyboard();

                switch (keyboardStatus)
                {
                    case 3: // not displaying input field anymore somehow
                    case 2: // cancelled
                        return null;
                    case 1: // finished editing
                        return GetOnscreenKeyboardResult();
                    default:
                        await Delay(0);
                        break;
                }
            }
        }

        public static async Task<string> GetUserInput(string text, int maxInputLength) => await GetUserInput(null, text, maxInputLength);

        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(Game.PlayerPed.Position, Game.PlayerPed.Position, radius, (IntersectOptions)10, Game.PlayerPed);

            return raycast.HitEntity as Vehicle;
        }

        public static Player GetClosestPlayer(float radius = 2f)
        {
            Player closestPlayer = null;
            PlayerList PlayerList = PlayerList.Players;

            float closestDist = float.MaxValue;
            float distance = Vector3.Distance(Game.Player.Character.Position, Game.PlayerPed.Position);

            foreach (var player in PlayerList)
            {
                if (player is null || player == Game.Player)
                    continue;

                if (distance < closestDist && distance < radius)
                {
                    closestPlayer = player;
                    closestDist = distance;
                }
            }

            return closestPlayer;
        }

        public static bool CannotDoAction(Ped ped)
        {
            return ped.IsCuffed
            || ped.IsDead || ped.IsBeingStunned
            || ped.IsClimbing || ped.IsDiving || ped.IsFalling
            || ped.IsGettingIntoAVehicle || ped.IsJumping || ped.IsJumpingOutOfVehicle
            || ped.IsRagdoll || ped.IsSwimmingUnderWater || ped.IsVaulting;
        }

        public static void WeaponSystem(WeaponHash hash)
        {
            Vehicle closestVeh = GetClosestVehicleToPlayer(1f);

            string gun = hash switch
            {
                WeaponHash.CarbineRifle => "long gun",
                WeaponHash.PumpShotgun => "12 gauge shotgun",
                _ => "gun"
            };

            if (Game.PlayerPed.IsInPoliceVehicle || closestVeh?.ClassType == VehicleClass.Emergency)
            {
                Weapon playerWeapon = Game.PlayerPed.Weapons[hash];

                if (playerWeapon is not null)
                {
                    Game.PlayerPed.Weapons.Remove(playerWeapon);
                    Screen.ShowNotification($"~g~~h~Success~h~~s~: You've unequipped and locked your {gun}.", true);
                }
                else
                {
                    playerWeapon = Game.PlayerPed.Weapons.Give(hash, 0, true, true);
                    playerWeapon.Ammo = playerWeapon.MaxAmmoInClip * 3;
                    playerWeapon.Components[WeaponComponentHash.AtArFlsh].Active = true;

                    if (hash == WeaponHash.CarbineRifle)
                    {
                        playerWeapon.Components[WeaponComponentHash.AtPiFlsh].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtArAfGrip].Active = true;
                        playerWeapon.Components[WeaponComponentHash.AtScopeMedium].Active = true;
                    }

                    Screen.ShowNotification($"~g~~h~Success~h~~s~: You've unlocked and equipped your {gun}", true);
                }
            }
            else
            {
                Screen.ShowNotification($"~r~~h~Error~h~~s~: You must be in or near a police cruiser to use this.", true);
            }
        }

        #endregion

        #region Cuff Methods
        [Command("cuffme")]
        private void OnCuffMeCommand() => HandleCuff();

        [Command("frontcuffme")]
        private void OnFrontCuffMeCommand() => HandleCuff(true);

        [Command("ziptieme")]
        private void OnZiptieMeCommand() => HandleCuff(false, true);

        [Command("frontziptieme")]
        private void OnFrontZiptieMeCommand() => HandleCuff(true, true);

        [Command("cuff")]
        private void OnCuffCommand() => CuffClosestPlayer(false, false);

        [Command("frontcuff")]
        private void OnFrontCuffCommand() => CuffClosestPlayer(true, false);

        [Command("ziptie")]
        private void OnZiptieCommand() => CuffClosestPlayer(false, true);

        [Command("frontziptie")]
        private void OnFrontZiptieCommand() => CuffClosestPlayer(true, true);

        private async void HandleCuff(bool isFront = false, bool isZiptie = false)
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

                Tick += CuffedTick;
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

                Tick -= CuffedTick;
            }
        }

        private async void PlayCuffedAnimation(int cuffer, bool isZiptie)
        {
            TriggerServerEvent("pnw:framework:server:playCuffAnimation", cuffer, !isCuffed);

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

                Tick += CuffedTick;
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

                Tick -= CuffedTick;
            }
        }

        private void CuffClosestPlayer(bool isFront, bool isZiptie)
        {
            Player closestPlayer = GetClosestPlayer();

            if (CannotDoAction(PlayerPed))
            {
                Screen.ShowNotification("~r~~h~Error~h~~s~: You can't do this right now.", true);
                return;
            }

            TriggerServerEvent("Cuff:Server:cuffClosestPlayer", closestPlayer.ServerId, isFront, isZiptie);
        }

        [EventHandler("Cuff:Client:playCuffAnimation")]
        private void OnPlayCuffAnimation(bool uncuff) => PlayerPed.Task.PlayAnimation(uncuff ? "mp_arresting" : "rcmpaparazzo_3", uncuff ? "a_uncuff" : "poppy_arrest_cop", 4f, 4f, 3000, AnimationFlags.UpperBodyOnly, 0.595f);

        [EventHandler("Cuff:Client:getCuffed")]
        private void OnGetCuffed(int cuffer, bool isFront, bool isZiptie)
        {
            isCuffed = !isCuffed;
            isFrontCuffed = isFront;
            PlayCuffedAnimation(cuffer, isZiptie);
        }

        private async Task CuffedTick()
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

        #region Grab / Seat Methods

        #endregion
    }
}
