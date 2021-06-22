using HarmonyLib;

namespace TownOfUs.SoloImpostorMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FindClosestTarget))]
    public static class TargetPatch
    {
        public static bool Prefix(PlayerControl __instance, ref PlayerControl __result)
        {
            if (!CustomGameOptions.AnonImpostors) return true;

            var target = Utils.getClosestPlayer(__instance);

            var dist = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];

            if (target == null || Utils.getDistBetweenPlayers(target, PlayerControl.LocalPlayer) < dist)
            {
                __result = target;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GameOptionsData), nameof(GameOptionsData.GetAdjustedNumImpostors))]
    public static class ImpostorCountPatch
    {
        public static bool Prefix(ref int __result)
        {
            if (!CustomGameOptions.AnonImpostors) return true;

            __result = PlayerControl.GameOptions.NumImpostors;

            return false;
        }
    }
}
