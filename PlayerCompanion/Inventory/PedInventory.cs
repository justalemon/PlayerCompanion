using GTA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        
        #region Tools

        private Item FindSpecific(Type type, bool returnNull)
        {
            foreach (Item item in items)
            {
                if (item.GetType() == type)
                {
                    return item;
                }
            }

            if (returnNull)
            {
                return null;
            }

            Item created = (Item)Activator.CreateInstance(type);
            Add(created);
            return created;
        }
        
        #endregion

        #region Functions

        /// <summary>
        /// Saves the inventory.
        /// </summary>
        public void Save()
        {
            string dir = Path.Combine(Companion.location, "Inventory");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, $"{Owner.Hash}.json");

            string contents = JsonConvert.SerializeObject(items.Where(x => !(x is StackableItem) || (x is StackableItem stackableItem && stackableItem.Count > 0)));
            File.WriteAllText(path, contents);
        }
        /// <summary>
        /// Tries to find an item with the matching type. If is not found, it will add a new one to the inventory.
        /// </summary>
        /// <typeparam name="T">The type of item to find.</typeparam>
        /// <returns>The item found, or a new one that was added to the inventory with the same type.</returns>
        /// <remarks>
        /// The item needs to have a parameterless constructor, otherwise an exception might be raised.
        /// </remarks>
        /// <exception cref="MissingMethodException">The item does not has a parameterless constructor.</exception>
        public T FindOrCreate<T>() where T: Item => (T)FindSpecific(typeof(T), false);
        /// <summary>
        /// Tries to find an item with the matching type. If is not found, it will add a new one to the inventory.
        /// </summary>
        /// <param name="type">The type of item to find.</param>
        /// <returns>The item found, or a new one that was added to the inventory with the same type.</returns>
        /// <remarks>
        /// The item needs to have a parameterless constructor, otherwise an exception might be raised.
        /// </remarks>
        /// <exception cref="MissingMethodException">The item does not has a parameterless constructor.</exception>
        public Item FindOrCreate(Type type) => FindSpecific(type, false);
        /// <summary>
        /// Tries to find an item with the matching type.
        /// </summary>
        /// <typeparam name="T">The type of item to find.</typeparam>
        /// <returns>The item found, or <see langword="null"/> if none were found.</returns>
        public T FindSingle<T>() where T: Item => (T)FindSpecific(typeof(T), true);
        /// <summary>
        /// Tries to find an item with the matching type.
        /// </summary>
        /// <param name="type">The type of item to find.</param>
        /// <returns>The item found, or <see langword="null"/> if none were found.</returns>
        public Item FindSingle(Type type) => FindSpecific(type, true);
        /// <summary>
        /// Finds all of the items that match a specific type.
        /// </summary>
        /// <typeparam name="T">The type of item to find.</typeparam>
        /// <returns>An iterator returning all of the items found.</returns> 
        public IEnumerator<T> FindMany<T>() where T : Item
        {
            Type type = typeof(T);
            
            foreach (Item item in items)
            {
                if (item.GetType() == type)
                {
                    yield return (T)item;
                }
            }
        }
        /// <summary>
        /// Finds all of the items that match a specific type.
        /// </summary>
        /// <typeparam name="T">The type of item to find.</typeparam>
        /// <returns>An iterator returning all of the items found.</returns> 
        public IEnumerator<Item> FindMany(Type type)
        {
            foreach (Item item in items)
            {
                if (item.GetType() == type)
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// Finds an item with the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the Item.</typeparam>
        /// <returns>The item that was found, null otherwise.</returns>
        [Obsolete("Find<T>() and Find(Type) are Obsolete, please use FindOrCreate<T>(), FindSingle<T>() or FindMany<T>() instead", true)]
        public Item Find<T>() => FindSpecific(typeof(T), true);
        /// <summary>
        /// Finds an item with the specified type.
        /// </summary>
        /// <param name="type">The type of the Item.</param>
        /// <returns>The item that was found, null otherwise.</returns>
        [Obsolete("Find<T>() and Find(Type) are Obsolete, please use FindOrCreate<T>(), FindSingle<T>() or FindMany<T>() instead", true)]
        public Item Find(Type type) => FindSpecific(type, true);
        /// <summary>
        /// Adds an item to this inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(Item item)
        {
            if (item.inventory == this && items.Contains(item))
            {
                throw new InvalidOperationException("The Item is already part of the Inventory.");
            }
            if (item.inventory != null)
            {
                throw new InvalidOperationException("The Item is part of another Inventory.");
            }

            bool added = false;

            if (item is StackableItem stackable)
            {
                if (FindSpecific(item.GetType(), true) is StackableItem found)
                {
                    found.Count += stackable.Count;
                }
                else
                {
                    items.Add(item);
                    
                    stackable.CountChanged -= CountChanged;
                    stackable.CountChanged += CountChanged;
                    
                    added = true;
                }
            }
            else
            {
                items.Add(item);
                added = true;
            }

            if (added)
            {
                ItemChangedEventArgs e = new ItemChangedEventArgs(item);
                ItemAdded?.Invoke(this, e);
                manager.OnItemAdded(this, e);
            }
            
            Save();
        }
        /// <summary>
        /// Saves if the count has been changed.
        /// </summary>
        private void CountChanged(object sender, EventArgs e)
        {
            Companion.menu.UpdateItems();
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
