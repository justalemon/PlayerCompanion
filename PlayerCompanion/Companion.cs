using GTA;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace PlayerCompanion
{
    /// <summary>
    /// Main class for managing the player information.
    /// </summary>
    public class Companion : Script
    {
        #region 

        private static Model lastModel = Game.Player.Character.Model;

        internal static string location = Path.Combine(new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath, "PlayerCompanion");
        internal static Configuration config = null; 

        #endregion

        #region Properties

        /// <summary>
        /// If the PlayerCompanion features are ready to be used.
        /// </summary>
        public static bool IsReady { get; private set; } = false;
        /// <summary>
        /// The manager for the Player Money.
        /// </summary>
        public static MoneyManager Money { get; private set; } = new MoneyManager();
        /// <summary>
        /// Manages the colors in the HUD and Radar.
        /// </summary>
        public static ColorManager Colors { get; private set; } = new ColorManager();
        /// <summary>
        /// Makages the Inventories of the Players
        /// </summary>
        public static InventoryManager Inventories { get; private set; } = new InventoryManager();

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new PlayerCompanion Script.
        /// Please note that this can only be done by SHVDN.
        /// </summary>
        public Companion()
        {
            // Get the assembly that called and it's name
            Assembly assembly = Assembly.GetCallingAssembly();
            AssemblyName name = assembly.GetName();
            // If the name is not ScriptHookVDotNet, raise an exception
            if (name.Name != "ScriptHookVDotNet")
            {
                throw new InvalidOperationException($"PlayerCompanion can only be started by ScriptHookVDotNet (it was called from '{name.Name}').");
            }

            // If there is a configuration file, load it
            string path = Path.Combine(location, "Config.json");
            if (File.Exists(path))
            {
                config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(path));
            }
            // Otherwise, create a new one and save it
            else
            {
                config = new Configuration();
                Directory.CreateDirectory(location);
                File.WriteAllText(Path.Combine(location, path), JsonConvert.SerializeObject(config));
            }

            // Finally, add the events that we need
            Tick += Companion_Tick;
            Aborted += Companion_Aborted;
        }

        #endregion

        #region Events

        private void Companion_Tick(object sender, EventArgs e)
        {
            // If PlayerCompanion is not ready to work, perform the initialization
            if (!IsReady)
            {
                Inventories.Load(Game.Player.Character.Model);
                IsReady = true;
            }

            // If the Player Ped Model has been changed, make the required updates
            if (Game.Player.Character.Model != lastModel)
            {
                if (Colors.HasCustomColor(Game.Player.Character.Model))
                {
                    Colors.Apply(Colors.Current);
                }
                Inventories.Load(Game.Player.Character.Model);
                lastModel = Game.Player.Character.Model;
            }
        }

        private void Companion_Aborted(object sender, EventArgs e)
        {
            // Do the required cleanup tasks
            Colors.RestoreDefault();
        }

        #endregion
    }
}
