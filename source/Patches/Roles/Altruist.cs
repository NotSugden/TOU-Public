using UnityEngine;

namespace TownOfUs.Roles
{
    public class Altruist : Role
    {
        public bool ReviveUsed;
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;
        
        public Altruist(PlayerControl player) : base(player)
        {
            Name = "Altruist";
            ImpostorText = () => "Sacrifice yourself to save another";
            TaskText = () => "Revive a dead body at the cost of your own life.";
            RoleType = RoleEnum.Altruist;
        }
    }
}