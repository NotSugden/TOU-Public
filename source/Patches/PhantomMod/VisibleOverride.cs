using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        // Token: 0x06000329 RID: 809 RVA: 0x000104B8 File Offset: 0x0000E6B8
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            if (Role.GetRole<Phantom>(__instance).Caught) return;
            value = !__instance.inVent;
        }
    }
}
