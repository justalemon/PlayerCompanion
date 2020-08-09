using GTA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Weapon Manager and Stash.
    /// </summary>
    public class Weapons : Script
    {
        #region Private Fields

        private static Model last = 0;
        private static readonly Dictionary<Model, WeaponSet> weapons = new Dictionary<Model, WeaponSet>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Weapon Stash Script.
        /// </summary>
        public Weapons()
        {
            // Finally, add the events
            Tick += Weapons_Tick;
        }

        #endregion

        #region Private Functions

        private void LoadPedWeapons()
        {
            // Get the ped model
            Model model = Game.Player.Character.Model;
            // Format a path for the file
            string file = Path.Combine(Locations.WeaponData, $"{model.Hash}.json");

            // If there is already an entry for the ped, apply it
            if (weapons.ContainsKey(model))
            {
                weapons[model].Apply();
            }
            // If there is a file for the ped, load it
            else if (File.Exists(file))
            {
                string contents = File.ReadAllText(file);
                WeaponSet set = JsonConvert.DeserializeObject<WeaponSet>(contents);
                set.Owner = model;
                weapons[model] = set;
                set.Apply();
            }
            // Otherwise, create a new set and save it
            else
            {
                WeaponSet set = WeaponSet.FromPlayer();
                set.Owner = model;
                weapons[model] = set;
                string contents = JsonConvert.SerializeObject(set);
                Directory.CreateDirectory(Locations.WeaponData);
                File.WriteAllText(file, contents);
            }
        }

        #endregion

        #region Local Events

        private void Weapons_Tick(object sender, EventArgs e)
        {
            // Get the ped model
            Model model = Game.Player.Character.Model;

            // If is not the same model, load the weapons and save the hash
            if (last != model)
            {
                LoadPedWeapons();
                last = model;
            }
        }

        #endregion
    }
}
