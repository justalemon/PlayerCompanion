using LemonUI.Elements;
using Newtonsoft.Json;
using System;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents a single Inventory Item.
    /// </summary>
    [JsonConverter(typeof(ItemConverter))]
    public abstract class Item
    {
        #region Fields

        internal PedInventory inventory = null;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the item shown on specific inventory interfaces.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// The description of the name shown on specific inventory interfaces.
        /// </summary>
        public virtual string Description => "No Description Available.";
        /// <summary>
        /// A custom white Sprite used as an icon for the item.
        /// </summary>
        public abstract ScaledTexture Icon { get; }
        /// <summary>
        /// The Monetary value of this item.
        /// </summary>
        public virtual int Value => 0;
        /// <summary>
        /// The inventory that this Item is part of.
        /// </summary>
        public PedInventory Inventory => inventory;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the Item has been used.
        /// </summary>
        public event EventHandler Used;

        #endregion

        #region Functions

        /// <summary>
        /// Removes the Item from it's existing inventory.
        /// </summary>
        public void Remove()
        {
            if (inventory == null)
            {
                return;
            }

            inventory.Remove(this);
            inventory = null;
        }
        /// <summary>
        /// Uses the item.
        /// </summary>
        public void Use() => Used?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
