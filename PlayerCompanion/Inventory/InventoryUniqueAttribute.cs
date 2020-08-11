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
}
