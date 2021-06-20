using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    class KillButtonPatches
    {
        public static void SetTimer(ref float killTimer, float max, KillButtonManager killButton)
        {
            if (killButton == null) return;
            killTimer = Mathf.Clamp(killTimer - Time.deltaTime, 0f, max);
            killButton.SetCoolDown(killTimer, max);
        }

        [HarmonyPostfix()]
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void UpdateKillTimer()
        {
			if (!ShipStatus.Instance) return;
            var player = PlayerControl.LocalPlayer;
            if (!player.CanMove) return;
            var role = Role.GetRole(player);
            if (role == null) return;
            
            var killCooldown = PlayerControl.GameOptions.KillCooldown;
            switch (role.RoleType)
            {
                case RoleEnum.Sheriff:
                    var sheriff = (Sheriff)role;
                    SetTimer(ref sheriff.KillTimer, killCooldown, HudManager.Instance.KillButton);
                    break;
                case RoleEnum.Glitch:
                    var glitch = (Glitch)role;
                    SetTimer(ref glitch.KillTimer, killCooldown, HudManager.Instance.KillButton);
                    SetTimer(ref glitch.HackTimer, CustomGameOptions.HackCooldown, glitch.HackButton);
                    SetTimer(ref glitch.MimicTimer, CustomGameOptions.MimicCooldown, glitch.MimicButton);
                    break;
            }
        }
    }
}
