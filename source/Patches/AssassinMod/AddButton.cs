using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;
using UnhollowerBaseLib;
namespace TownOfUs.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        public static Sprite CycleSprite => TownOfUs.CycleSprite;
        public static Sprite GuessSprite => TownOfUs.GuessSprite;

        private static List<int> guesses = new List<int>();

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
        public static void GenButton(Assassin role, int index, bool isDead)
        {
            if (isDead)
            {
                return;
            }

            var voteArea = MeetingHud.Instance.playerStates[index];

            var nameText = voteArea.NameText;
            var roleGuessText = Object.Instantiate(nameText, voteArea.transform);
            roleGuessText.transform.position = nameText.transform.position - new Vector3(0f, 0.25f, 0f);
            roleGuessText.text = "None";

            var cycleButton = makeButton(voteArea, new Vector3(0.2f, -0.15f, 0f), CycleSprite, CycleGuess(roleGuessText, index));
            var guessButton = makeButton(voteArea, new Vector3(0.2f, 0.15f, 0f), GuessSprite, Guess(voteArea, role, index));
            role.GameObjects.Add(cycleButton);
            role.GameObjects.Add(guessButton);
            role.GameObjects.Add(roleGuessText.gameObject);
        }

        private static List<RoleEnum> GetEnabledRoles()
        {
            var enabledRoles = CustomGameOptions.GetEnabledRoles();
            if (CustomGameOptions.AssassinCanGuessCrewmate) enabledRoles.Insert(0, RoleEnum.Crewmate);
            return enabledRoles;
        }

        private static Action Guess(PlayerVoteArea voteArea, Assassin assassin, int index)
        {
            void Listener()
            {
                if (MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion) return;
                var guessIndex = guesses.ElementAt(index);
                if (guessIndex == -1) return;

                var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(
                    p => p.PlayerId == voteArea.TargetPlayerId
                );
                var role = Role.GetRole(player);
                var enabledRoles = GetEnabledRoles();
                var guess = enabledRoles[guessIndex];
                if (role.RoleType != guess)
                {
                    player = PlayerControl.LocalPlayer;
                    voteArea = MeetingHud.Instance.playerStates.FirstOrDefault(
                        v => v.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId
                    );
                }
                Utils.RpcKillDuringMeeting(voteArea, player);
                assassin.timesKilled++;
                ShowHideButtons.Confirm.Prefix(MeetingHud.Instance);
            }

            return Listener;
        }

        private static Action CycleGuess(TMPro.TextMeshPro guessText, int index)
        {
            void Listener()
            {
                var enabledRoles = GetEnabledRoles();
                var newGuess = guesses.ElementAt(index) + 1;
                if (newGuess >= enabledRoles.Count) newGuess = 0;
                guesses[index] = newGuess;
                var currentGuess = enabledRoles[newGuess];
                var name = Role.GetRoleName(currentGuess);
                var color = Role.GetRoleColor(currentGuess);
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
                var swapper = (Assassin)role;
                swapper.GameObjects.Clear();
                guesses.Clear();
            }


            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return;
            var assassinrole = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            if (assassinrole.timesKilled == CustomGameOptions.AssassinMaxKills) return;
            for (var i = 0;i < __instance.playerStates.Length;i++)
            {
                var playerState = __instance.playerStates[i];
                guesses.Add(-1);
                if (playerState.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId)
                {
                    var role = Role.GetRole(playerState);
                    if (role != null && role.Faction != Faction.Impostors)
                        GenButton(assassinrole, i, playerState.AmDead);
                }
            }
        }
    }
}
