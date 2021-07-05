using System.Linq;

namespace TownOfUs.Roles
{
    public class Sheriff : Role
    {
        public Sheriff(PlayerControl player) : base(player)
        {
            ImpostorText = () => "Shoot the <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Kill off the impostor but don't kill crewmates.";
            RoleType = RoleEnum.Sheriff;
            CreateButtons();
        }

        public override void CreateButtons()
        {
            if (Player.AmOwner)
            {
                var killButton = HudManager.Instance.KillButton;
                AbilityManager.Add(new PlayerAbilityData
                {
                    Callback = KillCallback,
                    KillButton = killButton,
                    MaxTimer = PlayerControl.GameOptions.KillCooldown,
                    Range = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance],
                    TargetColor = Color,
                    Position = AbilityPositions.KillButton
                });
            }
        }

        public override bool CheckEndCriteria(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected || !CustomGameOptions.SheriffKeepsGameAlive) return true;

            var players = PlayerControl.AllPlayerControls.ToArray().Where(
                player => !player.Data.IsDead && !player.Data.Disconnected
            ).ToList();

            if (
                CustomGameOptions.SheriffKeepsGameAlive &&
                players.Any(player => player.Data.IsImpostor || player.Is(RoleEnum.Glitch))
            )
            {
                return false;
            }

            return true;
        }

        public bool CanKill(PlayerControl player)
        {
            if (player.Data.IsImpostor) return true;

            var role = GetRole(player)?.RoleType;

            return
                role != null && (role == RoleEnum.Glitch ||
                (CustomGameOptions.SheriffKillsJester && role == RoleEnum.Jester) ||
                (CustomGameOptions.SheriffKillsArsonist && role == RoleEnum.Arsonist));
        }

        public void KillCallback(PlayerControl player)
        {
            var canKill = CanKill(player);

            if (player.IsShielded())
            {
                Utils.RpcBreakShield(player);
                return;
            }

            if (canKill || CustomGameOptions.SheriffKillOther)
                Utils.RpcMurderPlayer(Player, player);

            if (!canKill)
                Utils.RpcMurderPlayer(Player, Player);
        }

        public override bool Criteria()
        {
            return Player.AmOwner || CustomGameOptions.ShowSheriff || base.Criteria();
        }
    }
}
