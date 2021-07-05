using Il2CppSystem.Collections.Generic;
using UnityEngine;
using System.Linq;
using Hazel;

namespace TownOfUs.Roles
{
    public class Eraser : Role
    {
        public PlayerAbilityData EraseButton;
        public bool ErasedAll = false;
        public List<byte> ErasedPlayers = new List<byte>();
        public List<byte> ToErase = new List<byte>();

        public Eraser(PlayerControl player) : base(player)
        {
            ImpostorText = () => "Erase Everyone";
            TaskText = () => "Erase everyones roles";
            RoleType = RoleEnum.Eraser;
            Faction = Faction.Neutral;
            CreateButtons();
        }

        public override void CreateButtons()
        {
            if (Player.AmOwner)
            {
                AbilityManager.Add(EraseButton = new PlayerAbilityData
                {
                    Callback = EraseCallback,
                    MaxTimer = CustomGameOptions.EraseCd,
                    Range = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance],
                    TargetColor = Color,
                    TargetFilter = player => !IsErased(player),
                    Icon = TownOfUs.EraseSprite,
                    Position = AbilityPositions.KillButton
                });
            }
        }

        public bool IsErased(PlayerControl player, bool erasedNow = false)
        {
            return (!erasedNow && ToErase.Contains(player.PlayerId)) || ErasedPlayers.Contains(player.PlayerId);
        }

        public void EraseCallback(PlayerControl target)
        {
            if (target.IsShielded())
            {
                Utils.RpcBreakShield(target);
                return;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.Erase, SendOption.Reliable, -1);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            ToErase.Add(target.PlayerId);

            var role = GetRole(target);

            if (role != null)
                EraseButton.MaxTimer += role.Faction == Faction.Crewmates
                    ? CustomGameOptions.ErasePenaltyCd
                    : -1f;
        }

        public override void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
            var eraserTeam = new List<PlayerControl>();
            eraserTeam.Add(PlayerControl.LocalPlayer);
            __instance.yourTeam = eraserTeam;
        }

        public override bool CheckEndCriteria(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var players = PlayerControl.AllPlayerControls.ToArray()
                .Where(player =>
                    !player.Data.IsDead &&
                    !player.Data.Disconnected &&
                    player.PlayerId != Player.PlayerId
                );
            if (players.All(player => ErasedPlayers.Contains(player.PlayerId)))
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.EraserWin, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Wins();
                Utils.EndGame();
                return false;
            }
            return true;
        }

        public void Wins()
        {
            ErasedAll = true;
        }

        public void Loses()
        {
            Player.Data.IsImpostor = true;
        }
    }
}
