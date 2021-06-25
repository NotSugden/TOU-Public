using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs
{
    public static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        public static class GameData_RecomputeTaskCounts
        {
            public static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                foreach (var playerData in __instance.AllPlayers)
                {
                    if (
                        playerData.IsImpostor ||
                        playerData.Disconnected ||
                        playerData.Tasks == null ||
                        (playerData.IsDead && !PlayerControl.GameOptions.GhostsDoTasks)
                    ) continue;
                    var player = playerData.Object;
                    if (player == null || !player.Is(Faction.Crewmates)) continue;

                    foreach (var task in playerData.Tasks)
                    {
                        __instance.TotalTasks++;
                        if (task.Complete) __instance.CompletedTasks++;
                    }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        public class Console_CanUse
        {
            public static bool Prefix(
                Console __instance,
                [HarmonyArgument(0)] GameData.PlayerInfo playerData,
                [HarmonyArgument(1)] out bool couldUse,
                [HarmonyArgument(2)] out bool canUse
            )
            {
                var player = playerData.Object;

                var hasTasks = player.Is(Faction.Crewmates);
                var playerPosition = player.GetTruePosition();
                var consolePosition = __instance.transform.position;

                couldUse = canUse = (
                    !playerData.IsDead ||
                    PlayerControl.GameOptions.GhostsDoTasks &&
                    !__instance.GhostsIgnored
                ) && player.CanMove && (
                    __instance.AllowImpostor || hasTasks
                ) && (
                    !__instance.onlySameRoom || __instance.InRoom(playerPosition)
                ) && (
                    !__instance.onlyFromBelow || playerPosition.y < consolePosition.y
                ) && __instance.FindTask(player);

                if (couldUse)
                {
                    var num = Vector2.Distance(playerPosition, __instance.transform.position);
                    couldUse &= num <= __instance.UsableDistance;
                    if (__instance.checkWalls)
                        couldUse &= !PhysicsHelpers.AnythingBetween(
                            playerPosition, consolePosition,
                            Constants.ShadowMask, false
                        );
                }

                return false;
            }
        }
    }
}
