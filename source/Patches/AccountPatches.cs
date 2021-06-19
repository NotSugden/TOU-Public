﻿using HarmonyLib;
using InnerNet;

namespace TownOfUs {
    [HarmonyPatch(typeof(StatsManager))]
    public static class StatsManagerPatches {
        [HarmonyPostfix()]
        [HarmonyPatch(nameof(StatsManager.AmBanned), MethodType.Getter)]
        public static void AmBannedPatch(out bool __result) => __result = false;
    }

    [HarmonyPatch(typeof(SaveManager))]
    public static class SaveManagerPatches {
        /*[HarmonyPostfix()]
        [HarmonyPatch(nameof(SaveManager.AccountLoginStatus), MethodType.Getter)]
        public static void AccountLoginStatusPatch(out EOSManager.AccountLoginStatus __result) =>
            __result = EOSManager.AccountLoginStatus.LoggedIn;*/

        [HarmonyPostfix()]
        [HarmonyPatch(nameof(SaveManager.ChatModeType), MethodType.Getter)]
        public static void ChatModeTypePatch(out QuickChatModes __result) => __result = QuickChatModes.FreeChatOrQuickChat;

        [HarmonyPostfix()]
        [HarmonyPatch(nameof(SaveManager.BirthDateYear), MethodType.Getter)]
        public static void BirthDateYearPatch(out int __result) => __result = 2000;
    }

    [HarmonyPatch(typeof(AccountManager))]
    public static class AccountManagerPatches
    {
        [HarmonyPrefix()]
        [HarmonyPatch(nameof(AccountManager.RandomizeName))]
        public static bool RandomizeNamePatch() => false;
    }
}
