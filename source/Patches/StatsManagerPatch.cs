using HarmonyLib;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(StatsManager))]
    public static class StatsManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static void Postfix(out bool __result) => __result = false;
    }
}
