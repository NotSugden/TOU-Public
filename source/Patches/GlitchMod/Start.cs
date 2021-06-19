using System;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.GlitchMod
{
    [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
    class Start
    {
        static void Postfix(IntroCutscene __instance)
        {
            var _role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
            var glitch = _role != null ? (Glitch)_role : null;
            if (glitch != null)
            {
                glitch.MimicTimer = CustomGameOptions.MimicCooldown;
                glitch.HackTimer = CustomGameOptions.HackCooldown;
                glitch.KillTimer = 10f;
            }
        }
    }
}
