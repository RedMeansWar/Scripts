using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public class Client : ClientScript
    {
        #region Private Variables
        private static Player Player;

        protected static readonly IReadOnlyList<Control> cameraControls = new List<Control>()
        {
            Control.LookBehind, Control.LookDown, Control.LookDownOnly, Control.LookLeft, Control.LookLeftOnly, Control.LookLeftRight, Control.LookRight,
            Control.LookRightOnly, Control.LookUp, Control.LookUpDown, Control.LookUpOnly, Control.ScaledLookDownOnly, Control.ScaledLookLeftOnly,
            Control.ScaledLookLeftRight, Control.ScaledLookUpDown, Control.ScaledLookUpOnly,  Control.VehicleDriveLook, Control.VehicleDriveLook2,
            Control.VehicleLookBehind, Control.VehicleLookLeft, Control.VehicleLookRight, Control.NextCamera, Control.VehicleFlyAttackCamera, Control.VehicleCinCam,
        };

        protected static readonly IReadOnlyList<Control> movementControls = new List<Control>()
        {
            Control.MoveDown, Control.MoveDownOnly, Control.MoveLeft, Control.MoveLeftOnly, Control.MoveLeftRight, Control.MoveRight, Control.MoveRightOnly,
            Control.MoveUp, Control.MoveUpDown, Control.MoveUpOnly, Control.VehicleFlyMouseControlOverride, Control.VehicleMouseControlOverride,
            Control.VehicleMoveDown, Control.VehicleMoveDownOnly, Control.VehicleMoveLeft, Control.VehicleMoveLeftRight, Control.VehicleMoveRight,
            Control.VehicleMoveRightOnly, Control.VehicleMoveUp, Control.VehicleMoveUpDown, Control.VehicleSubMouseControlOverride, Control.Duck, Control.SelectWeapon
        };

        protected static readonly IReadOnlyList<Control> attackControls = new List<Control>()
        {
            Control.Attack, Control.Attack2, Control.MeleeAttack1, Control.MeleeAttack2, Control.MeleeAttackAlternate, Control.MeleeAttackHeavy,
            Control.MeleeAttackLight, Control.MeleeBlock, Control.VehicleAttack, Control.VehicleAttack2, Control.VehicleFlyAttack, Control.VehicleFlyAttack2,
            Control.VehiclePassengerAttack
        };

        protected static readonly IReadOnlyList<WeaponHash> automaticWeapons = new List<WeaponHash>()
        {
            WeaponHash.MicroSMG, WeaponHash.MachinePistol, WeaponHash.MiniSMG, WeaponHash.SMG, WeaponHash.SMGMk2, WeaponHash.AssaultSMG, WeaponHash.CombatPDW,
            WeaponHash.MG, WeaponHash.CombatMG, WeaponHash.CombatMGMk2, WeaponHash.Gusenberg, WeaponHash.AssaultRifle, WeaponHash.AssaultRifleMk2, WeaponHash.CarbineRifle,
            WeaponHash.CarbineRifleMk2, WeaponHash.AdvancedRifle, WeaponHash.SpecialCarbine, WeaponHash.SpecialCarbineMk2, WeaponHash.BullpupRifle, WeaponHash.BullpupRifleMk2,
            WeaponHash.CompactRifle
        };
        #endregion

        #region Extensions
        public static Ped PlayerPed = Game.PlayerPed;
        public static Ped PlayerCharacter = Game.Player.Character;
        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without PlayerPlayer to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayer(float radius = 2f) => Player.GetClosestPlayerToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without PlayerPlayer to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToPed(float radius = 2f) => Player.GetClosestPlayerToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(float radius = 2f) => PlayerPed.GetClosestVehicleToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f) => PlayerPed.GetClosestVehicleToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => PlayerPed.GetClosestVehicleToClient(radius);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public static void PlayAnim(string dictionary, string name) => PlayerPed.Task.PlayAnimation(dictionary, name);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blindInSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, int duration, AnimationFlags flags) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, duration, flags);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blindInSpeed"></param>
        /// <param name="blindOutSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        /// <param name="playbackRate"></param>
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, float blindOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, blindOutSpeed, duration, flags, playbackRate);
        #endregion

        #region Model Checker
        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="modelHash"></param>
        /// <returns></returns>
        public static bool DoesModelExist(uint modelHash) => IsModelInCdimage(modelHash);
        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static bool DoesModelExist(string modelName) => DoesModelExist((uint)GetHashKey(modelName));
        /// <summary>
        /// Checks if a model exist.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool DoesModelExist(Model model) => DoesModelExist((uint)model.Hash);
        #endregion
        /// <summary>
        /// Teleports a player to a specific set of coords
        /// freezing them or teleporting the player with their vehicle is optional
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="pos"></param>
        /// <param name="withVehicle"></param>
        /// <param name="freezePlayer"></param>
        public static void TeleportPlayer(int playerId, Vector3 pos, bool withVehicle, bool freezePlayer)
        {
            PlayerList Players = PlayerList.Players;
            Player player = Players[playerId];

            if (freezePlayer && player is not null && PlayerCharacter.Exists())
            {
                PlayerCharacter.IsPositionFrozen = true;
                FreezeEntityPosition(PlayerCharacter.Handle, true);
                ClearPedTasksImmediately(PlayerCharacter.Handle);
                ClearPedTasks(PlayerCharacter.Handle);
            }

            if (withVehicle && player is not null && PlayerCharacter.Exists() && player.Character.IsInVehicle())
            {
                SetPedCoordsKeepVehicle(PlayerCharacter.Handle, pos.X, pos.Y, pos.Z);
            }
        }
        /// <summary>
        /// Changes the entity opacity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="alpha"></param>
        /// <param name="changeSkin"></param>
        public static void SetEntityOpacity(int entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity, alpha, changeSkin ? 1 : 0);
        /// <summary>
        /// Changes the entity opacity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="alpha"></param>
        /// <param name="changeSkin"></param>
        public static void SetEntityOpacity(Entity entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity.Handle, alpha, changeSkin ? 1 : 0);
        /// <summary>
        /// Request an animation dictionary.
        /// </summary>
        /// <param name="animDict"></param>
        public static async void RequestAnim(string animDict)
        {
            RequestAnimDict(animDict);
            while (!HasAnimDictLoaded(animDict))
            {
                await Delay(100);
            }

            await Delay(0);
        }
        /// <summary>
        /// Same as RequestAnim just extended
        /// </summary>
        /// <param name="animDict"></param>
        public static async void RequestAnimation(string animDict) => RequestAnim(animDict);
        /// <summary>
        /// Request an animation set
        /// </summary>
        /// <param name="animSet"></param>
        public static async void RequestSet(string animSet)
        {
            RequestAnimSet(animSet);
            while (!HasAnimSetLoaded(animSet))
            {
                await Delay(100);
            }

            await Delay(0);
        }
        /// <summary>
        /// Request an animation set
        /// </summary>
        /// <param name="animSet"></param>
        public static async void RequestAnimationSet(string animSet) => RequestSet(animSet);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public static void PlayAnimation(string dictionary, string name) => PlayAnim(dictionary, name);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blindInSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        public static void PlayAnimation(string dictionary, string name, float blendOutSpeed, int duration, AnimationFlags flags) => PlayAnim(dictionary, name, blendOutSpeed, duration, flags);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blindInSpeed"></param>
        /// <param name="blindOutSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        /// <param name="playbackRate"></param>
        public static void PlayAnimation(string dictionary, string name, float blindInSpeed, float blendOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayAnim(dictionary, name, blindInSpeed, blendOutSpeed, duration, flags, playbackRate);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blendInSpeed"></param>
        /// <param name="blendOutSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="animationFlags"></param>
        /// <param name="playbackRate"></param>
        public static void PlayAnimation(string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, int animationFlags, float playbackRate) => PlayAnim(dictionary, name, blendInSpeed, blendOutSpeed, duration, (AnimationFlags)animationFlags, playbackRate);
        /// <summary>
        /// Disables movement controls on control and on keyboard
        /// </summary>
        /// <param name="cameraMovement"></param>
        public static void DisableMovementControls(bool cameraMovement)
        {
            if (cameraMovement)
            {
                foreach (Control control in cameraControls)
                {
                    Game.DisableControlThisFrame(0, control);
                    Game.DisableControlThisFrame(2, control);
                }
            }

            foreach (Control control in movementControls)
            {
                Game.DisableControlThisFrame(0, control);
                Game.DisableControlThisFrame(2, control);
            }
        }
        /// <summary>
        /// Clears all of the peds current tasks that they are performing such as an animation within the next frame
        /// </summary>
        public static void ClearAllTasks() => PlayerPed.Task.ClearAll();
        /// <summary>
        /// Clears all of the peds current tasks that they are performing such as an animation within the time of the method is called.
        /// </summary>
        public static void ClearAllTasksImmediately() => PlayerPed.Task.ClearAllImmediately();
        /// <summary>
        /// Clears the secondary task of the ped that they are performing such as an animation within the next frame
        /// </summary>
        public static void ClearSecondaryTask() => PlayerPed.Task.ClearSecondary();
        /// <summary>
        /// Clears a current animation from the player
        /// </summary>
        /// <param name="animDict"></param>
        /// <param name="animName"></param>
        public static void ClearAnimationTask(string animDict, string animName) => PlayerPed.Task.ClearAnimation(animDict, animName);
    }
}