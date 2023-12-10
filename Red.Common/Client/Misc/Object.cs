using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using static Red.Common.Client.Client;

namespace Red.Common.Client.Misc
{
    public class Object : BaseScript
    {
        public static Task<Prop> CreateProp(Model model, Vector3 position, bool physics = false, bool placeOnGround = false) => World.CreateProp(model, position, physics, placeOnGround);
        public static Task<Prop> CreatePropOnGround(Model model, Vector3 position, bool physics = false) => CreateProp(model, position, physics, true);
        /// <summary>
        /// Reqyest a prop model by hash.
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
    }
}
