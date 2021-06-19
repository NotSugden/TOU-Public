using System;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButtonManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return true;
            var localPlayer = PlayerControl.LocalPlayer;
            if (!localPlayer.CanMove || localPlayer.Data.IsDead || !__instance.enabled) return false;
            var role = Role.GetRole<Undertaker>(localPlayer);
            if (__instance != role.DragDropButton) return true;
            MessageWriter messageWriter;
            if (role.DragDropButton.renderer.sprite == TownOfUs.DragSprite)
            {
				var body = role.CurrentTarget;
                if (__instance.isCoolingDown || body == null) return false;
                var isCloseEnough = (
					Vector2.Distance(localPlayer.GetTruePosition(), body.TruePosition) <
                    GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance]
				);
                if (!isCloseEnough) return false;
                messageWriter = AmongUsClient.Instance.StartRpcImmediately(localPlayer.NetId,
                    (byte)CustomRPC.DragBody, SendOption.Reliable, -1
                );
                messageWriter.Write(localPlayer.PlayerId);
                messageWriter.Write(body.ParentId);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                role.CurrentlyDragging = body;
                KillButtonTarget.SetTarget(__instance, null, role);
                __instance.renderer.sprite = TownOfUs.DropSprite;
            }
            else
            {
                messageWriter = AmongUsClient.Instance.StartRpcImmediately(localPlayer.NetId,
                    (byte) CustomRPC.DropBody, SendOption.Reliable, -1
                );
                messageWriter.Write(localPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);
                role.CurrentlyDragging.bodyRenderer.material.SetFloat("_Outline", 0f);
                role.CurrentlyDragging = null;
                __instance.renderer.sprite = TownOfUs.DragSprite;
                role.LastDragged = DateTime.UtcNow;
            }
            return false;
        }
    }
}
