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
            switch ((SHVDN2::GTA.Native.PedHash)SHVDN2::GTA.Game.Player.Character.Model)
            {
                case SHVDN2::GTA.Native.PedHash.Michael:
                case SHVDN2::GTA.Native.PedHash.Franklin:
                case SHVDN2::GTA.Native.PedHash.Trevor:
                    return true;
                default:
                    __result = Companion.Wallet.Money;
                    return false;
            }
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
            switch ((SHVDN2::GTA.Native.PedHash)SHVDN2::GTA.Game.Player.Character.Model)
            {
                case SHVDN2::GTA.Native.PedHash.Michael:
                case SHVDN2::GTA.Native.PedHash.Franklin:
                case SHVDN2::GTA.Native.PedHash.Trevor:
                    return true;
                default:
                    Companion.Wallet.Money = value;
                    return false;
            }
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
            switch ((SHVDN3::GTA.PedHash)SHVDN3::GTA.Game.Player.Character.Model)
            {
                case SHVDN3::GTA.PedHash.Michael:
                case SHVDN3::GTA.PedHash.Franklin:
                case SHVDN3::GTA.PedHash.Trevor:
                    return true;
                default:
                    __result = Companion.Wallet.Money;
                    return false;
            }
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
            switch ((SHVDN3::GTA.PedHash)SHVDN3::GTA.Game.Player.Character.Model)
            {
                case SHVDN3::GTA.PedHash.Michael:
                case SHVDN3::GTA.PedHash.Franklin:
                case SHVDN3::GTA.PedHash.Trevor:
                    return true;
                default:
                    Companion.Wallet.Money = value;
                    return false;
            }
        }
    }
}
