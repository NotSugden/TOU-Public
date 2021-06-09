using System;
using System.Linq;
using System.Collections.Generic;
using Hazel;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {

        public PlayerControl ClosestPlayer { get; set; }
        public DateTime LastKilled { get; set; }
        public List<byte> Kills = new List<byte>();

        public Sheriff(PlayerControl player) : base(player)
        {
            Name = "Sheriff";
            ImpostorText = () => "Shoot the <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Kill off the impostor but don't kill crewmates.";
            RoleType = RoleEnum.Sheriff;
        }

        public bool CanKill(PlayerControl player)
        {
            if (player?.Data == null) return false;
            return
                player.Data.IsImpostor ||
                player.Is(RoleEnum.Glitch) ||
                (CustomGameOptions.SheriffKillsArsonist && player.Is(RoleEnum.Arsonist)) ||
                (CustomGameOptions.SheriffKillsJester && player.Is(RoleEnum.Jester));
        }

        internal override bool CheckEndCriteria(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.SheriffKeepsGameAliveOn2) return true;

            var players = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !player.Data.IsDead && !player.Data.Disconnected
            ).ToList();
            if (players.Count() == 2)
            {
                var otherPlayer = players.Find(player => !player.AmOwner);
                if (otherPlayer.Data.IsImpostor || otherPlayer.Is(RoleEnum.Glitch)) return false;
            }

            if (
                CustomGameOptions.SheriffKeepsGameAlive &&
                players.Any(player => player.Data.IsImpostor || player.Is(RoleEnum.Glitch))
            )
            {
                return false;
            }

            return true;
        }

        public float SheriffKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = PlayerControl.GameOptions.KillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override bool Criteria()
        {
            return CustomGameOptions.ShowSheriff || base.Criteria();
        }



    }
}
