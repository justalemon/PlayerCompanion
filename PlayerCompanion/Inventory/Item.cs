using LemonUI.Elements;
using Newtonsoft.Json;
using PlayerCompanion.Converters;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents an item that can be added to the inventory.
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
        /// A custom Sprite used as an icon for the item.
        /// </summary>
        public abstract ScaledTexture Icon { get; set; }
        /// <summary>
        /// The Monetary value of this item.
        /// </summary>
        public abstract int Value { get; }
        /// <summary>
        /// The total number of items.
        /// </summary>
        public virtual int Count { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="Item"/> with a specific count.
        /// </summary>
        /// <param name="count">The total number of this <see cref="Item"/>.</param>
        public Item(int count)
        {
            Count = count;
        }

        #endregion
    }
}
