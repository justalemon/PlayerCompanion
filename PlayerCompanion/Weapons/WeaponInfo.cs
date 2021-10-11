using GTA;
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

            Weapon weapon = Game.Player.Character.Weapons[WeaponHash];

            foreach (WeaponComponent component in weapon.Components)
            {
                if (component.Active)
                {
                    Components.Add(component.ComponentHash);
                }
            }

            Ammo = weapon.Ammo;
            Tint = (int)weapon.Tint;
        }
        /// <summary>
        /// Applies this weapon information.
        /// </summary>
        public void Apply()
        {
            WeaponCollection collection = Game.Player.Character.Weapons;

            collection.Remove(WeaponHash);

            Weapon weapon = collection.HasWeapon(WeaponHash) ? collection[WeaponHash] : collection.Give(WeaponHash, 0, false, true);

            weapon.Ammo = Ammo;
            weapon.Tint = (WeaponTint)Tint;

            foreach (WeaponComponentHash component in Components)
            {
                weapon.Components[component].Active = true;
            }
        }

        #endregion
    }
}
