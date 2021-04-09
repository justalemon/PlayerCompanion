extern alias SHVDN3;

using HarmonyLib;

namespace PlayerCompanion.RuntimePatching
{
    /// <summary>
    /// Patches the calls to Game.Player.Money in SHVDN 2 and 3.
    /// </summary>
    public class Patcher : SHVDN3::GTA.Script
    {
        public Patcher()
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            Harmony harmony = new Harmony("PlayerCompanion.RuntimePatching");
            harmony.PatchAll();
#if DEBUG
            FileLog.FlushBuffer();
#endif
        }
    }
}
