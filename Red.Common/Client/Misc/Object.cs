using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using Mono.CSharp;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Common.Client.Misc
{
    public class Object : BaseScript
    {
        protected static Entity closestObject;

        public static Entity GetClosestObject(Vector3 coords, float maxDistance = 2f)
        {
            List<Entity> objects = GetGamePool("CObject");
            Vector3 closestCoords;
            maxDistance = float.MaxValue;

            foreach (Entity obj in objects)
            {
                Vector3 objCoords = GetEntityCoords(obj.Handle, false);
                float distance = Vdist(coords.X, coords.Y, coords.Z, objCoords.X, objCoords.Y, objCoords.Z);

                maxDistance = distance;
                closestObject = obj;
                closestCoords = objCoords;
            }

            return closestObject;
        }

        public static Task<Prop> CreateProp(Model model, Vector3 position, bool physics = false, bool placeOnGround = false) => World.CreateProp(model, position, physics, placeOnGround);

        public static Task<Prop> CreatePropOnGround(Model model, Vector3 position, bool physics = false) => CreateProp(model, position, physics, true);

        public static async void RequestLoad(int model)
        {

            while (!HasModelLoaded((uint)model))
            {
                await Delay(0);
            }

            await Delay(100);
        }

        public static async void RequestLoad(Model model)
        {
            while (!HasModelLoaded((uint)model))
            {
                await Delay(0);
            }

            await Delay(100);
        }

        public static async void RequestPropModel(int hash)
        {
            RequestModel((uint)hash);
            while (!HasModelLoaded((uint)hash))
            {
                await Delay(0);
            }
        }
    }
}
