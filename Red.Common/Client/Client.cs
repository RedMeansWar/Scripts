using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public class Client : BaseScript
    {
        #region Private Lists
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
        public static Ped ClientPed = Game.PlayerPed;
        public static Ped ClientCharacter = Game.Player.Character;
        public static Player ClientPlayer = Game.Player;
        /// <summary>
        /// Shortened version of GetClosestPlayerToClient without ClientPlayer to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayer(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestPlayerToClient without ClientPlayer to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToPed(float radius = 2f) => ClientPlayer.GetClosestPlayerToClient(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(float radius = 2f) => ClientPed.GetClosestVehicle(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f) => ClientPed.GetClosestVehicle(radius);
        /// <summary>
        /// Shortened version of GetClosestVehicle without ClientPed to access it.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => ClientPed.GetClosestVehicle(radius);
        /// <summary>
        /// Shortened version of ClientPed.LastVehicle
        /// </summary>
        public static Vehicle ClientLastVehicle => ClientPed?.LastVehicle;
        /// <summary>
        /// Shortened version of ClientPed.CurrentVehicle
        /// </summary>
        public static Vehicle ClientCurrentVehicle = ClientPed?.CurrentVehicle;
        /// <summary>
        /// Gets the Clients ped position.
        /// </summary>
        public static Vector3 ClientPedPostition = ClientPed.Position;
        /// <summary>
        /// Gets the Clients character position.
        /// </summary>
        public static Vector3 ClientCharacterPosition = ClientCharacter.Position;
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        public static void PlayAnim(string dictionary, string name) => ClientPed.Task.PlayAnimation(dictionary, name);
        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blindInSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, int duration, AnimationFlags flags) => ClientPed.Task.PlayAnimation(dictionary, name, blindInSpeed, duration, flags);
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
        public static void PlayAnim(string dictionary, string name, float blindInSpeed, float blindOutSpeed, int duration, AnimationFlags flags, float playbackRate) => ClientPed.Task.PlayAnimation(dictionary, name, blindInSpeed, blindOutSpeed, duration, flags, playbackRate);
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

            if (freezePlayer && player is not null && ClientCharacter.Exists())
            {
                ClientCharacter.IsPositionFrozen = true;
                FreezeEntityPosition(ClientCharacter.Handle, true);
                ClearPedTasksImmediately(ClientCharacter.Handle);
                ClearPedTasks(ClientCharacter.Handle);
            }

            if (withVehicle && player is not null && ClientCharacter.Exists() && player.Character.IsInVehicle())
            {
                SetPedCoordsKeepVehicle(ClientCharacter.Handle, pos.X, pos.Y, pos.Z);
            }
        }
        /// <summary>
        /// Sets the current players vehicle license plate to the desired name.
        /// Forked from Albo1125
        /// </summary>
        /// <param name="plate"></param>
        public static void SetPlate(string plate)
        {
            if (ClientPed is not null && ClientCharacter is not null && ClientCharacter.Exists())
            {
                if (plate.Length > 8)
                {
                    Debug.WriteLine("Plate was set but the text is too long. Must have a total of 8 characters. (Numbers and Letters)");
                }
                else
                {
                    SetVehicleNumberPlateText(ClientCurrentVehicle.Handle, plate);
                }
            }
        }
        /// <summary>
        /// Fixes the current vehicle that the player is in
        /// </summary>
        public static void FixVehicle()
        {
            if (ClientPlayer != null && ClientCharacter != null && ClientCharacter.CurrentVehicle != null && ClientCharacter.Exists())
            {
                Vehicle vehicle = ClientCurrentVehicle;
                vehicle.Repair();
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

        public static async Task PlayAnimation(string dictionary, string name) => ClientPed.Task.PlayAnimation(dictionary, name);
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
        /// Gets the distance from a blip to the desired vector
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>a float value resulting in meters</returns>
        public static float DistanceFromBlip(int blip, float x, float y, float z)
        {
            if (DoesBlipExist(blip))
            {
                Vector3 blipPos = GetBlipCoords(blip);
                float distance = Vdist(blipPos.X, blipPos.Y, blipPos.Z, x, y, z);

                return distance;
            }
            else
            {
                return -1f;
            }
        }
        /// <summary>
        /// Gets the distance from a blip to the desired vector
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="position"></param>
        /// <returns>a float value resulting in meters</returns>
        public static float DistanceFromBlip(int blip, Vector3 position) => DistanceFromBlip(blip, position.X, position.Y, position.Z);
        /// <summary>
        /// Gets the distance from a blip to the desired vector
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>a float value resulting in meters</returns>
        public static float DistanceFromBlip(Blip blip, float x, float y, float z) => DistanceFromBlip(blip.Handle, x, y, z);
        /// <summary>
        /// Gets the distance from a blip to the desired vector
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="position"></param>
        /// <returns>a float value resulting in meters</returns>
        public static float DistanceFromBlip(Blip blip, Vector3 position) => DistanceFromBlip(blip.Handle, position.X, position.Y, position.Z);
    }
}