using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(PlayerPhysics), "FixedUpdate")]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Undertaker)) return;
            var role = Role.GetRole<Undertaker>(__instance.myPlayer);
            if (role.CurrentlyDragging == null) return;
            if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
            {
                __instance.body.velocity /= 2f;
            }
        }
    }
}
