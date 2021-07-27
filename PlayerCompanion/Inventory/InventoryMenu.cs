using LemonUI.Menus;

namespace PlayerCompanion
{
    /// <summary>
    /// Simple LemonUI menu used for showing the inventory to the player.
    /// </summary>
    internal class InventoryMenu : NativeMenu
    {
        #region Constructor

        internal InventoryMenu() : base("Inventory", "Inventory", "Inventory", null)
        {
            Alignment = GTA.UI.Alignment.Right;
            NoItemsText = "There are no items in your Inventory.";
        }

        #endregion

        #region Functions

        /// <summary>
        /// Reloads all of the items on the inventory.
        /// </summary>
        public void ReloadItems()
        {
            // Clear all of the items
            Clear();

            // And add all of the items one by one
            foreach (Item item in Companion.Inventories.Current.Items)
            {
                Add(new InventoryItem(item));
            }

            // If we ended up with items, reset the selected index
            if (Items.Count > 0)
            {
                SelectedIndex = 0;
            }
        }

        #endregion
    }
}
