extern alias SHVDN2;
extern alias SHVDN3;
using HarmonyLib;

namespace PlayerCompanion.RuntimePatching
{
    /// <summary>
    /// Patches for the Getter in SHVDN2.
    /// </summary>
    [HarmonyPatch(typeof(SHVDN2::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Getter)]
    internal static class MoneyGetterPatch2
    {
        public static bool Prefix(ref int __result)
        {
            __result = Companion.Wallet.Money;
            return false;
        }
    }

    /// <summary>
    /// Patches for the Setter in SHVDN2.
    /// </summary>
    [HarmonyPatch(typeof(SHVDN2::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Setter)]
    internal static class MoneySetterPatch2
    {
        public static bool Prefix(ref int value)
        {
            Companion.Wallet.Money = value;
            return false;
        }
    }

    /// <summary>
    /// Patches for the Getter in SHVDN3.
    /// </summary>
    [HarmonyPatch(typeof(SHVDN3::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Getter)]
    internal static class MoneyGetterPatch3
    {
        public static bool Prefix(ref int __result)
        {
            __result = Companion.Wallet.Money;
            return false;
        }
    }

    /// <summary>
    /// Patches for the Setter in SHVDN3.
    /// </summary>
    [HarmonyPatch(typeof(SHVDN3::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Setter)]
    internal static class MoneySetterPatch3
    {
        public static bool Prefix(ref int value)
        {
            Companion.Wallet.Money = value;
            return false;
        }
    }
}
