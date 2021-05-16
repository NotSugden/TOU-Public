using UnityEngine;

namespace TownOfUs.Roles
{
    public class Medic : Role
    {
        
        public PlayerControl ClosestPlayer { get; set; }
        public bool UsedAbility { get; set; } = false;
        public PlayerControl ShieldedPlayer { get; set; }
        public PlayerControl exShielded { get; set; }
        
        public Medic(PlayerControl player) : base(player)
        {
            Name = "Medic";
            ImpostorText = () => "Create a shield to protect a crewmate";
            TaskText = () => "Protect a crewmate with a shield";
            RoleType = RoleEnum.Medic;
            ShieldedPlayer = null;
        }
    }
}