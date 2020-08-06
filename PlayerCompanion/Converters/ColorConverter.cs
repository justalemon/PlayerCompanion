using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;

namespace PlayerCompanion.Converters
{
    /// <summary>
    /// Serializes and Deserializes <see cref="Color"/>.
    /// </summary>
    public class ColorConverter : JsonConverter<Color>
    {
        /// <summary>
        /// Reads a JSON Object as a <see cref="Color"/>.
        /// </summary>
        /// <returns>The <see cref="Color"/> created from the JSON Object.</returns>
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject @object = JObject.Load(reader);
            return Color.FromArgb((int)@object["a"], (int)@object["r"], (int)@object["g"], (int)@object["b"]);
        }
        /// <summary>
        /// Writes the <see cref="Color"/> as a JSON Object.
        /// </summary>
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            JObject @object = new JObject
            {
                ["r"] = value.R,
                ["g"] = value.G,
                ["b"] = value.B,
                ["a"] = value.A
            };
            @object.WriteTo(writer);
        }
    }
}
