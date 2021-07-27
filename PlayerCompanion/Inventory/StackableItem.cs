using System;

namespace PlayerCompanion
{
    /// <summary>
    /// Represents an inventory item that can be stored in stacks (like Minecraft).
    /// </summary>
    public abstract class StackableItem : Item
    {
        #region Fields

        private int count = 1;

        #endregion

        #region Properties

        /// <summary>
        /// The total number of items in this Stack.
        /// </summary>
        public int Count
        {
            get => count;
            set
            {
                count = value;
                CountChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// The maximum number of items that can be stored in a Stack.
        /// </summary>
        public virtual int Maximum { get; } = int.MaxValue;

        #endregion

        #region Events

        /// <summary>
        /// Event triggered when the Count of items changes.
        /// </summary>
        public event EventHandler CountChanged;

        #endregion
    }
}
