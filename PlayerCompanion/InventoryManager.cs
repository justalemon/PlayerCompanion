using GTA;
using GTA.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace PlayerCompanion
{
    /// <summary>
    /// Handler for the Inventory of Player Peds.
    /// </summary>
    public class InventoryManager
    {
        #region Private Fields

        private static readonly Dictionary<Model, PedInventory> inventories = new Dictionary<Model, PedInventory>();
        private static readonly List<Assembly> completed = new List<Assembly>();
        private static readonly List<Type> nonUnique = new List<Type>();
        internal static InventoryConfiguration configuration = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// The inventory of the current Player Ped Model.
        /// </summary>
        public PedInventory Current
        {
            get
            {
                // If the inventory is shared across all peds, return the generic inventory
                if (configuration.SharedInventory)
                {
                    return GetInventory(0);
                }
                // Otherwise, return the inventory for the specific ped model
                else
                {
                    return GetInventory(Game.Player.Character.Model);
                }
            }
        }

        #endregion

        #region Constructors

        internal InventoryManager(Companion companion)
        {
            // If the configuration file does exists, load it
            if (File.Exists(Locations.ConfigInventory))
            {
                string contents = File.ReadAllText(Locations.ConfigInventory);
                configuration = JsonConvert.DeserializeObject<InventoryConfiguration>(contents);

            }
            // Otherwise, create a new one and save it
            else
            {
                configuration = new InventoryConfiguration();
                string contents = JsonConvert.SerializeObject(configuration);
                Directory.CreateDirectory(Locations.ModWorkDir);
                File.WriteAllText(Locations.ConfigInventory, contents);
            }
            // Load the inventory from the current player
            LoadInventory(configuration.SharedInventory ? new Model(0) : Game.Player.Character.Model);
            // And add the tick for loading the current inventory
            companion.Tick += InventoryManager_Tick;
        }

        #endregion

        #region Local Events

        private void InventoryManager_Tick(object sender, EventArgs e)
        {
            // If we are using ped-dependant inventories and the current model does not has one, load it
            if (!configuration.SharedInventory && !inventories.ContainsKey(Game.Player.Character.Model))
            {
                LoadInventory(Game.Player.Character.Model);
            }
        }

        #endregion

        #region Private Functions

        private PedInventory LoadInventory(Model model)
        {
            // Make the location of the file for the ped model
            string file = Path.Combine(Locations.InventoryData, $"{model.Hash}.json");

            // If there is none, create a new one and send it
            if (!File.Exists(file))
            {
                PedInventory empty = new PedInventory(model);
                inventories[model] = empty;
                return empty;
            }

            // Load the contents of the file
            string contents = File.ReadAllText(file);
            // Try to parse it
            List<Item> items = JsonConvert.DeserializeObject<List<Item>>(contents);
            // If any of them are null, notify the user
            if (items.RemoveAll(item => item == null) != 0)
            {
                Notification.Show("~r~Danger~s~: One of the items could not be parsed. This might represent a missing mod.");
            }
            // Finally, create a new inventory and save it
            PedInventory inventory = new PedInventory(model, items.ToArray());
            inventories[model] = inventory;
            return inventory;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Initializes the Inventory class for the Calling Assembly.
        /// You need to call this function in your Script Constructor once.
        /// </summary>
        public static void Initialize()
        {
            // Get the assembly that called this function
            Assembly assembly = Assembly.GetCallingAssembly();
            // If this item is already in the completed list, raise an exception
            if (completed.Contains(assembly))
            {
                throw new InvalidOperationException("The Assembly has already completed the Initialization process.");
            }

            // Otherwise, get all of the types in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // If the type is a class and is not unique, add it to the list to be given randomly
                if (type.GetTypeInfo().IsClass && Attribute.GetCustomAttribute(type, typeof(InventoryUniqueAttribute)) == null)
                {
                    nonUnique.Add(type);
                }
            }

            // Finally, mark the class initialization as complete
            completed.Add(assembly);
        }
        /// <summary>
        /// Gets the Inventory of a specific <see cref="Ped"/> <see cref="Model"/>.
        /// </summary>
        /// <param name="model">The Model of the <see cref="Ped"/>.</param>
        /// <returns>The <see cref="PedInventory"/> that contains the items of the <see cref="Ped"/> <see cref="Model"/>.</returns>
        public PedInventory GetInventory(Model model)
        {
            // If the model is not a ped, raise an exception
            if (!model.IsPed)
            {
                throw new InvalidOperationException("Model is not a Ped or is not present in the game files.");
            }

            // If there is an inventory in the dictionary, return it
            if (inventories.ContainsKey(model))
            {
                return inventories[model];
            }
            // Otherwise, load it and return it
            return LoadInventory(model);
        }

        #endregion
    }
}
