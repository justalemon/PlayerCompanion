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

        #region Events

        /// <summary>
        /// Event triggered when an Item is Added to this Inventory.
        /// </summary>
        public event ItemChangedEventHandler ItemAdded;
        /// <summary>
        /// Event triggered when an Item is Removed from this Inventory.
        /// </summary>
        public event ItemChangedEventHandler ItemRemoved;

        #endregion

        #region Constructor

        internal PedInventory(Model model, InventoryManager manager, List<Item> items)
        {
            Owner = model;
            this.manager = manager;
            if (items != null)
            {
                foreach (Item item in items)
                {
                    item.inventory = this;
                    this.items.Add(item);
                }
            }
        }

        #endregion

        #region Functions

        /// <summary>
        /// Adds an item to this inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(Item item)
        {
            // If the item is already present, raise an exception
            if (item.inventory == this && items.Contains(item))
            {
                throw new InvalidOperationException("The Item is already part of the Inventory.");
            }
            // If the inventory is part of another inventory, raise an exception
            if (item.inventory != null)
            {
                throw new InvalidOperationException("The Item is part of another Inventory.");
            }

            // Otherwise, add it and trigger the events
            items.Add(item);
            ItemChangedEventArgs e = new ItemChangedEventArgs(item);
            ItemAdded?.Invoke(this, e);
            manager.OnItemAdded(this, e);
        }
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void Remove(Item item)
        {
            // If the item is not part of the menu, just return
            if (items.Contains(item))
            {
                return;
            }
            // Otherwise, remove it and trigger the events
            items.Remove(item);
            ItemChangedEventArgs e = new ItemChangedEventArgs(item);
            ItemRemoved?.Invoke(this, e);
            manager.OnItemRemoved(this, e);
        }
        /// <summary>
        /// Checks if the item is part of this Inventory.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="true"/> if the item is on the inventory, <see langword="false"/> otherwise.</returns>
        public bool Contains(Item item) => items.Contains(item);

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
