using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;

namespace Red.Common.Client.Misc
{
    public class Object : ClientScript
    {
        #region Variables
        protected static Prop visualizedProp;
        #endregion
        /// <summary>
        /// Creates a prop.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="physics"></param>
        /// <param name="placeOnGround"></param>
        /// <returns></returns>
        public static Task<Prop> CreateProp(Model model, Vector3 position, bool physics = false, bool placeOnGround = false) => World.CreateProp(model, position, physics, placeOnGround);

        /// <summary>
        /// Creates a prop and places it on the ground properly.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public static Task<Prop> CreatePropOnGround(Model model, Vector3 position, bool physics = false) => CreateProp(model, position, physics, true);

        /// <summary>
        /// Spawns a prop in front of the player for visualization purposes, handling collision checks and vehicle restrictions.
        /// </summary>
        /// <param name="model">The model name of the prop to spawn.</param>
        /// <param name="opacity">The opacity of the prop (default: 75).</param>
        public static void SpawnVisualizedProp(string model, int opacity = 75)
        {
            // Get the player's position for prop placement.
            Vector3 playerPosition = Game.PlayerPed.Position;

            // Perform a capsule-shaped raycast to check for nearby obstructing entities.
            RaycastResult raycastResult = World.RaycastCapsule(
                playerPosition,
                playerPosition,
                2f,  // Raycast radius
                IntersectOptions.Everything,  // Check for all entity types
                Game.PlayerPed  // Ignore the player's ped to avoid self-intersection
            );

            // Spawn the prop only if no entities are obstructing the space in front of the player.
            if (!raycastResult.DitHitEntity)
            {
                // Calculate a position slightly in front and above the player.
                Vector3 propPosition = Game.PlayerPed.GetOffsetPosition(new Vector3(0.0f, 1.2f, 1.32f));

                // Create the prop with specified properties and adjustments for visualization.
                visualizedProp = new(CreateObjectNoOffset((uint)GetHashKey(model), propPosition.X, propPosition.Y, propPosition.Z, false, false, false))
                {
                    Rotation = Game.PlayerPed.Rotation + new Vector3(-20f, 0f, 0f),  // Rotate slightly for visual clarity
                    IsCollisionEnabled = false,  // Disable collision to avoid physical interactions
                    Opacity = opacity  // Set the desired opacity for visual effect
                };
            }

            // If the player is in a vehicle, display an error notification.
            if (Game.PlayerPed.IsInVehicle())
            {
                ErrorNotification("You can't be in a vehicle to use this.", false);
            }
        }

        /// <summary>
        /// Deletes the closest visualized prop
        /// </summary>
        public static void DeleteVisualizedProp()
        {
            if (Entity.Exists(visualizedProp)) // Check if a visualized prop exists.
            {
                // Delete the prop
                visualizedProp.Delete();
                
                // return the prop as null
                visualizedProp = null;
            }
        }

        /// <summary>
        /// Deletes the closest prop to the player within a specified radius, waiting for network confirmation.
        /// </summary>
        /// <param name="radius">The search radius (default: 2f).</param>
        public static void DeleteClosestProp(float radius = 2f)
        {
            // Find the closest prop within the radius.
            Prop closestProp = Game.PlayerPed.GetClosestPropToClient(radius);

            // If no prop was found, return without action.
            if (closestProp is null)
            {
                return;
            }

            // Request control of the prop from the network.
            NetworkRequestControlOfEntity(closestProp.Handle);

            // Delete the prop if control was successfully obtained.
            if (NetworkHasControlOfEntity(closestProp.Handle))
            {
                closestProp.Delete(); // Use the prop object directly for deletion
            }
        }
    }
}
