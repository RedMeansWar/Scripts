using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Object
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
    }
}
