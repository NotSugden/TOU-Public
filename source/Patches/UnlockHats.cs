using System.Linq;
using HarmonyLib;
using UnhollowerBaseLib;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetUnlockedHats))]
    public static class UnlockHats
    {
        public static bool Prefix(HatManager __instance, ref Il2CppReferenceArray<HatBehaviour> __result)
        {
            var array = __instance.AllHats.ToArray().Where(
                hat => !HatManager.IsMapStuff(hat.ProdId) || SaveManager.GetPurchase(hat.ProdId)
            ).ToArray();
            __result = array;
            return false;
        }
    }
}
