using LemonUI.Elements;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents an Item that could not be restored when the script was loaded.
    /// </summary>
    public sealed class InvalidItem : StackableItem
    {
        #region Properties

        /// <summary>
        /// The Name of this Invalid Item.
        /// </summary>
        public override string Name { get; }
        /// <summary>
        /// The Price of this Invalid Item.
        /// </summary>
        public override int Value { get; } = 0;
        /// <summary>
        /// The Type that could not be created during initialization.
        /// </summary>
        public string Type { get; }
        /// <summary>
        /// The icon of the invalid item.
        /// </summary>
        public override ScaledTexture Icon { get; } = new ScaledTexture("timerbar_icons", "pickup_random");

        #endregion

        #region Constructors

        internal InvalidItem(string type, int count)
        {
            Name = $"Invalid ({type})";
            Type = type;
            Count = count;
        }

        #endregion
    }
}
