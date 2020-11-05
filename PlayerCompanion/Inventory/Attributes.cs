using System;

namespace PlayerCompanion
{
    /// <summary>
    /// Marks a Class as a Unique Inventory Item.
    /// Unique Inventory Items will be ignored by <see cref="PedInventory.CreateRandomItem()"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InventoryUniqueAttribute : Attribute
    {
        /// <summary>
        /// If the Inventory Item is unique or not.
        /// </summary>
        public bool Unique { get; private set; } = false;

        /// <summary>
        /// Marks an item as unique.
        /// </summary>
        public InventoryUniqueAttribute()
        {
            Unique = true;
        }

        /// <summary>
        /// Marks an item as unique or not.
        /// </summary>
        /// <param name="unique"><see langword="true"/> if the item should be Unique, <see langword="false"/> otherwise.</param>
        public InventoryUniqueAttribute(bool unique)
        {
            Unique = unique;
        }
    }

    /// <summary>
    /// Marks an Inventory Item as Stackable.
    /// A Stackable item will be groupped into a single stack, and the extra properties will not be serialized.
    /// Items are Stackable by default, unless the item has the <see cref="InventoryUniqueAttribute"/>,
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InventoryStackableAttribute : Attribute
    {
        /// <summary>
        /// If the item is Stackable or not.
        /// </summary>
        public bool Stackable { get; private set; } = true;

        /// <summary>
        /// Marks an item as Stackable.
        /// </summary>
        public InventoryStackableAttribute()
        {
            Stackable = true;
        }

        /// <summary>
        /// Marks the item as stackable or not.
        /// </summary>
        /// <param name="stackable"><see langword="true"/> if the item is Stackable, <see langword="false"/> otherwise.</param>
        public InventoryStackableAttribute(bool stackable)
        {
            Stackable = stackable;
        }
    }
}
