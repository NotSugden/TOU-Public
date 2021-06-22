using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using System.Collections.Generic;

namespace TownOfUs.SoloImpostorMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (!CustomGameOptions.AnonImpostors || reason != GameOverReason.ImpostorByKill) return true;
            var loses = new List<byte>();
            foreach (Role role in Role.AllRoles)
            {
                if (role is Impostor imp && !imp.ImpWins)
                {
                    imp.Loses();
                    loses.Add(imp.Player.PlayerId);
                }
            }
            MessageWriter msg = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SoloImpLose,
                SendOption.Reliable,
                -1
            );
            msg.WriteBytesAndSize(loses.ToArray());
            AmongUsClient.Instance.FinishRpcImmediately(msg);
            return true;
        }
    }
}
