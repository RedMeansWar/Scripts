using System;
using System.Drawing;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public static class ClientExtensions
    {
        #region Ped Actions
        /// <summary>
        /// Determines if the Client cannot do a certain action.
        /// </summary>
        /// <param name="ped"></param>
        /// <returns></returns>
        public static bool CannotDoAction(this Ped ped) => DecorGetBool(ped.Handle, "IsDead")
           || ped.IsCuffed || ped.IsDead || ped.IsBeingStunned
           || ped.IsClimbing || ped.IsDiving || ped.IsFalling
           || ped.IsGettingIntoAVehicle || ped.IsJumping
           || ped.IsJumpingOutOfVehicle || ped.IsRagdoll
           || ped.IsSwimmingUnderWater || ped.IsVaulting;
        /// <summary>
        /// Gets the Closest Player to the client with a predefined radius (2 meters by default)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToClient(this Player player, float radius = 2f)
        {
            PlayerList Players = PlayerList.Players;
            Player character = Game.Player;
            Ped playerPed = Game.PlayerPed;
            // Gets the closest player to the client and calculates the distance between them and the player's vector.
            Player closestPlayer = Players.Where(player => player != null && player != character && Entity.Exists(player.Character) && 
            player.Character.Position.DistanceTo2d(playerPed.Position) < radius).OrderBy(player => player.Character.Position.DistanceTo(playerPed.Position)).FirstOrDefault();

            return closestPlayer;
        }
        #endregion

        #region Vehicle Extensions
        /// <summary>
        /// Gets the closest vehicle to a player with a radius (2 meters by default)
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(this Ped ped, float radius = 2f)
        {
            Vector3 pos = ped.Position;
            RaycastResult raycast = World.RaycastCapsule(pos, pos, radius, (IntersectOptions)10, ped);
            // Returns the raycast hit on the vehicle.
            return raycast.DitHitEntity && Entity.Exists(raycast.HitEntity) && raycast.HitEntity.Model.IsVehicle ? (Vehicle)raycast.HitEntity : null;
        }
        /// <summary>
        /// Converts MPS to MPH
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float ConvertToMPH(this float speed) => speed * 2.236936f;
        /// <summary>
        /// Converts MPH to MPS
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float ConvertFromMPH(this float speed) => speed * 0.44704f;
        #endregion

        #region Distance Calculations
        /// <summary>
        /// Shortened down version of "CalculateDistanceTo" to only calculate the 3D area.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float DistanceTo(this Vector3 v1, Vector3 v2) => CalculateDistanceTo(v1, v2, true);
        /// <summary>
        /// Shortened down version of "CalculateDistanceTo" to only target the 2D area.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float DistanceTo2d(this Vector3 v1, Vector3 v2) => CalculateDistanceTo(v1, v2, false);
        /// <summary>
        /// Calculates the distance between 2 vectors and calculates whether it's using 3D
        /// based off the "useZ" boolean.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="useZ"></param>
        /// <returns></returns>
        public static float CalculateDistanceTo(this Vector3 v1, Vector3 v2, bool useZ)
        {
            if (useZ)
            {
                return (float)Math.Sqrt(v1.DistanceToSquared(v2));
            }
            else
            {
                return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
            }
        }
        /// <summary>
        /// Calculeates the heading towards another entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetEntity"></param>
        /// <returns></returns>
        public static float CalculateHeadingTowardsEntity(this Entity entity, Entity targetEntity)
        {
            Vector3 directionToEntity = (targetEntity.Position - entity.Position);
            directionToEntity.Normalize();
            return GameMath.DirectionToHeading(directionToEntity);
        }
        /// <summary>
        /// Calculates the heading of the entity towards a certain position
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static float CalculateHeadingTowardsPosition(this Vector3 startPos, Vector3 targetPos)
        {
            Vector3 directionToTargetEntity = (targetPos - startPos);
            directionToTargetEntity.Normalize();
            return GameMath.DirectionToHeading(directionToTargetEntity);
        }
        #endregion
    }
}
