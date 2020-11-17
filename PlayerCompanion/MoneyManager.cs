using GTA;
using GTA.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Manages the money of the player.
    /// </summary>
    public class MoneyManager
    {
        #region Fields

        private readonly Dictionary<int, int> moneyValues = new Dictionary<int, int>();

        #endregion

        #region Properties

        /// <summary>
        /// The money associated to the current Player Ped.
        /// </summary>
        public int Value { get; set; }
        /// <summary>
        /// Gets or sets the money for a specific Ped Model.
        /// </summary>
        /// <param name="model">The Ped Model to check.</param>
        /// <returns>The money of the Ped.</returns>
        public int this[Model model]
        {
            get
            {
                if (model == PedHash.Michael || model == PedHash.Franklin || model == PedHash.Franklin)
                {
                    return Game.Player.Money;
                }
                else
                {
                    return moneyValues.ContainsKey(model) ? moneyValues[model] : 0;
                }
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Money can't be under Zero.");
                }

                if (model == PedHash.Michael || model == PedHash.Franklin || model == PedHash.Franklin)
                {
                    Game.Player.Money = value;
                }
                else
                {
                    moneyValues[model] = value;
                    Save();
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Money Manager.
        /// </summary>
        internal MoneyManager()
        {
            string file = Path.Combine(Companion.location, "Money.json");
            // If there is a money file, try to load it
            if (File.Exists(file))
            {
                try
                {
                    moneyValues = JsonConvert.DeserializeObject<Dictionary<int, int>>(File.ReadAllText(file));
                }
                catch (JsonSerializationException e)
                {
                    Notification.Show($"~r~Error~s~: Unable to load the Money File: {e.Message}");
                }
            }
            // Otherwise, create an empty file
            else
            {
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
            File.WriteAllText(Path.Combine(Companion.location, "Money.json"), JsonConvert.SerializeObject(moneyValues));
        }

        #endregion
    }
}
