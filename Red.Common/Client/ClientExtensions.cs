using System;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

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
        public static bool CannotDoAction(this Ped ped)
        {
            // returning DecorGetBool(ped.Handle, "isDead") is the only way this will work. Alternative?
            return ped.IsCuffed
            || ped.IsDead || ped.IsBeingStunned
            || ped.IsClimbing || ped.IsDiving || ped.IsFalling
            || ped.IsGettingIntoAVehicle || ped.IsJumping
            || ped.IsJumpingOutOfVehicle || ped.IsRagdoll
            || ped.IsSwimmingUnderWater || ped.IsVaulting;
        }
        /// <summary>
        /// Gets the Closest Player to the client with a predefined radius (2 meters by default)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToClient(this Player player, float radius = 2f)
        {
            // Grab player's position used as a reference
            Vector3 playerPos = PlayerPed.Position;

            // Retrieve the list of players that in server (can't use BaseScript when using static classes)
            PlayerList Players = PlayerList.Players;

            // Initialize a variable to track the closest player
            Player closestPlayer = null;

            // Iterate through all available players
            foreach (Player p in Players)
            {
                // Skip invalid or self-references
                if (p is null || p == Game.Player || !Entity.Exists(p.Character))
                {
                    continue;
                }

                // Calculate the distance between the player and the reference player
                float distance = p.Character.Position.DistanceTo2d(playerPos);

                // Update the closest player if a closer one is found within the specified radius
                if (distance < radius)
                {
                    closestPlayer = p;
                }
            }

            // Return the closest player, or null if none were found within the radius
            return closestPlayer;
        }
        #endregion

        #region Vehicle Extensions
        /// <summary>
        /// Gets the closest vehicle to a player with a radius (2 meters by default)
        /// Given to me by Traditionalism (https://github.com/traditionalism)
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Vehicle GetClosestVehicle(this Ped ped, float radius = 2f)
        {
            // Get the players's position for raycasting
            Vector3 plyrPos = ped.Position;

            // Perform a capsule-shaped raycast to detect nearby vehicles
;           RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, (IntersectOptions)10, Game.PlayerPed);

            // Return the hit entity as a vehicle, or null if none found | could this be shortened?
            return raycast.DitHitEntity && Entity.Exists(raycast.HitEntity) && raycast.HitEntity.Model.IsVehicle ? (Vehicle)raycast.HitEntity: null;
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
                return (float)Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2)); // best to use the distance formula for this.
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

        public static float CalculateDifferenceBetweenHeadings(this Entity entity1, Entity entity2)
        {
            if (entity1 is null || !entity1.Exists() || entity2 is null || entity2.Exists())
            {
                return 0f;
            }

            Vector3 h1 = entity1.ForwardVector;
            Vector3 h2 = entity2.ForwardVector;

            float headingDegreesE1 = MathUtil.Wrap(MathUtil.RadiansToDegrees((float)Math.Atan2(h1.Y, h1.X)), 0f, 360f);
            float headingDegreesE2 = MathUtil.Wrap(MathUtil.RadiansToDegrees((float)Math.Atan2(h2.Y, h2.X)), 0f, 360f);

            float difference = headingDegreesE1 - headingDegreesE2;
            difference = MathUtil.Wrap(difference, -180, 180);

            return difference;
        }
        #endregion

        #region Misc Extensions
        /// <summary>
        /// Calculates the distance to a blip position.
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        public static float CalculateDistanceTo(this Blip blip, Vector3 targetPos)
        {
            if (DoesBlipExist(blip.Handle))
            {
                // Grab blip position
                Vector3 blipPos = blip.Position;

                // Get the distance between the blip and the target position
                float distance = Vdist(blipPos.X, blipPos.Y, blipPos.Z, targetPos.X, targetPos.Y, targetPos.Z);

                return distance; // return the distance
            }
            else
            {
                return -1; // If the blip doesn't exist return as negitive.
            }
        }

        public static float CalculateDistanceTo(this Blip blip, float x, float y, float z) => CalculateDistanceTo(blip, new(x, y, z));
        #endregion
    }
}
