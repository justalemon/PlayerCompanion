namespace PlayerCompanion
{
    /// <summary>
    /// Represents an Item that can't be used.
    /// Invalid Items are automatically created when a menu item can't be restored.
    /// </summary>
    public sealed class InvalidItem : Item
    {
        #region Public Properties

        /// <summary>
        /// The Name of this Invalid Item.
        /// </summary>
        public override string Name { get; }
        /// <summary>
        /// The Price of this Invalid Item.
        /// </summary>
        public override int Price { get; } = 0;
        /// <summary>
        /// The Type that could not be created during initialization.
        /// </summary>
        public string Type { get; }

        #endregion

        #region Constructors

        internal InvalidItem(string type, int count) : base(count)
        {
            Name = $"Invalid ({type})";
            Type = type;
        }

        #endregion
    }
}
