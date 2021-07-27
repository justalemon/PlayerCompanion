using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;

namespace PlayerCompanion
{
    /// <summary>
    /// Serializes and Deserializes <see cref="Color"/>.
    /// </summary>
    internal class ColorConverter : JsonConverter<Color>
    {
        /// <summary>
        /// Reads a JSON Object as a <see cref="Color"/>.
        /// </summary>
        /// <returns>The <see cref="Color"/> created from the JSON Object.</returns>
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject @object = JObject.Load(reader);
            int red = @object.ContainsKey("r") ? (int)@object["r"] : 0;
            int green = @object.ContainsKey("g") ? (int)@object["g"] : 0;
            int blue = @object.ContainsKey("b") ? (int)@object["b"] : 0;
            int alpha = @object.ContainsKey("a") ? (int)@object["a"] : 255;
            return Color.FromArgb(alpha, red, green, blue);
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
