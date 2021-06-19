using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using InnerNet;
using Reactor.Extensions;

namespace TownOfUs
{
    public static class VentAnimationsPatch
    {
        public static bool Patch(Vent vent, PlayerControl player)
        {
            if (CustomGameOptions.VentAnimations && !player.Is(RoleEnum.Phantom)) return true;

            if (player.AmOwner) Vent.currentVent = vent;

            return false;
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
        public static class EnterVent
        {
            public static bool Prefix(Vent __instance, [HarmonyArgument(0)] PlayerControl player) => Patch(__instance, player);
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
        public static class ExitVent
        {
            public static bool Prefix(Vent __instance, [HarmonyArgument(0)] PlayerControl player) => Patch(__instance, player);
        }

        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
        public static class RainbowGameCode
        {
            public static void Postfix(GameStartManager __instance)
            {
                var code = GameCode.IntToGameName(AmongUsClient.Instance.GameId);
                if (code == "SUGDEN")
                {
                    __instance.GameRoomName.color = RainbowUtils.Rainbow;
                    return;
                }
                var color = code switch
                {
                    "ATZE" => new Color32(240, 125, 13, 255),
                    "ROLF" => new Color32(186, 161, 255, 255),
                    "STELLL" => new Color32(80, 240, 57, 255),
                    "FIVEUP" => new Color32(238, 84, 187, 255),
                    "MOTR" => new Color32(238, 84, 187, 255),
                    "PYRO" => new Color32(56, byte.MaxValue, 221, 255),
                    "JAHR" => new Color32(240, 211, 165, 255),
                    "FINN" => new Color32(236, 61, 255, 255),
                    _ => new Color32(255, 255, 255, 255)
                };
                __instance.GameRoomName.color = color;
            }
        }
    }
}

