using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Red.Framework.Client
{
    public static class Json
    {
        public static T Parse<T>(string json) where T : class
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            T obj;

            try
            {
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                obj = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception)
            {
                obj = null;
            }

            return obj;
        }

        public static string Stringify(object data)
        {
            if (data is null)
            {
                return null;
            }

            string json;

            try
            {
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                json = JsonConvert.SerializeObject(data, settings);
            }
            catch (Exception)
            {
                json = null;
            }

            return json;
        }
    }
}