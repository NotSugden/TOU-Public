using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles;

/*namespace TownOfUs
{
    [HarmonyPatch(typeof(KillOverlay), nameof(KillOverlay.CoShowOne), typeof(OverlayKillAnimation), typeof(GameData.PlayerInfo), typeof(GameData.PlayerInfo))]
    public class KillAnimation_KillPatch
    {
        public static bool Prefix(
            KillOverlay __instance,
            [HarmonyArgument(1)] GameData.PlayerInfo _killer
        )
        {
            var killer = _killer.Object;
            var role = Role.GetRole(killer);
            if (role != null) {
                var renderer = __instance.flameParent.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
                renderer.color = role.RoleType == RoleEnum.LoverImpostor ? Palette.ImpostorRed : role.Color;
                renderer.sprite = TownOfUs.GreyscaleKill;
            }
			return true;
        }
    }
}
*/