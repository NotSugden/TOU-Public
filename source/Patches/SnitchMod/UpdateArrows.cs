using System.Linq;
using HarmonyLib;
using Reactor.Extensions;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.SnitchMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var modifier in Modifier.AllModifiers.Where(x => x.ModifierType == ModifierEnum.Snitch))
            {
                var snitch = (Snitch) modifier;
                if (PlayerControl.LocalPlayer.Data.IsDead || snitch.Player.Data.IsDead)
                {
                    snitch.SnitchArrows.DestroyAll();
                    snitch.SnitchArrows.Clear();
                    snitch.ImpArrows.DestroyAll();
                    snitch.ImpArrows.Clear();
                }
                
                foreach (var arrow in snitch.ImpArrows)
                {
                    arrow.target = snitch.Player.transform.position;
                }

                foreach (var (arrow, target) in Utils.Zip(snitch.SnitchArrows, snitch.SnitchTargets))
                {
                    if(target.Data.IsDead)
                    {
                        arrow.Destroy();
                        if (arrow.gameObject != null) arrow.gameObject.Destroy();
                    }
                    arrow.target = target.transform.position;
                }
            }
        }
    }
}
