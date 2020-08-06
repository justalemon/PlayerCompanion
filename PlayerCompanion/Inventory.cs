using GTA;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Script that handles the player inventory.
    /// </summary>
    public class Inventory : Script
    {
        #region Private Fields

        private static readonly string locationMod = new Uri(Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase)).LocalPath;
        private static readonly string locationFolder = Path.Combine(locationMod, "PlayerCompanion");
        private static readonly string locationConfig = Path.Combine(locationFolder, "Inventory.json");

        #endregion

        #region Public Properties

        /// <summary>
        /// If the inventory is ready to work.
        /// </summary>
        public static bool Ready { get; private set; } = false;
        /// <summary>
        /// The configuration of the inventory.
        /// </summary>
        public static InventoryConfiguration Configuration { get; private set; } = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Inventory Script.
        /// </summary>
        public Inventory()
        {
            // If the configuration file does exists, load it
            if (File.Exists(locationConfig))
            {
                string contents = File.ReadAllText(locationConfig);
                Configuration = JsonConvert.DeserializeObject<InventoryConfiguration>(contents);

            }
            // Otherwise, create a new one and save it
            else
            {
                Configuration = new InventoryConfiguration();
                SaveConfiguration();
            }

            // Finally, say that we are ready to work
            Ready = true;
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Saves the configuration of the inventory.
        /// </summary>
        public static void SaveConfiguration()
        {
            // Dump the contents to a JSON String
            string contents = JsonConvert.SerializeObject(Configuration);
            // Create the folder (if there is none)
            Directory.CreateDirectory(locationFolder);
            // And dumps the contents of the file
            File.WriteAllText(locationConfig, contents);
        }

        #endregion
    }
}
