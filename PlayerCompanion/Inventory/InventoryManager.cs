using GTA;
using GTA.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Manages the inventory system for the Players.
    /// </summary>
    public class InventoryManager
    {
        #region Fields

        private static readonly Random random = new Random();
        private static readonly List<Type> items = new List<Type>();
        private static readonly Dictionary<Model, PedInventory> inventories = new Dictionary<Model, PedInventory>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the inventory of the current Player Ped.
        /// </summary>
        public PedInventory Current => this[Game.Player.Character.Model];
        /// <summary>
        /// Gets the inventory of a specific Ped Model.
        /// </summary>
        /// <param name="model">The Ped Model to use.</param>
        /// <returns>The Inventory of the Ped.</returns>
        /// <remarks>
        /// If the inventory has not yet been loaded, it will be opened and deserialized.
        /// </remarks>
        public PedInventory this[Model model]
        {
            get
            {
                // If the companion has not loaded entirely, return null
                if (!Companion.IsReady)
                {
                    return null;
                }

                // Send the existing inventory if present
                if (inventories.ContainsKey(model))
                {
                    return inventories[model];
                }
                // Otherwise, try to load the inventory from storage
                PedInventory loaded = Load(model);
                if (loaded != null)
                {
                    return loaded;
                }
                // If there is no inventory, create a new one
                PedInventory @new = new PedInventory(model, this, null);
                inventories[model] = @new;
                return @new;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when an Item is Added to any Inventory.
        /// </summary>
        public event ItemChangedEventHandler ItemAdded;
        internal void OnItemAdded(PedInventory inventory, ItemChangedEventArgs e) => ItemAdded?.Invoke(inventory, e);
        /// <summary>
        /// Event triggered when an Item is Removed from any Inventory.
        /// </summary>
        public event ItemChangedEventHandler ItemRemoved;
        internal void OnItemRemoved(PedInventory inventory, ItemChangedEventArgs e) => ItemRemoved?.Invoke(inventory, e);

        #endregion

        #region Constructor

        internal InventoryManager()
        {
        }

        #endregion

        #region Functions

        /// <summary>
        /// Generates a new Random Item from memory.
        /// </summary>
        /// <returns>A new item.</returns>
        public Item GetRandomItem()
        {
            // If there are no items, return null
            if (items.Count <= 0)
            {
                return null;
            }

            // Get a random item
            Item item = null;
            while (item == null)
            {
                Type type = items[random.Next(items.Count)];
                try
                {
                    item = (Item)Activator.CreateInstance(type);
                }
                catch (MissingMethodException)
                {
                    Notification.Show($"~o~Warning~s~: {type.Name} does not has a parameterless constructor!");
                    item = null;
                }
            }
            return item;
        }
        /// <summary>
        /// Populates a list of random items for the user to use.
        /// </summary>
        internal void PopulateItems()
        {
            // Clear the list of types
            items.Clear();

            // Iterate over the types in all of the assemblies and add those that inherit from Item to the list
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Skip dynamic assemblies
                // GetExportedTypes() do not work
                if (assembly.IsDynamic)
                {
                    continue;
                }

                foreach (Type type in assembly.GetExportedTypes())
                {
                    if (typeof(Item).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        items.Add(type);
                    }
                }
            }
        }
        /// <summary>
        /// Loads the inventory of the specified Ped Model.
        /// </summary>
        internal PedInventory Load(Model model)
        {
            // If there is already an inventory, return that inventory
            if (inventories.ContainsKey(model))
            {
                return inventories[model];
            }

            string file = Path.Combine(Companion.location, "Inventory", $"{model.Hash}.json");

            // If the player does not has an inventory saved, create a new one
            if (!File.Exists(file))
            {
                return null;
            }

            // Otherwise, load it's contents
            string contents = File.ReadAllText(file);
            // And try to deserialize them
            try
            {
                List<Item> items = JsonConvert.DeserializeObject<List<Item>>(contents);
                PedInventory inventory = new PedInventory(model, this, items);
                inventories[model] = inventory;
                return inventory;
            }
            catch (JsonSerializationException e)
            {
                Notification.Show($"~r~Error~s~: Unable to load the Inventory of {model.Hash}: {e.Message}");
                return null;
            }
        }

        #endregion
    }
}
