using GTA;
using GTA.Native;
using GTA.UI;
using Newtonsoft.Json;
using System;
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

        internal static Dictionary<WeaponHash, List<WeaponComponentHash>> sets = new Dictionary<WeaponHash, List<WeaponComponentHash>>();
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
            // Add the list of weapons and their components
            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                if (hash == WeaponHash.Unarmed || hash == WeaponHash.Parachute)
                {
                    continue;
                }

                List<WeaponComponentHash> components = new List<WeaponComponentHash>();

                foreach (WeaponComponentHash component in Enum.GetValues(typeof(WeaponComponentHash)))
                {
                    if (Function.Call<bool>(Hash.DOES_WEAPON_TAKE_WEAPON_COMPONENT, hash, component))
                    {
                        components.Add(component);
                    }
                }

                sets.Add(hash, components);
            }
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
            // If there is already a weapon set for the ped, return it
            if (weapons.ContainsKey(model))
            {
                return weapons[model];
            }
            // Otherwise, try to load it from a file
            string file = Path.Combine(Companion.location, "Weapons", $"{model.Hash}.json");
            // If it does not exists, create a new inventory and save it
            if (!File.Exists(file))
            {
                WeaponSet newSet = new WeaponSet();
                weapons[model] = newSet;
                return newSet;
            }
            // If it does, try to load it
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
