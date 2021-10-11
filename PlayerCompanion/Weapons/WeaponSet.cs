using GTA;
using GTA.Native;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents the weapons owned by a player.
    /// </summary>
    public class WeaponSet
    {
        #region Properties

        /// <summary>
        /// The weapons that are part of this Set.
        /// </summary>
        [JsonProperty("weapons")]
        public List<WeaponInfo> Weapons = new List<WeaponInfo>();

        #endregion

        #region Functions

        /// <summary>
        /// Updates the values of the Set from the Current Player.
        /// </summary>
        public void Populate()
        {
            Weapons.Clear();

            foreach (WeaponHash weaponHash in Weapon.GetAllWeaponHashesForHumanPeds())
            {
                if (weaponHash == WeaponHash.Unarmed || weaponHash == WeaponHash.Parachute)
                {
                    continue;
                }

                Weapon weapon = Game.Player.Character.Weapons[weaponHash];

                if (!weapon.IsPresent)
                {
                    continue;
                }

                WeaponInfo info = new WeaponInfo(weaponHash);
                Weapons.Add(info);
            }
        }
        /// <summary>
        /// Saves the current Weapon Set.
        /// </summary>
        /// <param name="model">The Model to save as.</param>
        public void Save(Model model)
        {
            string file = Path.Combine(Companion.location, "Weapons", $"{model.Hash}.json");
            Directory.CreateDirectory(Path.Combine(Companion.location, "Weapons"));
            File.WriteAllText(file, JsonConvert.SerializeObject(this));
        }
        /// <summary>
        /// Applies this Weapon Set to the Player.
        /// </summary>
        public void Apply()
        {
            foreach (WeaponInfo weapon in Weapons)
            {
                weapon.Apply();
            }
        }

        #endregion
    }
}
