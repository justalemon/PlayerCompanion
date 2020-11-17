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
            string configLoc = Path.Combine(location, "Config.json");
            if (File.Exists(configLoc))
            {
                config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(configLoc));
            }
            // Otherwise, create a new one and save it
            else
            {
                config = new Configuration();
                Directory.CreateDirectory(location);
                File.WriteAllText(Path.Combine(), JsonConvert.SerializeObject(config));
            }
        }

        #endregion
    }
}
