using GTA;
using System;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Game Script that handles the Companion Processing.
    /// </summary>
    public class Companion : Script
    {

        #region Public Properties

        /// <summary>
        /// If the Companion Features are ready to work.
        /// </summary>
        public static bool Ready { get; private set; } = false;
        /// <summary>
        /// The Inventory of the Player.
        /// </summary>
        public static InventoryManager Inventory { get; private set; } = null;
        /// <summary>
        /// The manager for the Ped Money.
        /// </summary>
        public static MoneyManager Money { get; private set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Companion worker.
        /// </summary>
        public Companion()
        {
            // Get the assembly that called and it's name
            Assembly assembly = Assembly.GetCallingAssembly();
            AssemblyName name = assembly.GetName();
            // If the name is not ScriptHookVDotNet, raise an exception
            if (name.Name != "ScriptHookVDotNet")
            {
                throw new InvalidOperationException($"This Class can only be started by ScriptHookVDotNet (it was called from '{name.Name}').");
            }

            // Create the instances of the classes that we use
            Inventory = new InventoryManager();
            Money = new MoneyManager(this);
            new ColorManager(this);
            new WeaponManager(this);

            // Finally, mark the script as ready
            Ready = true;
        }

        #endregion
    }
}
