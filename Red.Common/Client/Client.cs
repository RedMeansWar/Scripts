using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.MathExtender;

namespace Red.Common.Client
{
    public class Client : BaseScript
    {
        #region Variables
        protected static Ped PlayerPed = Game.PlayerPed;
        protected static Ped Character = Game.Player.Character;
        protected static Player Player = Game.Player;
        protected static Vector3 PlayerPosition = PlayerPed.Position;
        protected static Random random = new();

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

        #region "GetClosest" Methods
        /// <summary>
        /// Gets the closest player to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToClient(float radius = 2f)
        {
            Player closestPlayer = null;
            PlayerList PlayerList = PlayerList.Players;

            float closestDistance = float.MaxValue;
            float distance = Vector3.DistanceSquared(Character.Position, PlayerPosition);

            foreach (Player player in PlayerList)
            {
                if (player is null || player == Game.Player)
                    continue;

                if (distance < closestDistance && distance < radius)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            return closestPlayer;
        }

        /// <summary>
        /// Gets the closest vehicle to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPlayer(float radius = 2f)
        {
            RaycastResult raycast = World.RaycastCapsule(PlayerPosition, PlayerPosition, radius, (IntersectOptions)10, PlayerPed);

            return raycast.HitEntity as Vehicle;
        }

        /// <summary>
        /// Gets the closest player to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayer(float radius = 2f) => GetClosestPlayerToClient(radius);

        /// <summary>
        /// Gets the closest player to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToPed(float radius = 2f) => GetClosestPlayerToClient(radius);

        /// <summary>
        /// Gets the closest vehicle to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(float radius = 2f) => GetClosestVehicleToPlayer(radius);

        /// <summary>
        /// Gets the closest vehicle to the client within a given radius.
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicleToPed(float radius = 2f) => GetClosestVehicleToPlayer(radius);
        #endregion

        #region Player Actions
        /// <summary>
        /// Determines if the player cannot do an action based off of being cuffed, is dead, is currently being stunned, is climbing, is driving a vehicle, is falling,
        /// is getting into a vehicle, is jumping, is getting out of a vehicle, is in ragdoll mode, is swimming under water, or is vaulting over something.
        /// </summary>
        /// <param name="ped"></param>
        /// <returns></returns>
        public static bool CannotDoAction(Ped ped)
        {
            return ped.IsCuffed
            || ped.IsDead || ped.IsBeingStunned
            || ped.IsClimbing || ped.IsDiving || ped.IsFalling
            || ped.IsGettingIntoAVehicle || ped.IsJumping || ped.IsJumpingOutOfVehicle
            || ped.IsRagdoll || ped.IsSwimmingUnderWater || ped.IsVaulting;
        }

        /// <summary>
        /// Disables all attack controls.
        /// </summary>
        public static void DisableAttackControls()
        {
            foreach (Control control in attackControls)
            {
                Game.DisableControlThisFrame(0, control);
            }
        }
        
        /// <summary>
        /// Disables movement controls. (you can choose if you want to disable movement controls)
        /// </summary>
        /// <param name="disableCameraMovement"></param>
        public static void DisableMovementControls(bool disableCameraMovement = false)
        {
            foreach (Control control in movementControls)
            {
                Game.DisableControlThisFrame(0, control);
            }

            if (disableCameraMovement is true)
            {
                foreach (Control control in cameraControls)
                {
                    Game.DisableControlThisFrame(0, control);
                }
            }
        }
        #endregion

        #region Calculate Methods
        public static float CalculateHeadingTowardsEntity(Entity entity, Entity targetEntity)
        {
            Vector3 dirToTargetEnt = targetEntity.Position - entity.Position;
            dirToTargetEnt.Normalize();

            return ConvertDirectionToHeading(dirToTargetEnt);
        }

        public static float CalculateHeadingTowardsPosition(Vector3 start, Vector3 target)
        {
            Vector3 dirToTargetEnt = target - start;
            dirToTargetEnt.Normalize();

            return ConvertDirectionToHeading(dirToTargetEnt);
        }
        #endregion
    }
}
