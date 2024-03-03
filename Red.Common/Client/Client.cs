using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public class Client : BaseScript
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
        /// Shortened version of GetClosestPropToClient without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Prop GetClosestProp(float radius = 5f) => PlayerPed.GetClosestPropToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestPropToClient without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Prop GetClosestPropToPlayer(float radius = 5f) => PlayerPed.GetClosestPropToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestPropToClient without PlayerPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Prop GetClosestPropToPed(float radius = 5f) => PlayerPed.GetClosestPropToClient(radius);

        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public static void PlayAnim(string dictionary, string name) => PlayerPed.Task.PlayAnimation(dictionary, name);

        public static void PlayAnim(string dictionary, string name, float blindInSpeed, int duration, AnimationFlags flags) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, duration, flags);

        public static void PlayAnim(string dictionary, string name, float blindInSpeed, float blindOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, blindOutSpeed, duration, flags, playbackRate);

        /// <summary>
        /// Calculates the distance to a blip
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static float CalculateDistanceTo(Blip blip, float x, float y, float z) => blip.CalculateDistanceTo(new(x, y, z));
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

        #region Entity Opacity
        /// <summary>
        /// Changes the entity opacity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="alpha"></param>
        /// <param name="changeSkin"></param>
        public static void SetEntityOpacity(int entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity, alpha, changeSkin ? 1 : 0);

        public static void SetEntityOpacity(Entity entity, int alpha, bool changeSkin = false) => SetEntityAlpha(entity.Handle, alpha, changeSkin ? 1 : 0);
        #endregion

        #region Requests
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
            RequestAnimSet(animSet); // Request an animation set
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
        #endregion

        #region Loads
        /// <summary>
        /// Loads an audio bank ambient sound
        /// </summary>
        /// <param name="audioBank"></param>
        /// <returns>Loading an audio bank</returns>
        public static async Task LoadAmbientAudioBank(string audioBank)
        {
            while (!RequestAmbientAudioBank(audioBank, false))
            {
                await Delay(0);
            }
        }

        public static async void LoadScaleformMovie(string scaleformName)
        {
            int scaleform = RequestScaleformMovie(scaleformName);

            while (!HasScaleformMovieLoaded(scaleform))
            {
                await Delay(0);
            }
        }
        #endregion

        #region Animations
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <returns>An animation being played</returns>
        public static void PlayAnimation(string dictionary, string name) => PlayAnim(dictionary, name);
        
        public static void PlayAnimation(string dictionary, string name, float blendOutSpeed, int duration, AnimationFlags flags) => PlayAnim(dictionary, name, blendOutSpeed, duration, flags);
        
        public static void PlayAnimation(string dictionary, string name, float blindInSpeed, float blendOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayAnim(dictionary, name, blindInSpeed, blendOutSpeed, duration, flags, playbackRate);
        
        public static void PlayAnimation(string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, int animationFlags, float playbackRate) => PlayAnim(dictionary, name, blendInSpeed, blendOutSpeed, duration, (AnimationFlags)animationFlags, playbackRate);
        #endregion

        #region Controls
        /// <summary>
        /// Disables specific player controls for the current frame, optionally including camera controls.
        /// </summary>
        /// <param name="cameraMovement">Whether to disable camera controls in addition to movement controls.</param>
        public static void DisableMovementControls(bool cameraMovement)
        {
            // Disable camera controls if requested.
            if (cameraMovement)
            {
                foreach (Control control in cameraControls)
                {
                    // Disable the control for both the player and third-person camera.
                    Game.DisableControlThisFrame(0, control);  // Player
                    Game.DisableControlThisFrame(2, control);  // Third-person camera
                }
            }

            // Disable movement controls.
            foreach (Control control in movementControls)
            {
                // Disable the control for both the player and third-person camera.
                Game.DisableControlThisFrame(0, control);
                Game.DisableControlThisFrame(2, control);
            }
        }

        /// <summary>
        /// Checks if the last input was a controller input.
        /// </summary>
        /// <returns></returns>
        public static bool LastInputWasController() => IsUsingKeyboard(2);
        #endregion

        #region Tasks
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
        #endregion

        #region Chat Methods
        /// <summary>
        /// Shortened down version of the chat:addMessage event.
        /// </summary>
        /// <param name="author"></param>
        /// <param name="message"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void AddChatMessage(string author, string message, int r = 255, int g = 255, int b = 255) => TriggerEvent("chat:addMessage", new { color = new[] { r, g, b }, args = new[] { author, message } });

        /// <summary>
        /// Shortened down version of the _chat:chatMessage event.
        /// </summary>
        /// <param name="author"></param>
        /// <param name="message"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public static void ChatMessage(string author, string message, int r = 255, int g = 255, int b = 255) => TriggerEvent("_chat:chatMessage", author, new[] { r, g, b }, message);
        #endregion
    }
}