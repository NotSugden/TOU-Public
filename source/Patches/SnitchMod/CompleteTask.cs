using System.Linq;
using HarmonyLib;
using UnityEngine;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {

        public static Sprite ArrowSprite => TownOfUs.Arrow;
		
		public static bool CanSee(PlayerControl player) =>
			player.Data.IsImpostor || (CustomGameOptions.SnitchCanSeeNeutrals && player.Is(RoleEnum.Glitch));

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ModifierEnum.Snitch)) return;
            if (__instance.Data.IsDead) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            var modifier = Modifier.GetModifier<Snitch>(__instance);
            modifier.TasksLeft = tasksLeft;
            var localIsSnitch = PlayerControl.LocalPlayer.Is(ModifierEnum.Snitch);
            var role = Roles.Role.GetRole(__instance);
            if (localIsSnitch) role?.RegenTask();
            switch (tasksLeft)
            {
                case 1:
                    if (localIsSnitch)
                    {
                        Reactor.Coroutines.Start(Utils.FlashCoroutine(modifier.Color));
                    }
                    else if (CanSee(PlayerControl.LocalPlayer))
                    {
                        Reactor.Coroutines.Start(Utils.FlashCoroutine(modifier.Color));
                        modifier.AddImpArrow();
                    }

                    break;

                case 0:
                    if (localIsSnitch)
                    {
                        Reactor.Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        var impostors = PlayerControl.AllPlayerControls.ToArray().Where(CanSee);
                        foreach (var imp in impostors)
                        {
                            modifier.AddSnitchArrow(imp);
                        }
                    }
                    break;
            }
        }
    }
}
