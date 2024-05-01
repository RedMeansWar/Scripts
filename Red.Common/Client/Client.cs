using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    #pragma warning disable
    public class Client : BaseScript
    {
        #region Private Variables
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
        public static Player ClientPlayer = Game.Player;

        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without Player to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayer(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);

        /// <summary>
        /// Shortened version of GetClosestPlayerToPlayer without Player to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToPed(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);

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
        /// Loads a unloaded animation.
        /// </summary>
        /// <param name="animDict"></param>
        public static async void LoadAnim(string animDict)
        {
            while (!HasAnimDictLoaded(animDict))
            {
                RequestAnimDict(animDict);
                await Delay(5);
            }
        }

        /// <summary>
        /// Same as LoadAnim just extended
        /// </summary>
        /// <param name="animDict"></param>
        public static async void LoadAnimDict(string animDict) => LoadAnim(animDict);

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

        /// <summary>
        /// Loads a weapon asset if it's not loaded.
        /// </summary>
        /// <param name="weaponHash"></param>
        /// <returns>A weapon asset being loaded.</returns>
        public static async void LoadWeaponAsset(uint weaponHash)
        {
            RequestWeaponAsset(weaponHash, 31, 0);
            while (!HasWeaponAssetLoaded(weaponHash))
            {
                await Delay(0);
            }
        }

        public static void LoadWeaponAsset(WeaponHash weaponHash) => LoadWeaponAsset((uint)weaponHash);
        #endregion

        #region Animations
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <returns>An animation being played</returns>
        public static async Task PlayAnimation(string dictionary, string name) => PlayerPed.Task.PlayAnimation(dictionary, name);

        public static async Task PlayAnimation(string dictionary, string name, float blendOutSpeed, int duration, AnimationFlags flags) => PlayerPed.Task.PlayAnimation(dictionary, name, blendOutSpeed, duration, flags);

        public static async Task PlayAnimation(string dictionary, string name, float blindInSpeed, float blendOutSpeed, int duration, AnimationFlags flags, float playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, blindInSpeed, blendOutSpeed, duration, flags, playbackRate);

        public static async Task PlayAnimation(string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, int animationFlags, float playbackRate) => PlayerPed.Task.PlayAnimation(dictionary, name, blendInSpeed, blendOutSpeed, duration, (AnimationFlags)animationFlags, playbackRate);

        public static async Task PlayAnimation(string dictionary, string name, float blendOutSpeed, int duration, int flags) => PlayerPed.Task.PlayAnimation(dictionary, name, blendOutSpeed, duration, (AnimationFlags)flags);
        #endregion

        #region Props
        public static async Task CreateProp(string modelName, Vector3 position, bool hasPhysics = false, bool placeOnGround = false) => World.CreateProp(new(modelName), position, hasPhysics, placeOnGround);

        public static async Task CreatePropWithPhysics(string modelName, Vector3 position, bool placeOnGround = false) => World.CreateProp(new(modelName), position, true, placeOnGround);

        public static async Task CreatePropOnGround(string modelName, Vector3 position, bool hasPhysics = false) => World.CreateProp(new(modelName), position, hasPhysics, true);

        public static async Task CreateProp(Model model, Vector3 position, bool hasPhysics = false, bool placeOnGround = false) => World.CreateProp(model, position, hasPhysics, placeOnGround);

        public static async Task CreatePropWithPhysics(Model model, Vector3 position, bool placeOnGround = false) => World.CreateProp(model, position, true, placeOnGround);

        public static async Task CreatePropOnGround(Model model, Vector3 position, bool hasPhysics = false) => World.CreateProp(model, position, hasPhysics, true);
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

        /// <summary>
        /// Clear a lookout task
        /// </summary>
        public static void ClearLookAtTask() => PlayerPed.Task.ClearLookAt();
        #endregion
    }
}