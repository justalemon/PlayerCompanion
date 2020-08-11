using GTA;
using GTA.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Manager for the Money of the peds.
    /// </summary>
    public class MoneyManager
    {
        #region Private Fields

        private static Dictionary<int, int> money = new Dictionary<int, int>();

        #endregion

        #region Public Property

        /// <summary>
        /// If the current Ped is using a Custom money system.
        /// </summary>
        public static bool IsUsingCustomSystem
        {
            get
            {
                Model model = Game.Player.Character.Model;
                return model.Hash == -1692214353 || model.Hash == 225514697 || model.Hash == -1686040670;
            }
        }
        /// <summary>
        /// The Money of the current Player Ped.
        /// </summary>
        public static int PlayerMoney
        {
            get => GetMoneyForModel(Game.Player.Character.Model);
            set => SetMoneyForModel(Game.Player.Character.Model, value);
        }

        #endregion

        #region Constructors

        internal MoneyManager(Companion companion)
        {
            // If the money script exists, load the contents of it
            if (File.Exists(Locations.Money))
            {
                string contents = File.ReadAllText(Locations.Money);
                money = JsonConvert.DeserializeObject<Dictionary<int, int>>(contents);
            }
            // Otherwise, create an empty file
            else
            {
                File.WriteAllText(Locations.Money, "{}");
            }
            // Add a tick event that shows the money
            companion.Tick += Money_Tick;
        }

        #endregion

        #region Local Event

        private void Money_Tick(object sender, EventArgs e)
        {
            Function.Call(Hash.SHOW_HUD_COMPONENT_THIS_FRAME, 3);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Saves the Money information.
        /// </summary>
        public static void Save()
        {
            string contents = JsonConvert.SerializeObject(money);
            File.WriteAllText(Locations.Money, contents);
        }
        /// <summary>
        /// Gets the current money for the specified Ped model.
        /// </summary>
        /// <param name="model">The model to get the information.</param>
        /// <returns>The current money for the specified Ped.</returns>
        public static int GetMoneyForModel(Model model)
        {
            // If the model is not a ped, raise an exception
            if (!model.IsPed)
            {
                throw new ArgumentException(nameof(model), "The Model is not a ped.");
            }
            // If the player is F, M or T, return the value of the stats
            if (IsUsingCustomSystem)
            {
                return Game.Player.Money;
            }
            // Otherwise, return the value in the Dictionary above (if present, zero otherwise)
            return money.ContainsKey(model.Hash) ? money[model.Hash] : 0;
        }
        /// <summary>
        /// Sets the money of a specific Ped model.
        /// </summary>
        /// <param name="model">The model to set.</param>
        /// <param name="value">The money value to set.</param>
        public static void SetMoneyForModel(Model model, int value)
        {
            // If the money is under zero, raise an exception
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Money can't be set under zero.");
            }
            // If the player is F, M or T, save the value on the stats
            if (IsUsingCustomSystem)
            {
                Game.Player.Money = value;
            }
            // Otherwise, save the value in the dictionary
            money[model.Hash] = value;
            Save();
        }

        #endregion
    }
}
