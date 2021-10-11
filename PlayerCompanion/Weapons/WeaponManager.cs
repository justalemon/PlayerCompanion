using GTA;
using GTA.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Manages the weapons owned by the player.
    /// </summary>
    public class WeaponManager
    {
        #region Fields

        private readonly Dictionary<Model, WeaponSet> weapons = new Dictionary<Model, WeaponSet>();

        #endregion

        #region Properties

        /// <summary>
        /// The Weapon Set of the Current Player Ped.
        /// </summary>
        public WeaponSet Current => this[Game.Player.Character.Model];
        /// <summary>
        /// Gets the Weapon Inventory of a specific ped.
        /// </summary>
        /// <param name="model">The model of the Ped.</param>
        /// <returns>The Inventory of the Ped.</returns>
        public WeaponSet this[Model model] => LoadOrCreate(model);

        #endregion

        #region Constructors

        internal WeaponManager()
        {
        }

        #endregion

        #region Function

        /// <summary>
        /// Loads or Creates the Weapon Set for the specified ped.
        /// If the ped Weapon Set is already loaded, the existing one is returned.
        /// </summary>
        /// <param name="model">The Ped Model.</param>
        private WeaponSet LoadOrCreate(Model model)
        {
            if (weapons.ContainsKey(model))
            {
                return weapons[model];
            }

            string file = Path.Combine(Companion.location, "Weapons", $"{model.Hash}.json");

            if (!File.Exists(file))
            {
                WeaponSet newSet = new WeaponSet();
                weapons[model] = newSet;
                return newSet;
            }

            try
            {
                string contents = File.ReadAllText(file);
                WeaponSet loadedSet = JsonConvert.DeserializeObject<WeaponSet>(contents);
                weapons[model] = loadedSet;
                return loadedSet;
            }
            catch (JsonSerializationException e)
            {
                Notification.Show($"~r~Error~s~: Unable to load {model.Hash}.json: {e.Message}");
                return null;
            }
        }

        #endregion
    }
}
