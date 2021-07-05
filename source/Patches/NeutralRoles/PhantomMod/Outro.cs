﻿using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.PhantomMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x =>
                x.RoleType == RoleEnum.Phantom && ((Phantom) x).CompletedTasks);
            if (role == null) return;
            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            array[0].NameText.text = Utils.ColorText(role.Color, array[0].NameText.text);
            __instance.BackgroundBar.material.color = role.Color;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "Phantom wins";
            text.color = role.Color;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}
