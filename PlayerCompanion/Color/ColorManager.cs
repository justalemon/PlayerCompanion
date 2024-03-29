﻿using GTA;
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
    /// Manages the Colors on the HUD and Radar/Map for the Players.
    /// </summary>
    public class ColorManager
    {
        #region Fields

        private static readonly ColorConverter colorConverter = new ColorConverter();
        private readonly Color colorM = Color.FromArgb(255, 101, 180, 212);
        private readonly Color colorF = Color.FromArgb(255, 171, 237, 171);
        private readonly Color colorT = Color.FromArgb(255, 255, 163, 87);
        private readonly Dictionary<Model, Color> colors = new Dictionary<Model, Color>();

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
                    Dictionary<string, Color> values = JsonConvert.DeserializeObject<Dictionary<string, Color>>(contents, colorConverter);
                    foreach (KeyValuePair<string, Color> pair in values)
                    {
                        Model model;
                        if (int.TryParse(pair.Key, out int number))
                        {
                            model = new Model(number);
                        }
                        else
                        {
                            model = new Model(pair.Key);
                        }
                        colors[model] = pair.Value;
                    }
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
            File.WriteAllText(Path.Combine(Companion.location, "Colors.json"), JsonConvert.SerializeObject(colors, colorConverter));
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
        /// Applies a color in the HUD temporarily:
        /// </summary>
        /// <param name="color">The color to apply.</param>
        public void Apply(Color color)
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
        /// Checks if the Ped Model has a custom color set.
        /// </summary>
        /// <param name="model">The Ped Model to check.</param>
        public bool HasCustomColor(Model model) => colors.ContainsKey(model);

        #endregion
    }
}
