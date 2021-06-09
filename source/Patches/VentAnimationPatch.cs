using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace TownOfUs.Patches
{
    public static class VentAnimationsPatch
    {
        public static bool Patch(Vent vent, PlayerControl player)
        {
            if (CustomGameOptions.VentAnimations) return true;

            if (player.AmOwner) Vent.currentVent = vent;

            return false;
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
        public class EnterVent
        {
            public static bool Prefix(Vent __instance, [HarmonyArgument(0)] PlayerControl player) => Patch(__instance, player);
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
        public class ExitVent
        {
            public static bool Prefix(Vent __instance, [HarmonyArgument(0)] PlayerControl player) => Patch(__instance, player);
        }
    }
}

