using System;
using System.IO;
using System.Reflection;

namespace PlayerCompanion
{
    internal static class Locations
    {
        public static string ScriptRoot { get; } = new Uri(Path.GetDirectoryName(Assembly.GetCallingAssembly().CodeBase)).LocalPath;
        public static string ModWorkDir { get; } = Path.Combine(ScriptRoot, "PlayerCompanion");
        public static string InventoryData { get; } = Path.Combine(ModWorkDir, "Inventory");
        public static string Colors { get; } = Path.Combine(ModWorkDir, "Colors.json");
        public static string Money { get; } = Path.Combine(ModWorkDir, "Money.json");
        public static string ConfigInventory { get; } = Path.Combine(ModWorkDir, "Inventory.json");
    }
}
