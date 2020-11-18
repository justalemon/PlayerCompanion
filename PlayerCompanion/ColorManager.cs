using GTA;
using GTA.Native;
using GTA.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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

    /// <summary>
    /// Manages the Colors on the HUD and Radar/Map for the Players.
    /// </summary>
    public class ColorManager
    {
        #region Fields

        private readonly Color colorM = Color.FromArgb(255, 101, 180, 212);
        private readonly Color colorF = Color.FromArgb(255, 171, 237, 171);
        private readonly Color colorT = Color.FromArgb(255, 255, 163, 87);
        private readonly Dictionary<int, Color> colors = new Dictionary<int, Color>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or Sets the color based on the current Ped Model.
        /// </summary>
        public Color Current
        {
            get => this[Game.Player.Character.Model];
            set => this[Game.Player.Character.Model] = value;
        }
        /// <summary>
        /// Gets or Sets the color for the specified Ped.
        /// </summary>
        /// <param name="model">The Model of the ped.</param>
        /// <returns>The Color used for the specific ped.</returns>
        public Color this[Model model]
        {
            get
            {
                if (colors.ContainsKey(model))
                {
                    return colors[model];
                }
                else if (model == PedHash.Michael)
                {
                    return colorM;
                }
                else if (model == PedHash.Franklin)
                {
                    return colorF;
                }
                else if (model == PedHash.Trevor)
                {
                    return colorT;
                }
                else
                {
                    return default;
                }
            }
            set
            {
                colors[model] = value;
                Save();
                if (Game.Player.Character.Model == model)
                {
                    Apply(value);
                }
            }
        }

        #endregion

        #region Constructor

        internal ColorManager()
        {
            string file = Path.Combine(Companion.location, "Colors.json");
            // If the file with colors exists, load it
            if (File.Exists(file))
            {
                string contents = File.ReadAllText(file);
                try
                {
                    colors = JsonConvert.DeserializeObject<Dictionary<int, Color>>(contents, new ColorConverter());
                }
                catch (JsonSerializationException e)
                {
                    Notification.Show($"~r~Error~s~: Unable to load Colors: {e.Message}");
                }
            }
            // Otherwise, write an empty dictionary
            else
            {
                Directory.CreateDirectory(Companion.location);
                File.WriteAllText(file, "{}");
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Saves the current money used by the peds.
        /// </summary>
        internal void Save()
        {
            Directory.CreateDirectory(Companion.location);
            File.WriteAllText(Path.Combine(Companion.location, "Colors.json"), JsonConvert.SerializeObject(colors));
        }
        /// <summary>
        /// Applies a color in the HUD:
        /// </summary>
        /// <param name="color">The color to apply.</param>
        internal void Apply(Color color)
        {
            // Light
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
            // Dark
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 153, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 154, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 155, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
        }
        /// <summary>
        /// Restores the colors to their default values.
        /// </summary>
        internal void RestoreDefault()
        {
            // Light
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, 101, 180, 212, 255); // M
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, 171, 237, 171, 255); // F
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, 255, 163, 87, 255); // T
            // Dark
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 153, 72, 103, 116, 255); // M
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 154, 85, 118, 85, 255); // F
            Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 155, 127, 81, 43, 255); // T
        }
        /// <summary>
        /// Checks if the Ped Model has a custom color set.
        /// </summary>
        /// <param name="model">The Ped Model to check.</param>
        public bool HasCustomColor(Model model) => colors.ContainsKey(model);

        #endregion
    }
}
