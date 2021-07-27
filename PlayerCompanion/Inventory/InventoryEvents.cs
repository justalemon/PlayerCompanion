namespace PlayerCompanion
{
    /// <summary>
    /// Represents the event triggered when an Inventory Item is changed.
    /// </summary>
    /// <param name="sender">The Inventory that triggered the event.</param>
    /// <param name="e">The Item information.</param>
    public delegate void ItemChangedEventHandler(object sender, ItemChangedEventArgs e);

    /// <summary>
    /// The Item information of an item when is changed.
    /// </summary>
    public class ItemChangedEventArgs
    {
        #region Properties

        /// <summary>
        /// The item that was changed.
        /// </summary>
        public Item Item { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="ItemChangedEventArgs"/>.
        /// </summary>
        /// <param name="item">The item that was Changed.</param>
        public ItemChangedEventArgs(Item item)
        {
            Item = item;
        }

        #endregion
    }
}
