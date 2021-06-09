using System.Linq;
using HarmonyLib;
using TownOfUs.CustomHats;
using UnityEngine;
using Reactor.Extensions;

namespace TownOfUs
{

    //[HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            AspectPosition position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3f, 0.4f, 0);
            position.AdjustPosition();

            __instance.text.text =
                Utils.ColorText(RainbowUtils.Rainbow, $"TownOfUs v{TownOfUs.Version}\n") +
                Utils.ColorText(Color.green, "Originally made by Slushiegoose") +
                $"Custom Patch By Sugden\n" +
                $"Ping: {AmongUsClient.Instance.Ping}ms";

        }
    }
}
