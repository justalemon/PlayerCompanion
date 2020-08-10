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
            // Make a property to check if we need to save the set
            bool needsToBeSaved = false;

            // If is not the same model, load the weapons and save the hash
            if (last != model)
            {
                LoadPedWeapons();
                last = model;
            }

            // Get the current player Weapon Set
            WeaponSet set = weapons[model];

            // Iterate over the weapons
            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                // If this is unarmed or parachute, skip it
                if (hash == WeaponHash.Unarmed || hash == WeaponHash.Parachute)
                {
                    continue;
                }

                // Check if the player has this weapon
                bool playerHas = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, hash, false);

                // If the player does not has the weapon but is in the inventory, delete it and continue
                if (!playerHas && set.Weapons.ContainsKey(hash))
                {
                    set.Weapons.Remove(hash);
                    needsToBeSaved = true;
                    continue;
                }
                // If the player has the weapon but is not in the inventory, add it and continue
                else if (playerHas && !set.Weapons.ContainsKey(hash))
                {
                    set.Weapons[hash] = WeaponInfo.FromPlayer(hash);
                    needsToBeSaved = true;
                    continue;
                }

                // If there is no weapon, continue
                if (!set.Weapons.ContainsKey(hash))
                {
                    continue;
                }
                // Otherwise, get the weapon info
                WeaponInfo info = set.Weapons[hash];

                // Get the current tint
                int tint = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Game.Player.Character, hash);
                // If is not the same, save it
                if (info.Tint != tint)
                {
                    info.Tint = tint;
                    needsToBeSaved = true;
                }

                // Start checking the weapon components
                foreach (WeaponComponentHash component in Enum.GetValues(typeof(WeaponComponentHash)))
                {
                    // Get the current activation of the component
                    bool activated = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, hash, component);

                    // If is activated but not on the list, add it
                    if (activated && !info.Components.Contains(component))
                    {
                        info.Components.Add(component);
                        needsToBeSaved = true;
                    }
                    // If is not activated but is on the list, remove it
                    else if (!activated && info.Components.Contains(component))
                    {
                        info.Components.Remove(component);
                        needsToBeSaved = true;
                    }
                }
            }

            // If we need to save the weapon set, do it
            if (needsToBeSaved)
            {
                set.Save();
            }
        }

        #endregion
    }
}
