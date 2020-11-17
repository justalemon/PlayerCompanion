using GTA;
using GTA.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents the inventory of a specific Ped.
    /// </summary>
    public class PedInventory
    {
        #region Fields

        private readonly InventoryManager manager = null;
        private readonly List<Item> items = new List<Item>();

        #endregion

        #region Properties

        /// <summary>
        /// The Ped Model that owns this inventory.
        /// </summary>
        public Model Owner { get; }

        #endregion

        #region Constructor

        internal PedInventory(Model model, InventoryManager manager, List<Item> items)
        {
            Owner = model;
            this.manager = manager;
            if (items != null)
            {
                this.items.AddRange(items);
            }
        }

        #endregion
    }

    /// <summary>
    /// Manages the inventory system for the Players.
    /// </summary>
    public class InventoryManager
    {
        #region Fields

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

        #region Constructor

        internal InventoryManager()
        {
        }

        #endregion

        #region Functions

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
