using GTA;
using GTA.Native;
using Newtonsoft.Json;

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
        public int Ammo { get; set; }

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
            // Finally, return it
            return info;
        }
    }
}
