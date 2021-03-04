extern alias SHVDN2;
using GTA;
using HarmonyLib;

namespace PlayerCompanion
{
    [HarmonyPatch(typeof(SHVDN2::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Getter)]
    internal static class HarmonyMoneyGetterPatch2
    {
        public static bool Prefix(ref int __result)
        {
            __result = Companion.Wallet.Money;
            return false;
        }
    }

    [HarmonyPatch(typeof(SHVDN2::GTA.Player))]
    [HarmonyPatch("Money", MethodType.Setter)]
    internal static class HarmonyMoneySetterPatch2
    {
        public static bool Prefix(ref int value)
        {
            Companion.Wallet.Money = value;
            return false;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Money", MethodType.Getter)]
    internal static class HarmonyMoneyGetterPatch3
    {
        public static bool Prefix(ref int __result)
        {
            __result = Companion.Wallet.Money;
            return false;
        }
    }

    [HarmonyPatch(typeof(Player))]
    [HarmonyPatch("Money", MethodType.Setter)]
    internal static class HarmonyMoneySetterPatch3
    {
        public static bool Prefix(ref int value)
        {
            Companion.Wallet.Money = value;
            return false;
        }
    }
}
