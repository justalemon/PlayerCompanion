using GTA;
using GTA.UI;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Windows.Forms;

namespace PlayerCompanion
{
    /// <summary>
    /// The script used to process the menus.
    /// </summary>
    public class Menus : Script
    {
        #region Public Properties

        /// <summary>
        /// The pool that holds the items.
        /// </summary>
        public static ObjectPool Pool { get; } = new ObjectPool();
        /// <summary>
        /// The menu used to show the current items in the inventory.
        /// </summary>
        public static NativeMenu Inventory { get; } = new NativeMenu("", "Inventory", "", null)
        {
            NoItemsText = "There are no items in your Inventory.",
            Alignment = Alignment.Right,
            RotateCamera = true
        };

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new game script to process the menus.
        /// </summary>
        public Menus()
        {
            // Add the menus to the pool
            Pool.Add(Inventory);
            // And add the events
            Tick += Menus_Tick;
            KeyDown += Menus_KeyDown;
        }

        #endregion

        #region Local Events

        private void Menus_Tick(object sender, EventArgs e)
        {
            Pool.Process();
        }

        private void Menus_KeyDown(object sender, KeyEventArgs e)
        {
            // If the player is using a keyboard and he pressed I, toggle the menu
            if (Game.LastInputMethod == InputMethod.MouseAndKeyboard && e.KeyCode == Keys.I)
            {
                Inventory.Visible = !Inventory.Visible;
            }
        }

        #endregion
    }
}
