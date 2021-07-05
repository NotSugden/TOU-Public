using HarmonyLib;
using Il2CppSystem.Collections.Generic;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
    public static class MedScanPatch
    {
        public static void Prefix(MedScanMinigame __instance)
        {
            if (!__instance.MyNormTask.IsComplete)
                __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
        }
    }
}
