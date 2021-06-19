using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.UndertakerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class DragBody
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Undertaker)) return;
            var role = Role.GetRole<Undertaker>(__instance);
            var body = role.CurrentlyDragging;
            if (body == null) return;
            var currentPosition = __instance.GetTruePosition();
            if (!PhysicsHelpers.AnythingBetween(
				currentPosition,
				((Vector2) __instance.transform.position) + body.myCollider.offset,
				Constants.ShipAndObjectsMask,
				false
			)) body.transform.position = __instance.transform.position;
            if (!__instance.AmOwner) return;
            var material = body.bodyRenderer.material;
            material.SetColor("_OutlineColor", Color.green);
            material.SetFloat("_Outline", 1f);
        }
    }
}
