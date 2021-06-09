using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.PhantomMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByTask && reason != GameOverReason.HumansByVote) return true;
            foreach (Role role in Role.AllRoles)
            {
                if (role.RoleType == RoleEnum.Phantom)
                {
                    ((Phantom)role).Loses();
                }
            }
            MessageWriter msg = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.PhantomLose,
                SendOption.Reliable,
                -1
            );
            AmongUsClient.Instance.FinishRpcImmediately(msg);
            return true;
        }
    }
}
