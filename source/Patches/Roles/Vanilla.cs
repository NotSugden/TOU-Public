using UnityEngine;
using System.Linq;
using Hazel;

namespace TownOfUs.Roles
{
    public class Impostor : Role
    {
        public bool ImpWins { get; set; }
        public new bool Hidden => RoleType == RoleEnum.Impostor;
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            Faction = Faction.Impostors;
            RoleType = RoleEnum.Impostor;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = false;
        }

        public void Wins()
        {
            ImpWins = true;
        }

        public override bool Criteria()
        {
            if (base.Criteria()) return true;

            if (CustomGameOptions.AnonImpostors) return false;

            return PlayerControl.LocalPlayer.Data.IsImpostor && CustomGameOptions.ImpostorSeeRoles;
        }

        internal override bool CheckEndCriteria(ShipStatus __instance)
        {
            if (!CustomGameOptions.AnonImpostors) return true;
            var player = Player;
            if (player.Data.IsDead || player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) == 1)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SoloImpWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame(GameOverReason.ImpostorByKill);
                return false;
            }

            return false;
        }

        protected override void IntroPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
        {
            if (!CustomGameOptions.AnonImpostors) return;
            var impostorTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            impostorTeam.Add(PlayerControl.LocalPlayer);
            yourTeam = impostorTeam;
        }
    }

    public class Crewmate : Role
    {

        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            Hidden = true;
            Faction = Faction.Crewmates;
            RoleType = RoleEnum.Crewmate;
        }
    }
}
