using HarmonyLib;

namespace TownOfUs
{

    public class ShowRoleNamePatch
    {
        public static void Patch(bool force = false)
        {
            Utils.ShowDeadBodies = force || PlayerControl.LocalPlayer.Data.IsDead;
            Roles.Role.UpdateRoleNames();
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_Start
        {
            public static void Postfix(MeetingHud __instance) => Patch();
        }
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class MeetingHud_VotingComplete
        {
            public static void Postfix(MeetingHud __instance) => Patch();
        }
    }
}
