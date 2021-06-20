using System.Linq;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Roles;

namespace TownOfUs
{
    /*[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
    public static class EndGamePatch
    {
        public static void Postfix()
        {
            Utils.potentialWinners.Clear();
            foreach (var player in PlayerControl.AllPlayerControls)
                Utils.potentialWinners.Add(new WinningPlayerData(player.Data));
        }
    }*/

    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class EndGameManagerPatches
    {
        public static void Prefix()
        {
            if (Role.NobodyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                return;
            }

            void SetWinners(PlayerControl[] players = null)
            {
                var winners = new List<WinningPlayerData>();
                if (players != null)
                {
                    foreach (var player in players)
                    {
                        var data = new WinningPlayerData(player.Data) { IsDead = false };
                        winners.Add(data);
                    }
                }
                TempData.winners = winners;
            }

            var dictionary = new Dictionary<RoleEnum, Role>();
            foreach (var role in Role.AllRoles)
            {
                dictionary[role.RoleType] = role;
            }

            dictionary.TryGetValue(RoleEnum.Jester, out var jester);

            if (jester != null && ((Jester) jester).VotedOut)
            {
                SetWinners(new[] { jester.Player });
                return;
            }

            dictionary.TryGetValue(RoleEnum.Executioner, out var executioner);

            if (executioner != null && ((Executioner) executioner).TargetVotedOut)
            {
                SetWinners(new[] { executioner.Player });
                return;
            }

            dictionary.TryGetValue(RoleEnum.Lover, out var lover);

            if (lover != null && ((Lover) lover).LoveCoupleWins)
            {
                SetWinners(new[]
                {
                    lover.Player,
                    ((Lover) lover).OtherLover.Player
                });
                return;
            }

            dictionary.TryGetValue(RoleEnum.Child, out var child);

            if (child != null && ((Child) child).Dead)
            {
                SetWinners();
                return;
            }

            dictionary.TryGetValue(RoleEnum.Glitch, out var glitch);

            if (glitch != null && ((Glitch) glitch).GlitchWins)
            {
                SetWinners(new[] { glitch.Player });
                return;
            }

            dictionary.TryGetValue(RoleEnum.Arsonist, out var arsonist);

            if (arsonist != null && ((Arsonist) arsonist).ArsonistWins)
            {
                SetWinners(new[] { arsonist.Player });
            }
        }
    }
}
