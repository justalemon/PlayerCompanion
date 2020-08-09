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
        public Dictionary<int, WeaponInfo> Weapons { get; set; } = new Dictionary<int, WeaponInfo>();

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
                    set.Weapons[(int)hash] = WeaponInfo.FromPlayer(hash);
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
                if (!Weapons.ContainsKey((int)hash))
                {
                    continue;
                }

                // Get the weapon information and current weapon
                WeaponInfo info = Weapons[(int)hash];
                OutputArgument argument = new OutputArgument();
                Function.Call(Hash.GET_CURRENT_PED_WEAPON, Game.Player.Character, argument, true);
                WeaponHash previous = argument.GetResult<WeaponHash>();

                // If the player does not has the weapon, give it to him with the correct ammo
                if (!Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, hash, false))
                {
                    Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, hash, info.Ammo, false, false);
                }
                // Then apply the components
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
            string path = Path.Combine(Locations.WeaponData, $"{Owner}.json");
            string contents = JsonConvert.SerializeObject(this);
            File.WriteAllText(path, contents);
        }

        #endregion
    }
}
