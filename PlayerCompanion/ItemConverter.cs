using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace PlayerCompanion
{
    /// <summary>
    /// Serializes and Deserializes inventory items.
    /// </summary>
    public class ItemConverter : JsonConverter<Item>
    {
        /// <summary>
        /// Reads an <see cref="Item"/>.
        /// </summary>
        /// <returns></returns>
        public override Item ReadJson(JsonReader reader, Type objectType, Item existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Load the entire thing into a JSON Object
            JObject @object = JObject.Load(reader);
            // Try to get the type of the object
            Type type = Type.GetType((string)@object["type"], false);
            // If no item was found, return null
            if (type == null)
            {
                return null;
            }
            // Otherwise, create a new instance and return a generic inventory item
            return (Item)Activator.CreateInstance(type, (int)@object["count"]);
        }
        /// <summary>
        /// Writes an <see cref="Item"/>.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            // Create a new JObject with the values
            JObject @object = new JObject
            {
                ["type"] = value.GetType().ToString(),
                ["count"] = value.Count
            };
            // And save it
            @object.WriteTo(writer);
        }
    }
}
