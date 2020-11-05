using System;

namespace PlayerCompanion
{
    /// <summary>
    /// Marks a Class as a Unique Inventory Item.
    /// Unique Inventory Items can't be found in the wild and will not be returned when calling <see cref="Inventory.CreateRandomItem()"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InventoryUniqueAttribute : Attribute
    {
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
