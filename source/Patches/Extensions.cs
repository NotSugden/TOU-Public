using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using System.Linq;

namespace TownOfUs
{
    public static class PlayerExtensions
    {
        public static bool IsShielded(this PlayerControl player) => player.GetMedic() != null;

        public static Medic GetMedic(this PlayerControl player)
        {
            var medic = Role.GetRole<Medic>();
            return medic == null || medic.ShieldedPlayer.PlayerId == player.PlayerId
                ? medic
                : null;
        }

        public static bool isLover(this PlayerControl player) =>
            player.IsAny(new RoleEnum[] { RoleEnum.LoverImpostor, RoleEnum.Lover });

        public static bool Is(this PlayerControl player, RoleEnum roleType) =>
            Role.GetRole(player)?.RoleType == roleType;

        public static bool Is(this PlayerControl player, ModifierEnum modifierType) =>
            Modifier.GetModifier(player)?.ModifierType == modifierType;

        public static bool Is(this PlayerControl player, Faction faction) =>
            Role.GetRole(player)?.Faction == faction;

        public static bool IsAny(this PlayerControl player, RoleEnum[] roleTypes)
        {
            var role = Role.GetRole(player)?.RoleType ?? RoleEnum.None;
            if (role == RoleEnum.None) return false;
            return roleTypes.Any(type => role == type);
        }
    }
}
