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
}
