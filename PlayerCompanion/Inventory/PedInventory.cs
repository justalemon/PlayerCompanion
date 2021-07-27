using GTA;
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
        /// <summary>
        /// The items that are part of the inventory.
        /// </summary>
        public List<Item> Items => new List<Item>(items); // So the user can't modify them manually

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
        /// Saves the inventory.
        /// </summary>
        public void Save()
        {
            // Create the path of the items
            string dir = Path.Combine(Companion.location, "Inventory");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, $"{Owner.Hash}.json");
            // And save the contents
            string contents = JsonConvert.SerializeObject(items);
            File.WriteAllText(path, contents);
        }
        /// <summary>
        /// Finds an item with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the Item.</typeparam>
        /// <returns>The item that was found, null otherwise.</returns>
        public Item Find<T>()
        {
            foreach (Item item in items)
            {
                if (item is T)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// Finds an item with the specified type.
        /// </summary>
        /// <param name="type">The type of the Item.</param>
        /// <returns>The item that was found, null otherwise.</returns>
        public Item Find(Type type)
        {
            foreach (Item item in items)
            {
                if (item.GetType() == type)
                {
                    return item;
                }
            }
            return null;
        }
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

            // Track if the item was added instead of updated
            bool added = false;

            // If the item is stackable, try to find an item with the same type
            if (item is StackableItem stackable)
            {
                // Try to find an item and add the count
                StackableItem found = (StackableItem)Find(item.GetType());
                if (found != null)
                {
                    found.Count += stackable.Count;
                }
                else
                {
                    items.Add(item);
                    added = true;
                }
                // And add the event used to save when required
                stackable.CountChanged -= CountChanged;
                stackable.CountChanged += CountChanged;
            }
            // Otherwise, add it as-is
            else
            {
                Items.Add(item);
                added = true;
            }

            // Otherwise, add it and trigger the events
            if (added)
            {
                ItemChangedEventArgs e = new ItemChangedEventArgs(item);
                ItemAdded?.Invoke(this, e);
                manager.OnItemAdded(this, e);
            }
            // Saving just in case
            Save();
        }
        /// <summary>
        /// Saves if the count has been changed.
        /// </summary>
        private void CountChanged(object sender, EventArgs e)
        {
            Save();
        }
        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        internal void Remove(Item item)
        {
            // If the item is not part of the menu, just return
            if (!items.Contains(item))
            {
                return;
            }
            // Otherwise, remove it and trigger the events
            items.Remove(item);
            ItemChangedEventArgs e = new ItemChangedEventArgs(item);
            ItemRemoved?.Invoke(this, e);
            manager.OnItemRemoved(this, e);
            // Saving just in case
            Save();
        }
        /// <summary>
        /// Checks if the item is part of this Inventory.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns><see langword="true"/> if the item is on the inventory, <see langword="false"/> otherwise.</returns>
        public bool Contains(Item item) => items.Contains(item);

        #endregion
    }
}
