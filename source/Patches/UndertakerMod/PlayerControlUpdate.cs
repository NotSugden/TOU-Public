using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnhollowerBaseLib;
using UnityEngine;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class PlayerControlUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (localPlayer?.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return;
            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
            if (role.DragDropButton == null)
            {
                role.DragDropButton = UnityEngine.Object.Instantiate(__instance.KillButton, DestroyableSingleton<HudManager>.Instance.transform);
                role.DragDropButton.renderer.enabled = true;
                role.DragDropButton.renderer.sprite = TownOfUs.DragSprite;
            }
            if (role.DragDropButton.renderer.sprite != TownOfUs.DragSprite && role.DragDropButton.renderer.sprite != TownOfUs.DropSprite)
            {
                role.DragDropButton.renderer.sprite = TownOfUs.DragSprite;
            }
            if (role.DragDropButton.renderer.sprite == TownOfUs.DropSprite && role.CurrentlyDragging == null)
            {
                role.DragDropButton.renderer.sprite = TownOfUs.DragSprite;
            }
            role.DragDropButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var localPosition = __instance.KillButton.transform.localPosition;
            role.DragDropButton.transform.localPosition = new Vector3(localPosition.x, __instance.ReportButton.transform.localPosition.y, localPosition.z);
            if (role.DragDropButton.renderer.sprite == TownOfUs.DragSprite)
            {
                var data = localPlayer.Data;
                var isDead = data.IsDead;
                var truePosition = localPlayer.GetTruePosition();
                var maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                var flag = (PlayerControl.GameOptions.GhostsDoTasks || !data.IsDead) &&
                           (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && localPlayer.CanMove;
                var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                    LayerMask.GetMask(new[] { "Players", "Ghost" }));
                var killButton = role.DragDropButton;
                DeadBody closestBody = null;
                var closestDistance = float.MaxValue;

                foreach (var collider2D in allocs)
                {
                    if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                    var component = collider2D.GetComponent<DeadBody>();
                    if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                          maxDistance)) continue;

                    var distance = Vector2.Distance(truePosition, component.TruePosition);
                    if (!(distance < closestDistance)) continue;
                    closestBody = component;
                    closestDistance = distance;
                }
                KillButtonTarget.SetTarget(killButton, closestBody, role);
            }
            if (role.DragDropButton.renderer.sprite == TownOfUs.DragSprite)
            {
                role.DragDropButton.SetCoolDown(role.DragTimer(), CustomGameOptions.DragCooldown);
            }
            else
            {
                role.DragDropButton.SetCoolDown(0f, 1f);
                role.DragDropButton.renderer.color = Palette.EnabledColor;
                role.DragDropButton.renderer.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
