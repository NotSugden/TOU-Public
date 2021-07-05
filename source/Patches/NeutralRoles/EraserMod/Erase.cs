using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.EraserMod
{
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class Erase
    {
        public static void Postfix()
        {
            var eraser = Role.GetRole<Eraser>();
            if (eraser == null) return;
            foreach (var playerId in eraser.ToErase)
            {
                ErasePlayer(Utils.PlayerById(playerId), eraser);
            }
            eraser.ToErase.Clear();
            Role.NamePatch.UpdateAll();
        }

        public static void ErasePlayer(PlayerControl player, Eraser eraser)
        {
            if (player == null) return;
            if (player.AmOwner)
            {
                AbilityManager.DestroyAll();
            }

            var role = Role.GetRole(player);

            role?.RemoveCustomTasks();

            eraser.ErasedPlayers.Add(player.PlayerId);

            Role.RoleDictionary.Remove(player.PlayerId);

            var newRole = (
                role != null ? role.Faction == Faction.Impostors : player.Data.IsImpostor
            ) ? (Role)new Impostor(player) : new Crewmate(player);

            newRole.RegenTask();
        }
    }
}
