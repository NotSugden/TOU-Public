using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] {
        typeof(UnityEngine.Object)
    })]
    public static class HUDClose
    {
        public static void Postfix(UnityEngine.Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker)) return;
            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
            role.DragDropButton.renderer.sprite = TownOfUs.DragSprite;
            role.CurrentlyDragging = null;
            role.LastDragged = DateTime.UtcNow;
        }
    }
}
