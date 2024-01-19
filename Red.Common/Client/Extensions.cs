using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client
{
    public static class Extensions
    {
        #region Ped Actions
        /// <summary>
        /// Determines if the Client cannot do a certain action.
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
        /// </summary>
        /// <param name="player"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Player GetClosestPlayerToClient(this Player player, float radius = 2f)
        {
            // Get the player's position for distance calculations.
            Vector3 playerPos = Game.PlayerPed.Position;

            // Initialize variables to track the closest player and distance.
            Player closestPlayer = null;
            float closestDist = float.MaxValue; // start with the maxium possible distance

            // Iterate through all players
            foreach (Player plyr in PlayerList.Players)
            {
                // Skip over players that are null or any self-references, and players without characters.
                if (plyr is null || plyr == Game.Player || !Entity.Exists(plyr.Character))
                {
                    continue; // continue if the all the condition aren't met
                }

                // Calculate the squared distance between the player and the secondary player.
                float distance = Vector3.DistanceSquared(plyr.Character.Position, playerPos);

                // Update the closest player and distance if a closer one is found within the specified radius.
                if (distance < closestDist && distance < radius)
                {
                    closestPlayer = plyr;
                    closestDist = distance;
                }
            }

            // Return the closest player, or null if none were found within the radius.
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
        public static Vehicle GetClosestVehicleToClient(this Ped ped, float radius = 2f)
        {
            // Gets the players position
            Vector3 plyrPos = ped.Position;

            // Perform a capsule-shaped raycast to detect and hit a vehicle that is a radius.
            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, (IntersectOptions)10, Game.PlayerPed);

            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsVehicle || !raycast.DitHitEntity)
            {
                // return null if there is no vehicle or raycast didn't hit the vehicle.
                return null;
            }
            else
            {
                // return the raycast that hit the vehicle.
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
        /// Modified Version of TaskPlayAnimation
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blendInSpeed"></param>
        /// <param name="blendOutSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        /// <param name="playbackRate"></param>
        /// <param name="lockX"></param>
        /// <param name="lockY"></param>
        /// <param name="lockZ"></param>
        public static void PlayAnim(this Ped ped, string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, AnimationFlags flags, float playbackRate, bool lockX, bool lockY, bool lockZ) => TaskPlayAnim(ped.Handle, dictionary, name, blendInSpeed, blendOutSpeed, duration, (int)flags, playbackRate, lockX, lockY, lockZ);
        /// <summary>
        /// Modified Version of TaskPlayAnimation
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="dictionary"></param>
        /// <param name="name"></param>
        /// <param name="blendInSpeed"></param>
        /// <param name="blendOutSpeed"></param>
        /// <param name="duration"></param>
        /// <param name="flags"></param>
        /// <param name="playbackRate"></param>
        /// <param name="lockX"></param>
        /// <param name="lockY"></param>
        /// <param name="lockZ"></param>
        public static void PlayAnim(this Ped ped, string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, int flags, float playbackRate, bool lockX, bool lockY, bool lockZ) => TaskPlayAnim(ped.Handle, dictionary, name, blendInSpeed, blendOutSpeed, duration, flags, playbackRate, lockX, lockY, lockZ);
        #endregion

        #region Misc Extensions
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
        /// <summary>
        /// Gets the closest prop to the player using a raycast.
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Prop GetClosestPropToClient(this Ped ped, float radius = 5f)
        {
            Vector3 plyrPos = ped.Position;

            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, IntersectOptions.Objects, Game.PlayerPed);

            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsProp || !raycast.DitHitEntity)
            {
                return null;
            }
            else
            {
                return (Prop)raycast.HitEntity;
            }
        }
        #endregion
    }
}
