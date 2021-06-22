using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TownOfUs.AssassinMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public class ShowHideButtons
    {
        public static bool Prefix(MeetingHud __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return true;

            Hide();

            return true;
        }

        private static void HideButton(GameObject cycle, GameObject guess, TextMeshPro text)
        {
            cycle.SetActive(false);
            guess.SetActive(false);
            text.gameObject.SetActive(false);

            cycle.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
            guess.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        }

        public static void Hide(PlayerControl player = null)
        {
            var assassin = Roles.Role.GetRole<Roles.Assassin>(PlayerControl.LocalPlayer);

            if (player != null && assassin.KilledThisMeeting < CustomGameOptions.AssassinMaxPerMeeting)
            {
                var (cycle, guess, text) = assassin.Buttons[player.PlayerId];
                HideButton(cycle, guess, text);
                return;
            }

            foreach (var (cycle, guess, text) in assassin.Buttons.Values)
            {
                HideButton(cycle, guess, text);
            }
        }
    }
}
