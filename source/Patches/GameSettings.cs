
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using TownOfUs.CustomOption;
using UnityEngine;
using Reactor.Extensions;

namespace TownOfUs
{

    [HarmonyPatch]
    public static class GameSettings
    {
        [HarmonyPatch]
        private static class GameOptionsDataPatch
        {
            public static IEnumerable<MethodBase> TargetMethods() =>
                typeof(GameOptionsData).GetMethods(typeof(string), typeof(int));
            public static void Postfix(ref string __result)
            {
                StringBuilder builder = new StringBuilder(__result);

                foreach (CustomOption.CustomOption option in CustomOption.CustomOption.AllOptions)
                {
                    if (option.Type == CustomOptionType.Button) continue;
                    if (option.Type == CustomOptionType.Header) builder.AppendLine($"\n{option.Name}");
                    else if (option.Indent) builder.AppendLine($"     {option.Name}: {option}");
                    else builder.AppendLine($"{option.Name}: {option}");
                }


                __result = builder.ToString();



                if (CustomOption.CustomOption.LobbyTextScroller && __result.Count(c => c == '\n') > 38)
                    __result = __result.Insert(__result.IndexOf('\n'), " (Scroll for more)");
                else __result = __result.Insert(__result.IndexOf('\n'), "Press Tab to see All Options");


                __result = $"<size=1.25>{__result}</size>";

            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class Update
        {
            public static void Postfix(ref GameOptionsMenu __instance)
            {
                __instance.GetComponentInParent<Scroller>().YBounds.max = 70f;
            }
        }
    }
}
