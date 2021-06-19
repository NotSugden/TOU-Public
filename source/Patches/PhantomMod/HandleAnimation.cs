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
