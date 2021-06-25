using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs
{
    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentUseable
    {
        private static bool CanVent(PlayerControl player)
        {
            return player.Data.IsImpostor
                ? !player.IsAny(new RoleEnum[] { RoleEnum.Swooper, RoleEnum.Morphling })
                : player.Is(RoleEnum.Engineer);
        }


        public static bool Prefix(
            Vent __instance,
            [HarmonyArgument(0)] GameData.PlayerInfo playerData,
            [HarmonyArgument(1)] out bool canUse,
            [HarmonyArgument(2)] out bool couldUse,
            ref float __result
        )
        {
            var player = playerData.Object;
            couldUse = !playerData.IsDead && CanVent(player);
            canUse = couldUse;

            var num = Vector2.Distance(player.GetTruePosition(), __instance.transform.position);
            canUse &= num <= __instance.UsableDistance;


            __result = num;
            return false;
        }
    }
}
