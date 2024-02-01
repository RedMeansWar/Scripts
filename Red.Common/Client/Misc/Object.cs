using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

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
        /// Creates a prop using the CreateObject method in the FiveM API
        /// </summary>
        /// <param name="model"></param>
        /// <param name="postition"></param>
        /// <param name="isNetwork"></param>
        /// <param name="netMissionEntity"></param>
        /// <param name="doorEntity"></param>
        /// <returns>A prop that is created spawned at specific position</returns>
        public static int CreateProp(string model, Vector3 postition, bool isNetwork = false, bool netMissionEntity = false, bool doorEntity = false) => CreateObject(GetHashKey(model), postition.X, postition.Y, postition.Z, isNetwork, netMissionEntity, doorEntity);

        public static int CreateProp(int modelHash, Vector3 position, bool isNetwork = false, bool netMissionEntity = false, bool doorEntity = false) => CreateObject(modelHash, position.X, position.Y, position.Z, isNetwork, netMissionEntity, doorEntity);

        public static int CreateProp(string model, float x, float y, float z, bool isNetwork = false, bool netMissionEntity = false, bool doorEntity = false) => CreateProp(model, new Vector3(x, y, z), isNetwork, netMissionEntity, doorEntity);

        public static int CreateProp(int modelHash, float x, float y, float z, bool isNetwork = false, bool netMissionEntity = false, bool doorEntity = false) => CreateProp(modelHash, x, y, z, isNetwork, netMissionEntity, doorEntity);

        /// <summary>
        /// Creates a prop and places it on the ground properly.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <param name="physics"></param>
        /// <returns></returns>
        public static Task<Prop> CreatePropOnGround(Model model, Vector3 position, bool physics = false) => CreateProp(model, position, physics, true);

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

        /// <summary>
        /// Loads a prop using a model's hash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task LoadModel(uint model)
        {
            RequestModel(model);
            while (!HasModelLoaded(model))
            {
                await Delay(0);
            }
        }

        /// <summary>
        /// Loads a prop using a string model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task LoadModel(string model) => await LoadModel((uint)GetHashKey(model));
    }
}
