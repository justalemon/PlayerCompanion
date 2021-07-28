using GTA;
using GTA.Native;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents the information of a specific weapon.
    /// </summary>
    public class WeaponInfo
    {
        #region Properties

        /// <summary>
        /// The Hash of this weapon.
        /// </summary>
        [JsonProperty("hash")]
        public WeaponHash WeaponHash { get; set; }
        /// <summary>
        /// The current ammo of the Weapon.
        /// </summary>
        [JsonProperty("ammo")]
        public int Ammo { get; set; }
        /// <summary>
        /// The current Tint of the weapon.
        /// </summary>
        [JsonProperty("tint")]
        public int Tint { get; set; }
        /// <summary>
        /// The components of the weapon.
        /// </summary>
        [JsonProperty("components")]
        public List<WeaponComponentHash> Components { get; set; } = new List<WeaponComponentHash>();

        #endregion

        #region Constructors

        /// <summary>
        /// Creates empty Weapon Information.
        /// </summary>
        public WeaponInfo()
        {
        }
        /// <summary>
        /// Creates a new Weaapon Info with the specified hash.
        /// This will automatically populate the properties.
        /// </summary>
        /// <param name="hash">The Hash of the weapon.</param>
        public WeaponInfo(WeaponHash hash)
        {
            WeaponHash = hash;
            Update();
        }

        #endregion

        #region Functions

        /// <summary>
        /// Updates the Weapon Information based on the Hash.
        /// </summary>
        public void Update()
        {
            Components.Clear();
            foreach (WeaponComponentHash component in WeaponManager.sets[WeaponHash])
            {
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON_COMPONENT, Game.Player.Character, WeaponHash, component))
                {
                    Components.Add(component);
                }
            }
            Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Game.Player.Character, WeaponHash);
            Tint = Function.Call<int>(Hash.GET_PED_WEAPON_TINT_INDEX, Game.Player.Character, WeaponHash);
        }
        /// <summary>
        /// Applies this weapon information.
        /// </summary>
        public void Apply()
        {
            if (!Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, WeaponHash, false))
            {
                Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, WeaponHash, 0, false, false);
            }

            foreach (WeaponComponentHash component in Components)
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character, WeaponHash, component);
            }
            Function.Call(Hash.SET_PED_AMMO, Game.Player.Character, WeaponHash, Ammo);
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character, WeaponHash, Tint);
        }

        #endregion
    }
}
