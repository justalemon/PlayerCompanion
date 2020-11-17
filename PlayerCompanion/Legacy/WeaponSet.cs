using GTA;
using GTA.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// A Set of Weapons owned by a player.
    /// </summary>
    public class WeaponSet
    {
        #region Public Properties

        /// <summary>
        /// The Ped Model that owns this Weapon Set.
        /// </summary>
        [JsonIgnore]
        public Model Owner { get; set; }
        /// <summary>
        /// The weapons that are part of this set.
        /// </summary>
        [JsonProperty("weapons")]
        public Dictionary<WeaponHash, WeaponInfo> Weapons { get; set; } = new Dictionary<WeaponHash, WeaponInfo>();

        #endregion

        #region Public Functions

        /// <summary>
        /// Creates a new Weapon Set from the player.
        /// </summary>
        /// <returns>The weapon set with the player information.</returns>
        public static WeaponSet FromPlayer()
        {
            // Create a new weapon set
            WeaponSet set = new WeaponSet();
            set.Owner = Game.Player.Character.Model;
            // And populate it with the weapons
            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                // If this is unarmed or parachute, skip it
                if (hash == WeaponHash.Unarmed || hash == WeaponHash.Parachute)
                {
                    continue;
                }
                // Otherwise, add it if is present
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, hash, false))
                {
                    set.Weapons[hash] = WeaponInfo.FromPlayer(hash);
                }
            }
            return set;
        }
        /// <summary>
        /// Gives all of the Weapons and Modifications to the local player.
        /// </summary>
        public void Apply()
        {
            // Iterate over the weapons in the inventory
            foreach (WeaponHash hash in Enum.GetValues(typeof(WeaponHash)))
            {
                // If the weapon is not on the inventory, continue
                if (!Weapons.ContainsKey(hash))
                {
                    continue;
                }

                // Get the weapon information and current weapon
                WeaponInfo info = Weapons[hash];
                OutputArgument argument = new OutputArgument();
                Function.Call(Hash.GET_CURRENT_PED_WEAPON, Game.Player.Character, argument, true);
                WeaponHash previous = argument.GetResult<WeaponHash>();

                // If the player does not has the weapon, give it to him with the correct ammo
                if (!Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, hash, false))
                {
                    Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, hash, info.Ammo, false, false);
                }
                // Then apply the ammo type, tint and components
                Function.Call(Hash.SET_PED_AMMO_BY_TYPE, Game.Player.Character, hash, info.Ammo);
                Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character, hash, info.Tint);
                foreach (WeaponComponentHash component in info.Components)
                {
                    Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character, hash, component);
                }

                // Finally, restore the previous player weapon
                Function.Call(Hash.SET_CURRENT_PED_WEAPON, Game.Player.Character, previous, true);
            }
        }
        /// <summary>
        /// Saves the current weapon set.
        /// </summary>
        public void Save()
        {
            string path = Path.Combine(Locations.WeaponData, $"{Owner.Hash}.json");
            string contents = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, contents);
        }
        /// <summary>
        /// Updates the values on the Weapon Set.
        /// </summary>
        /// <returns><see langword="true"/> if changes were made, <see langword="false"/> otherwise.</returns>
        public bool Update()
        {
            // Have a place to store the check of changes
            bool modified = false;

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
                if (!playerHas && Weapons.ContainsKey(hash))
                {
                    Weapons.Remove(hash);
                    modified = true;
                    continue;
                }
                // If the player has the weapon but is not in the inventory, add it and continue
                else if (playerHas && !Weapons.ContainsKey(hash))
                {
                    Weapons[hash] = WeaponInfo.FromPlayer(hash);
                    modified = true;
                    continue;
                }

                // If there is no weapon, continue
                if (!Weapons.ContainsKey(hash))
                {
                    continue;
                }
                // Otherwise, get the weapon info
                WeaponInfo info = Weapons[hash];

                // Save the total ammo
                int ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Game.Player.Character, hash);
                if (info.Ammo != ammo)
                {
                    info.Ammo = ammo;
                }

                // Get the current ammo type and save it if is not the same
                int ammoType = Function.Call<int>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Game.Player.Character, hash);
                if (info.AmmoType != ammoType)
                {
                    info.AmmoType = ammoType;
                    modified = true;
                }

                // Get the current tint
                int tint = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Game.Player.Character, hash);
                // If is not the same, save it
                if (info.Tint != tint)
                {
                    info.Tint = tint;
                    modified = true;
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
                        modified = true;
                    }
                    // If is not activated but is on the list, remove it
                    else if (!activated && info.Components.Contains(component))
                    {
                        info.Components.Remove(component);
                        modified = true;
                    }
                }
            }

            // Finally, return the modification status
            return modified;
        }

        #endregion
    }
}
