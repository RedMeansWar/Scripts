using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Object
    {
        #region Props
        public static async Task<Prop> SpawnPropOnPlayer(string model, bool physics = false, bool placeOnGround = true)
        {
            if (Game.PlayerPed.IsInVehicle())
            {
                return null;
            }

            Prop createdProp = await World.CreateProp(new(model), Game.PlayerPed.Position, false, false);

            return createdProp;
        }

        public static async Task<Prop> SpawnProp(string model, Vector3 position, bool physics = false, bool placeOnGround = false) => await World.CreateProp(new(model), position, physics, placeOnGround);
        #endregion
    }
}
