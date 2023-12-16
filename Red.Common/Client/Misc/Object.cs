using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Hud.HUD;

namespace Red.Common.Client.Misc
{
    public class Object : BaseScript
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
        /// Request a prop model by hash.
        /// </summary>
        /// <param name="hash"></param>
        public static async void RequestPropModel(uint hash)
        {
            RequestModel(hash);
            while (!HasModelLoaded(hash))
            {
                await Delay(100);
            }

            await Delay(0);
        }
        /// <summary>
        /// Spawns a prop with a transparent prop and attaches it to the player.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="opacity"></param>
        public static void SpawnVisualizedProp(string model, int opacity = 75)
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            RaycastResult raycast = World.RaycastCapsule(playerPos, playerPos, 2f, (IntersectOptions)10, Game.PlayerPed);
            
            if (!raycast.DitHitEntity)
            {
                Vector3 propPos = Game.PlayerPed.GetOffsetPosition(new(0.0f, 1.2f, 1.32f));
                visualizedProp = new(CreateObjectNoOffset((uint)GetHashKey(model), propPos.X, propPos.Y, propPos.Z, false, false, false))
                {
                    Rotation = Game.PlayerPed.Rotation + new Vector3(-20f, 0f, 0f),
                    IsCollisionEnabled = false,
                    Opacity = opacity
                };
            }

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
            if (Entity.Exists(visualizedProp))
            {
                visualizedProp.Delete();
                visualizedProp = null;
            }
        }
        /// <summary>
        /// Deletes the closest prop to the player.
        /// </summary>
        public static void DeleteClosestProp(float radius = 2f)
        {
            Vector3 playerPos = Game.PlayerPed.Position;
            Prop closestProp = null;

            for (int i = 0; i < World.GetAllProps().Length; i++)
            {
                Prop prop = World.GetAllProps()[i];
                Vector3 propPos = GetEntityCoords(prop.Handle, false);

                float distance = Vector3.Distance(playerPos, propPos);

                if (distance <= radius)
                {
                    closestProp = prop;
                    closestProp.Delete();
                    break;
                }
            }

            if (closestProp is null)
            {
                return;
            }
        }
    }
}
