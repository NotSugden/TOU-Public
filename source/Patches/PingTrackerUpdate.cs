﻿﻿using HarmonyLib;
using UnityEngine;

namespace TownOfUs
{
    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.1f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text =
                $"<color=#00FF00FF>TownOfUs v{TownOfUs.Version}</color>\n" +
                $"(unofficial) Custom ToU Patch by Sugden\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms\n" +
                (!MeetingHud.Instance
                    ? "<color=#00FF00FF>slushiegoose ft. edisonparklive</color>"
                    : "");
        }
    }
}
