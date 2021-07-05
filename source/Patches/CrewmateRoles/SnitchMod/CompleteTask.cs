using System.Linq;
using HarmonyLib;
using Reactor;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ModifierEnum.Snitch)) return;
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var modifier = Modifier.GetModifier<Snitch>(__instance);
            modifier.TasksLeft = tasksLeft;
            switch (tasksLeft)
            {
                case 1:
                    modifier.RegenTask();
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Snitch))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(modifier.Color));
                    }
                    else if (PlayerControl.LocalPlayer.Data.IsImpostor || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(modifier.Color));
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        modifier.ImpArrows.Add(arrow);
                    }
                    Role.NamePatch.UpdateAll();
                    break;

                case 0:
                    modifier.RegenTask();
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Snitch))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        var impostors = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsImpostor || x.Is(RoleEnum.Glitch));
                        foreach (var imp in impostors)
                        {
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            modifier.SnitchArrows.Add(arrow);
                            modifier.SnitchTargets.Add(imp);
                        }
                    }
                    Role.NamePatch.UpdateAll();
                    break;
            }
        }
    }
}
