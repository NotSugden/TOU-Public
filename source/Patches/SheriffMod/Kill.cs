using System;
using HarmonyLib;
using Hazel;
using Il2CppSystem.Reflection;

namespace TownOfUs.SheriffMod
{
    
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    public static class Kill
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButtonManager __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff);
            if (!flag) return true;
            var role = Roles.Role.GetRole<Roles.Sheriff>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var flag2 = role.SheriffKillTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var target = role.ClosestPlayer;
            var distBetweenPlayers = Utils.getDistBetweenPlayers(PlayerControl.LocalPlayer, target);
            var flag3 = distBetweenPlayers < (double)GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            if (!flag3) return false;
            if(target.isShielded()) {
                var medic = target.getMedic().Player.PlayerId;
                var writer1 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.AttemptSound, Hazel.SendOption.Reliable, -1);
                    writer1.Write(medic);
                    writer1.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer1);

                    if (CustomGameOptions.ShieldBreaks)
                    {
                        role.LastKilled = DateTime.UtcNow;
                    }
                    
                    MedicMod.StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);

                    return false;
            }

            var flag4 = role.CanKill(target);
            if (!flag4)
            {
                if (CustomGameOptions.SheriffKillOther)
                {
                    Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                }
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
            }
            else
            {
                Utils.RpcMurderPlayer(PlayerControl.LocalPlayer, target);
                role.Kills.Add(target.PlayerId);
            }
            role.LastKilled = DateTime.UtcNow;

            return false;
        }
    }
}
