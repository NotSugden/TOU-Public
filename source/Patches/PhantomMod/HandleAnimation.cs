using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.PhantomMod
{
    public class PhysicsPatch
    {
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
        public class HandleAnimation
        {
            public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
            {
                var player = __instance.myPlayer;
                if (!player.Is(RoleEnum.Phantom)) return;
                amDead = Role.GetRole<Phantom>(player).Caught;
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
        public class ResetMoveState
        {
            // Token: 0x06000321 RID: 801 RVA: 0x0001011C File Offset: 0x0000E31C
            public static void Postfix(PlayerPhysics __instance)
            {
                var player = __instance.myPlayer;
                if (!player.Is(RoleEnum.Phantom)) return;
                var role = Role.GetRole<Phantom>(player);
                player.Collider.enabled = !role.Caught;
            }
        }
    }
}
