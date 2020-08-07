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
        /// The total number of items.
        /// </summary>
        public virtual int Count { get; set; }
        /// <summary>
        /// The Dictionary where the Item image could be found.
        /// </summary>
        public virtual string ImageDictionary => "";
        /// <summary>
        /// The Sprite of the Item image.
        /// </summary>
        public virtual string ImageSprite => "";
        /// <summary>
        /// If this inventory item has a valid image to use.
        /// </summary>
        public virtual bool HasImage => false;

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
