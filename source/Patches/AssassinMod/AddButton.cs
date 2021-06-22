using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

namespace TownOfUs.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static Sprite CycleSprite => TownOfUs.CycleSprite;
        public static Sprite GuessSprite => TownOfUs.GuessSprite;

        private static void Log(object message) => TownOfUs.LogMessage(message);

        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = Utils.PlayerById(voteArea.TargetPlayerId);
            if (IsExempt(player)) return true;
            var role = Role.GetRole(player);
            var flag = IsExempt(role);
            return flag;
        }
        private static bool IsExempt(PlayerControl player) {
            if (player == null) return true;
            var data = player.Data;
            return data.IsImpostor || data.IsDead || data.Disconnected;
        }
        private static bool IsExempt(Role role) => role != null && role.Criteria();

        private static GameObject makeButton(PlayerVoteArea voteArea, Vector3 diffPos, Sprite sprite, Action onClick)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var button = Object.Instantiate(confirmButton, voteArea.transform);

            var renderer = button.GetComponent<SpriteRenderer>();
            var passive = button.GetComponent<PassiveButton>();

            renderer.sprite = sprite;
            button.transform.position = confirmButton.transform.position - diffPos;

            button.layer = 5;
            button.transform.parent = voteArea.transform;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(onClick);

            passive.OnMouseOver = new UnityEngine.Events.UnityEvent();

            var hitbox = button.GetComponent<BoxCollider2D>();

            hitbox.size = new Vector2(0.6f, 0.3f);

            return button;
        }

        public static void GenButton(Assassin role, PlayerVoteArea voteArea)
        {
            if (IsExempt(voteArea)) return;

            var nameText = voteArea.NameText;
            var roleGuessText = Object.Instantiate(nameText, voteArea.transform);
            roleGuessText.transform.position = nameText.transform.position - new Vector3(0f, 0.25f, 0f);
            roleGuessText.text = "None";

            var cycleButton = makeButton(voteArea, new Vector3(0.2f, -0.15f, 0f), CycleSprite, Cycle(role, voteArea));
            var guessButton = makeButton(voteArea, new Vector3(0.2f, 0.15f, 0f), GuessSprite, Guess(role, voteArea));

            role.Buttons[voteArea.TargetPlayerId] = (
                cycleButton,
                guessButton,
                roleGuessText
            );
            role.Guesses[voteArea.TargetPlayerId] = -1;
        }

        private static List<RoleEnum> GetEnabledRoles()
        {
            var enabledRoles = CustomGameOptions.GetEnabledRoles();
            if (CustomGameOptions.AssassinCanGuessCrewmate) enabledRoles.Insert(0, RoleEnum.Crewmate);
            return enabledRoles;
        }

        private static Action Guess(Assassin assassin, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                var targetId = voteArea.TargetPlayerId;
                if (
                    MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                    !assassin.Guesses.ContainsKey(targetId)
                ) return;

                var player = Utils.PlayerById(targetId);
                if (IsExempt(player)) return;

                var role = Role.GetRole(player);
                if (IsExempt(role)) return;

                var enabledRoles = GetEnabledRoles();
                var guess = enabledRoles[assassin.Guesses[targetId]];
                if (role.RoleType != guess)
                {
                    player = PlayerControl.LocalPlayer;
                    voteArea = MeetingHud.Instance.playerStates.FirstOrDefault(
                        v => v.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId
                    );
                }
                Utils.RpcKillDuringMeeting(voteArea, player);
                assassin.TimesKilled++;
                assassin.KilledThisMeeting++;
                ShowHideButtons.Hide(player);
            }

            return Listener;
        }

        private static Action Cycle(Assassin assassin, PlayerVoteArea voteArea)
        {
            void Listener()
            {
                var targetId = voteArea.TargetPlayerId;
                if (
                    MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion ||
                    !assassin.Guesses.ContainsKey(targetId)
                ) return;

                var player = Utils.PlayerById(targetId);
                if (IsExempt(player)) return;

                var role = Role.GetRole(player);
                if (IsExempt(role)) return;

                var enabledRoles = GetEnabledRoles();
                var newGuess = assassin.Guesses[targetId] + 1;
                if (newGuess == enabledRoles.Count) newGuess = 0;
                assassin.Guesses[targetId] = newGuess;

                var currentGuess = enabledRoles[newGuess];
                var name = Role.GetRoleName(currentGuess);
                var color = Role.GetRoleColor(currentGuess);

                var guessText = assassin.Buttons[targetId].Item3;
                guessText.text = name;
                guessText.color = color;
            }

            return Listener;
        }

        [HarmonyPriority(Priority.Last)]
        public static void Postfix(MeetingHud __instance)
        {

            foreach (var role in Role.GetRoles(RoleEnum.Assassin))
            {
                var assassin = (Assassin)role;
                assassin.Buttons.Clear();
                assassin.Guesses.Clear();
                assassin.KilledThisMeeting = 0;
            }

            var localPlayer = PlayerControl.LocalPlayer;
            if (localPlayer.Data.IsDead || !localPlayer.Is(RoleEnum.Assassin)) return;
            var assassinRole = Role.GetRole<Assassin>(localPlayer);
            if (assassinRole.TimesKilled == CustomGameOptions.AssassinMaxKills) return;
            foreach (var voteArea in __instance.playerStates)
                if (voteArea.TargetPlayerId != localPlayer.PlayerId)
                    GenButton(assassinRole, voteArea);
        }
    }
}
