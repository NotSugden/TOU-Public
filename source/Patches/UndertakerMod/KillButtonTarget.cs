using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.SetTarget))]
    public class KillButtonTarget
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return true;
            return __instance == DestroyableSingleton<HudManager>.Instance.KillButton;
        }

        public static void SetTarget(KillButtonManager __instance, DeadBody target, Undertaker role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                role.CurrentTarget.bodyRenderer.material.SetFloat("_Outline", 0f);
            }
            role.CurrentTarget = target;
            if (target && __instance.enabled)
            {
                var material = target.bodyRenderer.material;
                material.SetFloat("_Outline", 1f);
                material.SetColor("_OutlineColor", Color.yellow);
                __instance.renderer.color = Palette.EnabledColor;
                __instance.renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                __instance.renderer.color = Palette.DisabledClear;
                __instance.renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
