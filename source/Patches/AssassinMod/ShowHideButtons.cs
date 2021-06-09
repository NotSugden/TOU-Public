using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Hazel;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using Reactor.Extensions;

namespace TownOfUs.AssassinMod
{
    public class ShowHideButtons
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Assassin)) return true;

                var assassin = Roles.Role.GetRole<Roles.Assassin>(PlayerControl.LocalPlayer);
                foreach (var button in assassin.GameObjects)
                {
                    if (button == null) continue;
                    button.SetActive(false);

                    var comp = button.GetComponent<PassiveButton>();
                    if (comp == null)
                    {
                        continue;
                    }
                    comp.OnClick = new Button.ButtonClickedEvent();
                }

                return true;

            }
        }
    }
}
