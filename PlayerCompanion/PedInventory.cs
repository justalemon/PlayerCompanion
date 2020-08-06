using GTA;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents an Inventory for a specific <see cref="Ped"/> <see cref="GTA.Model"/>.
    /// </summary>
    public class PedInventory
    {
        #region Private Fields

        private static readonly string locationMod = new Uri(Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase)).LocalPath;
        private static readonly string locationInventoryData = Path.Combine(locationMod, "PlayerCompanion", "Inventory");

        private readonly List<Item> items = new List<Item>();

        #endregion

        #region Public Properties

        /// <summary>
        /// The <see cref="Ped"/> <see cref="GTA.Model"/> assigned to this inventory.
        /// </summary>
        public Model Model { get; }

        #endregion

        #region Constructors

        internal PedInventory(Model model, params Item[] items)
        {
            Model = model;
            this.items.AddRange(items);
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Adds a specific item onto this inventory.
        /// </summary>
        /// <param name="item">The item added.</param>
        public void Add(Item item)
        {
            // If the item is already on the list, return
            if (items.Contains(item))
            {
                throw new InvalidOperationException("This item is already part of this inventory.");
            }

            // Try to find an item with the same type
            foreach (Item existingItem in items)
            {
                // If the types match, raise an exception
                if (existingItem.GetType() == item.GetType())
                {
                    throw new InvalidOperationException("This inventory already has an item with the same type.");
                }
            }

            // Otherwise, add it and save the item
            items.Add(item);
            Save();
        }
        /// <summary>
        /// Saves this <see cref="PedInventory"/>.
        /// </summary>
        public void Save()
        {
            Directory.CreateDirectory(locationInventoryData);
            string json = JsonConvert.SerializeObject(items);
            File.WriteAllText(Path.Combine(locationInventoryData, $"{Model.Hash}.json"), json);
        }

        #endregion
    }
}
