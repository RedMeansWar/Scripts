using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using CitizenFX.Core;
using Mono.CSharp;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public static class Extensions
    {
        #region Ped Actions
        /// <summary>
        /// Determines if the Client cannot do a certain action.
        /// DecorGetBool(ped.Handle, "IsDead") seems to work fine, is there an alternative?
        /// </summary>
        /// <param name="ped"></param>
        /// <returns></returns>
        public static bool CannotDoAction(this Ped ped) => ped.IsCuffed || ped.IsDead || ped.IsBeingStunned
           || ped.IsClimbing || ped.IsDiving || ped.IsFalling
           || ped.IsGettingIntoAVehicle || ped.IsJumping
           || ped.IsJumpingOutOfVehicle || ped.IsRagdoll
           || ped.IsSwimmingUnderWater || ped.IsVaulting;
        /// <summary>
        /// Gets the Closest Player to the client with a predefined radius (2 meters by default)
        /// Given to me by Traditionalism (https://github.com/traditionalism)
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToClient(this Player player, float radius = 2f)
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            Player closestPlayer = null;
            float closestDist = float.MaxValue;

            foreach (Player plyr in PlayerList.Players)
            {
                if (plyr is null || plyr == Game.Player || !Entity.Exists(plyr.Character))
                {
                    continue;
                }

                float distance = Vector3.DistanceSquared(plyr.Character.Position, playerPos);

                if (distance < closestDist && distance < radius)
                {
                    closestPlayer = plyr;
                    closestDist = distance;
                }
            }

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
        public static Vehicle GetClosestVehicleToClient(this Ped ped, float radius = 2f)
        {
            Vector3 plyrPos = ped.Position;
            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, (IntersectOptions)10, Game.PlayerPed);

            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsVehicle || !raycast.DitHitEntity)
            {
                return null;
            }
            else
            {
                return (Vehicle)raycast.HitEntity;
            }
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
        /// <summary>
        /// Calculates MPS to KPH
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static float ConvertToKPH(this float speed) => speed * 3.6f;
        #endregion

        #region Distance Calculations
        /// <summary>
        /// Calculates the distance between headings
        /// needs more testing
        /// </summary>
        /// <param name="entity1"></param>
        /// <param name="entity2"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Gets the distance to a prop model.
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static float GetDistanceToProp(this Prop prop, string modelName)
        {
            Ped PlayerPed = Game.PlayerPed;

            if (prop is null)
            {
                BaseScript.Delay(250);
                return 0f; // Return 0f if null
            }

            // The prop to get the distance to. Is this one prop or all props in the world?
            prop = World.GetAllProps().Where(prop => prop.Model == modelName).OrderBy(prop => Vector3.DistanceSquared(prop.Position, PlayerPed.Position)).FirstOrDefault();

            // gets the distance from the prop to the player
            float distance = Vector3.DistanceSquared(PlayerPed.Position, prop.Position);

            return distance;
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
            Vector3 blipLocation = blip.Position;
            float distance = Vector3.DistanceSquared(blipLocation, targetPos);

            return distance;
        }
        /// <summary>
        /// Calculates the distance to a blip position.
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static float CalculateDistanceTo(this Blip blip, float x, float y, float z) => CalculateDistanceTo(blip, new(x, y, z));
        /// <summary>
        /// Mainly used for NUI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <param name="key"></param>
        /// <param name="defaultVal"></param>
        /// <returns></returns>
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            // Attempt to retrieve the value using a TryGetValue for efficiency
            if (dict.TryGetValue(key, out object value) && value is T t) // Check if the retrieved value is of the expected type
            {
                // Return the value if it matches the expected type
                return t;
            }
            // If the key wasn't found or the value wasn't of the expected type, return the default value
            return defaultVal;
        }
        #endregion
    }
}
