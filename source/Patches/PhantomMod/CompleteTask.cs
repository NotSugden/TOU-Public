using HarmonyLib;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.PhantomMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            var role = Role.GetRole<Phantom>(__instance);
            var tasks = __instance.Data.Tasks.ToArray();
            if (tasks.Count(x => !x.Complete) != 0) return;
            var writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.PhantomWin,
                Hazel.SendOption.Reliable,
                -1
            );
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.EndGame();
        }
    }
}
