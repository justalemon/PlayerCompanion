using LemonUI.Elements;
using Newtonsoft.Json;
using PlayerCompanion.Converters;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents an inventory item that can be stored in stacks (like Minecraft).
    /// </summary>
    public abstract class StackableItem : Item
    {
        /// <summary>
        /// The total number of items in this Stack.
        /// </summary>
        public virtual int Count { get; set; }
        /// <summary>
        /// The maximum number of items that can be stored in a Stack.
        /// </summary>
        public virtual int Maximum { get; }
    }

    /// <summary>
    /// Represents a single Inventory Item.
    /// </summary>}
    [JsonConverter(typeof(ItemConverter))]
    public abstract class Item
    {
        #region Public Properties

        /// <summary>
        /// The name of the item.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// A custom white Sprite used as an icon for the item.
        /// </summary>
        public abstract ScaledTexture Icon { get; set; }
        /// <summary>
        /// The Monetary value of this item.
        /// </summary>
        public abstract int Value { get; }

        #endregion
    }
}
