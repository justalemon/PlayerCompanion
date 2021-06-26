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
            Function.Call(Hash.REMOVE_WEAPON_FROM_PED, Game.Player.Character, WeaponHash);
            Function.Call(Hash.GIVE_WEAPON_TO_PED, Game.Player.Character, WeaponHash, 0, false, false);

            foreach (WeaponComponentHash component in Components)
            {
                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Game.Player.Character, WeaponHash, component);
            }
            Function.Call(Hash.SET_PED_AMMO, Game.Player.Character, WeaponHash, Ammo);
            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Game.Player.Character, WeaponHash, Tint);
        }

        #endregion
    }

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
            foreach (WeaponHash hash in WeaponManager.sets.Keys)
            {
                if (hash == WeaponHash.Unarmed || hash == WeaponHash.Parachute)
                {
                    continue;
                }
                if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Game.Player.Character, hash, false))
                {
                    WeaponInfo info = new WeaponInfo(hash);
                    Weapons.Add(info);
                }
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
