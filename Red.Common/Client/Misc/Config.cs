using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace Red.Common.Client.Misc
{
    public class Config
    {
        #region Variables
        protected static string resourceName = GetCurrentResourceName();
        #endregion

        public static int GetConfigValue(string key, int defaultValue)
        {
            string val = GetResourceMetadata(resourceName, key, GetNumResourceMetadata(resourceName, key) - 1);

            if (int.TryParse(val, out int result))
            {
                return result;
            }

            Log.Error($"Failed to parse config value '{val}' for metadata key.");
            return defaultValue;
        }
    }
}
