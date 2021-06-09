using System;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using TownOfUs.Roles;
using TMPro;

namespace TownOfUs.PhantomMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            var role = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Phantom && ((Phantom)x).CompletedTasks);
            if (role == null) return;
            var array = UnityEngine.Object.FindObjectsOfType<PoolablePlayer>();
            array[0].NameText.text = role.ColorString + array[0].NameText.text + "</color>";
            __instance.BackgroundBar.material.color = role.Color;
            var textMeshPro = UnityEngine.Object.Instantiate(__instance.WinText);
            textMeshPro.text = "Phantom wins";
            textMeshPro.color = role.Color;
            var localPosition = __instance.WinText.transform.localPosition;
            localPosition.y = 1.5f;
            textMeshPro.transform.position = localPosition;
            textMeshPro.text = $"<size=4>{textMeshPro.text}</size>";
        }
    }
}
