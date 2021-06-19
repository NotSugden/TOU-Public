using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs
{
    public static class VentExtensions
    {
        [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
        public static class Vent_CanUse
        {
            public static bool Prefix(Vent __instance, ref float __result, [HarmonyArgument(0)] GameData.PlayerInfo pc,
                [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
            {
                var num = float.MaxValue;
                var localPlayer = pc.Object;
                var localRole = Role.GetRole(localPlayer);
                bool IsRole(RoleEnum[] roles)
                {
                    if (localRole == null) return false;
                    foreach (var role in roles)
                    {
                        if (localRole.RoleType == role) return true;
                    }
                    return false;
                }
                var isAlive = !localPlayer.Data.IsDead;
                if (IsRole(new[] { RoleEnum.Undertaker }))
                {
                    couldUse = isAlive && ((Undertaker)localRole).CurrentlyDragging == null;
                }
                else
                {
                    couldUse = IsRole(new[] { RoleEnum.Phantom }) || (isAlive && (
                        IsRole(new[] { RoleEnum.Engineer }) ||
                        localPlayer.Data.IsImpostor &&
                        !IsRole(new RoleEnum[] { RoleEnum.Morphling, RoleEnum.Swooper })
                    ));
                }
                canUse = couldUse;

                num = Vector2.Distance(localPlayer.GetTruePosition(), __instance.transform.position);
                canUse &= num <= __instance.UsableDistance;


                __result = num;
                return false;
            }
        }

        [HarmonyPatch(typeof(Vent), nameof(Vent.SetOutline))]
        public static class Vent_SetOutline
        {
            public static bool Prefix(
                Vent __instance,
                [HarmonyArgument(0)] bool on,
                [HarmonyArgument(1)] bool mainTarget
            )
            {
                var material = __instance.myRend.material;
                var color = Role.GetRole(PlayerControl.LocalPlayer)?.Color ?? Color.red;
                material.SetFloat("_Outline", (on ? 1f : 0f));
                material.SetColor("_OutlineColor", color);
                material.SetColor("_AddColor", mainTarget ? color : Color.clear);
                return false;
            }
        }
    }
}
