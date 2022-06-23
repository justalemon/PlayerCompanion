using LemonUI.Menus;

namespace PlayerCompanion
{
    /// <summary>
    /// LemonUI item used to show the Inventory Items in the menu.
    /// </summary>
    internal class InventoryItem : NativeItem
    {
        #region Properties

        /// <summary>
        /// The Inventory Item controlled by this Menu Item.
        /// </summary>
        public Item Item { get; }

        #endregion

        #region Constructor

        internal InventoryItem(Item item) : base(item.Name, "", item is StackableItem s ? s.Count.ToString() : "")
        {
            Item = item;
            Activated += (sender, e) => Item.Use();
            if (item is StackableItem stackable)
            {
                stackable.CountChanged += (sender, e) => Title = $"{stackable.Name} ({stackable.Count})";
            }
        }

        #endregion
    }
}
