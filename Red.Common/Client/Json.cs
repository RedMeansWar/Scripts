using System;
using Newtonsoft.Json;

namespace Red.Common.Client
{
    public static class Json
    {
        /// <summary>
        /// Parses a JSON string into an object of the specified type, handling empty strings and potential exceptions gracefully.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into. Must be a class.</typeparam>
        /// <param name="json">The JSON string to parse.</param>
        /// <returns>The deserialized object of type T, or null if the input string is empty, invalid, or an exception occurs.</returns>
        public static T Parse<T>(string json) where T : class
        {
            // Handle empty or whitespace-only strings:
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            T obj;

            try
            {
                // Configure serialization settings to prevent issues with circular references:
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                // Attempt to deserialize the JSON string:
                obj = JsonConvert.DeserializeObject<T>(json, settings);
            }
            catch (Exception)
            {
                // Handle any exceptions that occur during deserialization:
                obj = null;
            }

            return obj;
        }

        /// <summary>
        /// Serializes an object into a JSON string, handling null inputs and potential exceptions gracefully.
        /// </summary>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The JSON string representing the object, or null if the input object is null or an exception occurs.</returns>
        public static string Stringify(object data)
        {
            // Handle null input:
            if (data is null)
            {
                return null;
            }

            string json;

            try
            {
                // Configure serialization settings to prevent issues with circular references:
                JsonSerializerSettings settings = new()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                };

                // Attempt to serialize the object:
                json = JsonConvert.SerializeObject(data, settings);
            }
            catch (Exception)
            {
                // Handle any exceptions that occur during serialization:
                json = null;
            }

            return json;
        }
    }
}
