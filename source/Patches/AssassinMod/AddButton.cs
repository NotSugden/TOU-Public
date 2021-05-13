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

        private static List<RoleEnum> guesses = new List<RoleEnum>();

        private static GameObject makeButton(PlayerVoteArea voteArea, Vector3 diffPos, Sprite sprite, Action onClick)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var button = Object.Instantiate(confirmButton, voteArea.transform);

            var renderer = button.GetComponent<SpriteRenderer>();
            var passive = button.GetComponent<PassiveButton>();

            renderer.sprite = sprite;
            var pos = button.transform.position = confirmButton.transform.position - diffPos;

            // button.transform.localScale *= 0.2f;
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

            var cycleButton = makeButton(voteArea, new Vector3(0.4f, -0.15f, 0f), CycleSprite, CycleGuess(roleGuessText, index));
            var guessButton = makeButton(voteArea, new Vector3(0.4f, 0.15f, 0f), GuessSprite, Guess(voteArea, role, index));
            role.GameObjects.Add(cycleButton);
            role.GameObjects.Add(guessButton);
            role.GameObjects.Add(roleGuessText.gameObject);
        }

        private static Action Guess(PlayerVoteArea voteArea, Assassin assassin, int index)
        {
            void Listener()
            {
                var guess = guesses.ElementAt(index);
                if (guess == RoleEnum.None) return;

                var player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(
                    p => p.PlayerId == voteArea.TargetPlayerId
                );
                var role = Role.GetRole(player);
                if (role.RoleType != guess)
                {
                    player = PlayerControl.LocalPlayer;
                    voteArea = MeetingHud.Instance.playerStates.FirstOrDefault(
                        v => v.TargetPlayerId == PlayerControl.LocalPlayer.PlayerId
                    );
                }
                Utils.RpcMurderPlayer(player, player, false);
                Utils.RpcSetDeadInMeeting(player);
                assassin.timesKilled++;
                ShowHideButtons.Confirm.Prefix(MeetingHud.Instance);
            }

            return Listener;
        }

        private static Action CycleGuess(TMPro.TextMeshPro guessText, int index)
        {
            void Listener()
            {
                var currentGuess = guesses.ElementAt(index);
                switch (currentGuess)
                {
                    case RoleEnum.Engineer:
                        currentGuess = RoleEnum.Lover;
                        break;
                    case RoleEnum.Altruist:
                        currentGuess = RoleEnum.Glitch;
                        break;
                    case RoleEnum.Glitch:
                    case RoleEnum.None:
                        currentGuess = RoleEnum.Sheriff;
                        break;
                    default:
                        currentGuess++;
                        break;
                }
                guesses[index] = currentGuess;
                var name = Role.getRoleName(currentGuess);
                guessText.text = name;
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            
            foreach(var role in Role.GetRoles(RoleEnum.Assassin))
            {
                var swapper = (Assassin) role;
                swapper.GameObjects.Clear();
                guesses.Clear();
            }


            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return;
            var assassinrole = Role.GetRole<Assassin>(PlayerControl.LocalPlayer);
            if (assassinrole.timesKilled == CustomGameOptions.AssassinMaxKills) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerState = __instance.playerStates[i];
                guesses.Add(RoleEnum.None);
                if (playerState.TargetPlayerId != PlayerControl.LocalPlayer.PlayerId)
                    GenButton(assassinrole, i, playerState.isDead);
            }
        }
    }
}