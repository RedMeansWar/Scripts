using CitizenFX.Core;
using System.Collections.Generic;

namespace Red.Framework
{
    public static class Extensions
    {
        public static T GetValue<T>(this IDictionary<string, object> dict, string key, T defaultVal)
        {
            if (dict.ContainsKey(key))
                if (dict[key] is T)
                    return (T)dict[key];
            return defaultVal;
        }
    }
}