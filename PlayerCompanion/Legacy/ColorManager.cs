using GTA;
using GTA.Native;
using GTA.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ColorConverter = PlayerCompanion.Converters.ColorConverter;

namespace PlayerCompanion
{
    /// <summary>
    /// Script that deals with the HUD Color Changes.
    /// </summary>
    internal class ColorManager
    {
        #region Private Fields

        private static readonly Model modelFranklin = "player_one";
        private static readonly Model modelTrevor = "player_two";
        private static readonly Model modelMichael = "player_zero";
        private static Model last = null;
        private static Dictionary<int, Color> colors = new Dictionary<int, Color>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new class that changes the HUD Colors.
        /// </summary>
        internal ColorManager(Companion companion)
        {
            // If the file with colors exists, load it
            if (File.Exists(Locations.Colors))
            {
                string contents = File.ReadAllText(Locations.Colors);
                colors = JsonConvert.DeserializeObject<Dictionary<int, Color>>(contents, new ColorConverter());
            }
            // Otherwise, write an empty dictionary
            else
            {
                File.WriteAllText(Locations.Colors, "{}");
            }

            // And add the Tick and Aborted events
            //companion.Tick += HUDColors_Tick;
            //companion.Aborted += HUDColors_Aborted;
        }

        #endregion

        #region Private Functions

        private void RestoreColors()
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

        #endregion

        #region Local Events

        private void HUDColors_Tick(object sender, EventArgs e)
        {
            // Save the model
            Model model = Game.Player.Character.Model;

            // If the last ped model is different, change the color
            if (last != model)
            {
                // If is Michael, Franklin or Trevor, restore it to default
                if (model == modelFranklin || model == modelTrevor || model == modelMichael)
                {
                    RestoreColors();
                }
                // Otherwise, set the colors to the one loaded (if any)
                else if (colors.ContainsKey(model))
                {
                    Color color = colors[model];
                    // Light
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
                    // Dark
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 153, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 154, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 155, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
                }
                // If we got here, set a gray color and notify the player
                else
                {
                    Color color = Color.DarkGray;
                    // Light
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 143, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 144, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 145, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
                    // Dark
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 153, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // M
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 154, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // F
                    Function.Call(Hash.REPLACE_HUD_COLOUR_WITH_RGBA, 155, (int)color.R, (int)color.G, (int)color.B, (int)color.A); // T
                    Notification.Show($"{model.Hash} does not has a color set!");
                }

                // Finally, save the model
                last = model;
            }
        }

        private void HUDColors_Aborted(object sender, EventArgs e)
        {
            // Restore the vanilla colors when terminating the script
            RestoreColors();
        }

        #endregion
    }
}
