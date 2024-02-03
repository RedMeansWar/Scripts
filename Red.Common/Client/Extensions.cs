using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
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
        /// <returns>If the ped can do an action or not.</returns>
        public static bool CannotDoAction(this Ped ped) => 
           ped.IsCuffed || ped.IsDead || ped.IsBeingStunned
           || ped.IsClimbing || ped.IsDiving || ped.IsFalling
           || ped.IsGettingIntoAVehicle || ped.IsJumping
           || ped.IsJumpingOutOfVehicle || ped.IsRagdoll
           || ped.IsSwimmingUnderWater || ped.IsVaulting;

        /// <summary>
        /// Finds the closest player to the given player within a specified radius.
        /// </summary>
        /// <param name="player">The player to search around.</param>
        /// <param name="radius">The search radius (default: 2f).</param>
        /// <returns>The closest player found, or null if none is within the radius.</returns>
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
        /// Finds the closest vehicle within a specified radius of a ped.
        /// </summary>
        /// <param name="ped">The ped to search around.</param>
        /// <param name="radius">The search radius (default: 2f).</param>
        /// <returns>The closest vehicle found, or null if none is within the radius.</returns>
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
        /// Converts a speed value from meters per second to miles per hour.
        /// </summary>
        /// <param name="speed">The speed in meters per second.</param>
        /// <returns>The speed in miles per hour.</returns>
        public static float ConvertToMPH(this float speed) => speed * 2.236936f;

        /// <summary>
        /// Converts a speed value from miles per hour to meters per second.
        /// </summary>
        /// <param name="speed">The speed in miles per hour.</param>
        /// <returns>The speed in meters per second.</returns>
        public static float ConvertFromMPH(this float speed) => speed * 0.44704f;

        /// <summary>
        /// Converts a speed value from meters per second to kilometers per hour.
        /// </summary>
        /// <param name="speed">The speed in meters per second.</param>
        /// <returns>The speed in kilometers per hour.</returns>
        public static float ConvertToKPH(this float speed) => speed * 3.6f;

        /// <summary>
        /// Turns off the engine of a vehicle.
        /// </summary>
        /// <param name="vehicle">The vehicle to turn the engine off for.</param>
        public static void TurnOffEngine(this Vehicle vehicle)
        {
            // Set the vehicle's engine state to off, ensuring immediate effect and sound playback.
            SetVehicleEngineOn(vehicle.Handle, false, false, true);  // Force engine off, no fade, play sound
        }

        /// <summary>
        /// Teleports a ped to the specified coordinates, handling network synchronization and visual effects.
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static async Task TeleportToCoords(this Ped ped, Vector3 pos)
        {
            // Fade out the ped on the network to mask the teleportation.
            NetworkFadeOutEntity(ped.Handle, true, false);

            // Wait for the fade-out to complete before proceeding.
            while (NetworkIsEntityFading(ped.Handle))
            {
                await BaseScript.Delay(0);
            }

            // Ensure colliision is loaded at the target position for seamless transition
            RequestCollisionAtCoord(pos.X, pos.Y, pos.Z);

            // Initiate a new scene load focused around the target coordinates.
            NewLoadSceneStart(pos.X, pos.Y, pos.Z, pos.X, pos.Y, pos.Z, 3f, 0);

            // Track the scene loading time for potential timeout.
            int startTime = Game.GameTime;

            // Wait for the scene loading to complete, with a timeout safeguard.
            while (IsNetworkLoadingScene())
            {
                if (GetGameTimer() - startTime > 2000)
                {
                    break; // Exit the loop if loading takes to long.
                }

                await BaseScript.Delay(0); // Yield control during loop
            }

            // Initiate the actual ped teleportation process.
            StartPlayerTeleport(ped.Handle, pos.X, pos.Y, pos.Z, GetEntityHeightAboveGround(ped.Handle), false, true, true);

            // Wait for the teleportation to complete.
            while (IsPlayerTeleportActive())
            {
                await BaseScript.Delay(0); // Yield control during teleportation.
            }

            // Fade the ped back in on the network for a smooth visual transition.
            NetworkFadeInEntity(ped.Handle, false);

            // Fade in the screen for visual consistency.
            Screen.Fading.FadeIn(500);
        }
        #endregion

        #region Distance Calculations
        /// <summary>
        /// Calculates the squared distance between a blip and a target position.
        /// </summary>
        /// <param name="blip">The blip to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The squared distance between the blip and the target position.</returns>
        public static float CalculateDistanceTo(this Blip blip, Vector3 targetPos)
        {
            // Retrieve the blip's position in the world.
            Vector3 blipLocation = blip.Position;

            // Calculate the distance between the blip and the target position.
            float distanceSquared = Vector3.Distance(blipLocation, targetPos);

            return distanceSquared;
        }

        /// <summary>
        /// Calculates the squared distance between a ped and a target position.
        /// </summary>
        /// <param name="blip">The blip to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the ped and the target position.</returns>
        public static float CalculateDistanceTo(this Ped ped, Vector3 targetPos)
        {
            // Retrieve the ped's position in the world.
            Vector3 pedPosition = ped.Position;

            // Calculate the distance between the ped and the target position.
            float distance = Vector3.Distance(pedPosition, targetPos);

            return distance;
        }

        /// <summary>
        /// Calculates the distance between a vehicle and a target position.
        /// </summary>
        /// <param name="vehicle">The vehicle to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the vehicle and the target position.</returns>
        public static float CalculeDistanceTo(this Vehicle vehicle, Vector3 targetPos)
        {
            // Retrieve the vehicle's position in the world.
            Vector3 vehPosition = vehicle.Position;

            // Calculate the distance between the vehicle and the target position.
            float distance = Vector3.Distance(vehPosition, targetPos);

            return distance;
        }

        /// <summary>
        /// Calculates the distance between a prop and a target position.
        /// </summary>
        /// <param name="prop">The prop to calculate the distance from.</param>
        /// <param name="targetPos">The target position to measure the distance to.</param>
        /// <returns>The distance between the prop and the target position.</returns>
        public static float CalculeDistanceTo(this Prop prop, Vector3 targetPos)
        {
            // Retrieve the prop's position in the world.
            Vector3 propPos = prop.Position;

            // Calculate the distance between the prop and the target position.
            float distance = Vector3.Distance(propPos, targetPos);

            return distance;
        }

        #endregion

        #region Misc Extensions
        /// <summary>
        /// Retrieves a value of the specified type from a dictionary, providing a default value if not found or of the wrong type.
        /// Seemed to be used in NUI with JavaScript to C# communication.
        /// </summary>
        /// <typeparam name="T">The type of value to retrieve.</typeparam>
        /// <param name="dict">The dictionary to retrieve the value from.</param>
        /// <param name="key">The key associated with the value.</param>
        /// <param name="defaultVal">The default value to return if the key is not found or the value is not of the expected type.</param>
        /// <returns>The value of type T associated with the key, or the default value if not found or of the wrong type.</returns>
        public static T GetVal<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            // Attempt to retrieve the value efficiently using TryGetValue.
             if (dict.TryGetValue(key, out object value) && value is T t) 
            {
                return t; // Return the value if it matches the expected type.
            }

            // If the key wasn't found or the value wasn't of the expected type, return the default value.
            return defaultVal;
        }

        /// <summary>
        /// Finds the closest prop within a specified radius of a ped.
        /// </summary>
        /// <param name="ped">The ped to search around.</param>
        /// <param name="radius">The search radius (default: 5f).</param>
        /// <returns>The closest prop found, or null if none is within the radius.</returns>
        public static Prop GetClosestPropToClient(this Ped ped, float radius = 5f)
        {
            // Get the ped's position for raycasting.
            Vector3 plyrPos = ped.Position;

            // Perform a capsule-shaped raycast to detect nearby props.
            RaycastResult raycast = World.RaycastCapsule(plyrPos, plyrPos, radius, IntersectOptions.Objects, Game.PlayerPed);

            // Validate raycast results and entity type.
            if (!Entity.Exists(raycast.HitEntity) || !raycast.HitEntity.Model.IsProp || !raycast.DitHitEntity)
            {
                // Return null if no valid prop was detected.
                return null;
            }
            else
            {
                // Cast the hit entity to a prop and return it.
                return (Prop)raycast.HitEntity;
            }
        }
        #endregion

        #region Animations
        /// <summary>
        /// Plays an animation on a ped with extensive control over blending, duration, flags, playback rate, and locking.
        /// </summary>
        /// <param name="ped">The ped to play the animation on.</param>
        /// <param name="dictionary">The animation dictionary containing the animation.</param>
        /// <param name="name">The name of the animation to play.</param>
        /// <param name="blendInSpeed">The speed at which to blend in the animation (0.0 to 1.0).</param>
        /// <param name="blendOutSpeed">The speed at which to blend out the animation (0.0 to 1.0).</param>
        /// <param name="duration">The duration of the animation in milliseconds (-1 for default).</param>
        /// <param name="flags">Flags controlling animation behavior (see AnimationFlags).</param>
        /// <param name="playbackRate">The playback rate of the animation (1.0 is normal speed).</param>
        /// <param name="lockX">Whether to lock the ped's X position during the animation.</param>
        /// <param name="lockY">Whether to lock the ped's Y position during the animation.</param>
        /// <param name="lockZ">Whether to lock the ped's Z position during the animation.</param>
        public static void PlayAnim(this Ped ped, string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, AnimationFlags flags, float playbackRate, bool lockX, bool lockY, bool lockZ)
        {
            // Utilize the underlying TaskPlayAnim function for animation playback.
            TaskPlayAnim(
                ped.Handle,  // Ped handle
                dictionary,  // Animation dictionary
                name,        // Animation name
                blendInSpeed,  // Blend in speed
                blendOutSpeed, // Blend out speed
                duration,      // Duration
                (int)flags,   // Animation flags
                playbackRate, // Playback rate
                lockX,        // Lock X position
                lockY,        // Lock Y position
                lockZ         // Lock Z position
            );
        }

        public static void PlayAnim(this Ped ped, string dictionary, string name, float blendInSpeed, float blendOutSpeed, int duration, int flags, float playbackRate, bool lockX, bool lockY, bool lockZ)
        {
            // Utilize the underlying TaskPlayAnim function for animation playback.
            TaskPlayAnim(
                ped.Handle,  // Ped handle
                dictionary,  // Animation dictionary
                name,        // Animation name
                blendInSpeed,  // Blend in speed
                blendOutSpeed, // Blend out speed
                duration,      // Duration
                flags,   // Animation flags
                playbackRate, // Playback rate
                lockX,        // Lock X position
                lockY,        // Lock Y position
                lockZ         // Lock Z position
            );
        }

        /// <summary>
        /// Modified version of blip.Scale
        /// </summary>
        /// <param name="blip"></param>
        /// <param name="size"></param>
        public static void Size(this Blip blip, float size = 1f) => blip.Scale = size;
        #endregion
    }
}
