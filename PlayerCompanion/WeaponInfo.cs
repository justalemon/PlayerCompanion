using GTA;
using GTA.Native;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PlayerCompanion
{
    /// <summary>
    /// The information of a specific vehicle Weapon.
    /// </summary>
    public class WeaponInfo
    {
        /// <summary>
        /// How much ammo does this weapon has.
        /// </summary>
        [JsonProperty("ammo")]
        public int Ammo { get; set; } = 0;
        /// <summary>
        /// The tint of the weapon.
        /// </summary>
        [JsonProperty("tint")]
        public int Tint { get; set; } = 0;
        /// <summary>
        /// The Components or Parts of the weapon.
        /// </summary>
        [JsonProperty("components")]
        public List<WeaponComponentHash> Components { get; set; } = new List<WeaponComponentHash>();

        /// <summary>
        /// Creates a new Weapon Information from the player.
        /// </summary>
        /// <param name="hash">The hash of the weapon to get.</param>
        /// <returns>A new Weapon Information.</returns>
        public static WeaponInfo FromPlayer(WeaponHash hash)
        {
            // Create a new Weapon Info object
            WeaponInfo info = new WeaponInfo();
            // And populate it
            info.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Game.Player.Character, hash);
            info.Tint = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Game.Player.Character, hash);
            foreach (WeaponComponentHash component in Enum.GetValues(typeof(WeaponComponentHash)))
            {
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, hash, component))
                {
                    info.Components.Add(component);
                }
            }
            // Finally, return it
            return info;
        }
    }
}
