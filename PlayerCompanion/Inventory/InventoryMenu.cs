using System.Collections.Generic;
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

        public void RecreateMenu()
        {
            Clear();

            foreach (Item item in Companion.Inventories.Current.Items)
            {
                if (item is StackableItem stackableItem && stackableItem.Count == 0)
                {
                    continue;
                }
                
                Add(new InventoryItem(item));
            }
        }
        public void UpdateItems()
        {
            foreach (NativeItem item in new List<NativeItem>(Items))
            {
                InventoryItem inventoryItem = (InventoryItem)item;

                if (inventoryItem.Item is StackableItem stackableItem && stackableItem.Count == 0)
                {
                    Remove(item);
                }
            }
        }

        #endregion
    }
}
