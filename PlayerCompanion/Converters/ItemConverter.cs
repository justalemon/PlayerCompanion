using GTA.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace PlayerCompanion.Converters
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

            // If there is no Type in the object, raise an exception
            if (!@object.ContainsKey("type"))
            {
                throw new KeyNotFoundException("There was no Type specified in the class.");
            }

            // Get the count of this item
            int count = @object.ContainsKey("count") ? (int)@object["count"] : 0;

            // Try to get the type of the object
            Type type = Type.GetType((string)@object["type"], false);
            // If the type was not found, return an invalid type
            if (type == null)
            {
                Notification.Show($"~r~Danger~s~: {(string)@object["type"]} was not found. Make sure that all of the required mods are installed and try again.");
                return new InvalidItem((string)@object["type"], count);
            }
            // Otherwise, create a new instance and return a generic inventory item
            return (Item)Activator.CreateInstance(type, (int)@object["count"]);
        }
        /// <summary>
        /// Writes an <see cref="Item"/>.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Item value, JsonSerializer serializer)
        {
            // Create a new JObject to store the values
            JObject @object = new JObject();
            // If this is an invalid item, save the type in the property
            if (value is InvalidItem invalid)
            {
                @object["type"] = invalid.Type;
            }
            // Otherwise, save the real Type
            else
            {
                @object["type"] = value.GetType().ToString();
            }
            // Store the total count of this item
            @object["count"] = value.Count;
            // And save it
            @object.WriteTo(writer);
        }
    }
}
