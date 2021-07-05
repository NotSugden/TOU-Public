using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(GameStartManager))]
    public static class LobbyTimer
    {
        public static float Timer = 0f;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameStartManager.Start))]
        public static void Start() => Timer = 0f;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameStartManager.Update))]
        public static void Update(GameStartManager __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            Timer += Time.deltaTime;

            var seconds = Mathf.Floor(Timer);
            var minutes = Mathf.FloorToInt(seconds / 60);
            seconds -= minutes * 60;
            var players = GameData.Instance.PlayerCount;
            var min = __instance.MinPlayers;

            var color = "FF0000FF";
            if (players > min)
                color = "00FF00FF";
            else if (players == min)
                color = "FFFF00FF";

            __instance.PlayerCounter.text =
                $"<color=#{color}>{players}/{PlayerControl.GameOptions.MaxPlayers}</color>\n" +
                $"<size=2.5>({minutes:00}:{seconds:00})</size>";

        }
    }
}
