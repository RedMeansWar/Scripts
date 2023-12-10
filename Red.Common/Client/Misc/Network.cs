using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Network : BaseScript
    {
        public static async Task RequestControlOfEntity(Entity entity)
        {
            NetworkRequestControlOfEntity(entity.Handle);

            for (int i = 4; !NetworkHasControlOfEntity(entity.Handle) && i > 0; --i)
            {
                await Delay(250);
            }

            if (NetworkHasControlOfEntity(entity.Handle))
            {
                return;
            }

            Debug.WriteLine("unable to get control of entity");
        }

        public static async Task<Entity> GetEntityFromNetId(int networkId, bool networkControl = true)
        {
            if (networkId == 0 || !NetworkDoesNetworkIdExist(networkId))
            {
                Debug.WriteLine($"Couldn't request network Identifier: {networkId} because it doesn't exist.");
                return null;
            }

            if (networkControl)
            {
                int timeout = 0;
                while (!NetworkHasControlOfNetworkId(networkId) && timeout < 4)
                {
                    timeout++;
                    NetworkRequestControlOfEntity(networkId);
                    await Delay(500);
                }

                if (NetworkHasControlOfNetworkId(networkId))
                {
                    Debug.WriteLine($"Could not request control of Network Identifier: {networkId}.");
                    return null;
                }
            }

            return Entity.FromNetworkId(networkId);
        }

        public static Task<Entity> GetEntityFromNetworkId(int networkId, bool networkControl = true) => GetEntityFromNetId(networkId, networkControl);
        public static Task<Entity> GetEntityFromNetwork(int networkId, bool networkControl = true) => GetEntityFromNetId(networkId, networkControl);
    }
}
