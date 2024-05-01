using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Network : BaseScript
    {
        /// <summary>
        /// Requests control of an entity from the network and awaits confirmation.
        /// </summary>
        /// <param name="entity">The entity to request control of.</param>
        /// <returns>A task that completes when control is granted or fails.</returns>
        public static async Task RequestControlOfEntity(Entity entity)
        {
            // Initiate the network request for entity control.
            NetworkRequestControlOfEntity(entity.Handle);

            // Await control confirmation with a timeout mechanism.
            for (int i = 4; !NetworkHasControlOfEntity(entity.Handle) && i > 0; --i)
            {
                // Wait briefly for control to be granted.
                await Delay(250);  // Delay 250 milliseconds
            }

            // If control was obtained, return successfully.
            if (NetworkHasControlOfEntity(entity.Handle))
            {
                return;
            }

            // Otherwise, log a warning message indicating failure.
            Debug.WriteLine("Unable to get control of entity");
        }

        /// <summary>
        /// Gets a entity from their Network Id.
        /// </summary>
        /// <param name="networkId"></param>
        /// <param name="networkControl"></param>
        /// <returns></returns>
        public static async Task<Entity> GetEntityFromNetId(int networkId, bool networkControl = true)
        {
            // Validate network ID existence
            if (networkId == 0 || !NetworkDoesNetworkIdExist(networkId))
            {
                Debug.WriteLine($"Couldn't request network Identifier: {networkId} because it doesn't exist.");
                return null; // Return null if invalid or non-existent
            }

            // Optionally acquire network control of the entity
            if (networkControl)
            {
                // Attempt to gain control up to 4 times with delays should more be added?
                int timeout = 0;
                while (!NetworkHasControlOfNetworkId(networkId) && timeout < 4)
                {
                    timeout++;
                    NetworkRequestControlOfEntity(networkId);
                    await Delay(500); // Wait for control response
                }

                if (NetworkHasControlOfNetworkId(networkId))
                {
                    Debug.WriteLine($"Could not request control of Network Identifier: {networkId}.");
                    return null; // return as null if the entity doesn't exist
                }
            }

            return Entity.FromNetworkId(networkId);
        }
    }
}