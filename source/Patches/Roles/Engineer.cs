using UnityEngine;

namespace TownOfUs.Roles
{
    public class Engineer : Role
    {

        public int Uses = 0;
        public bool CanUse => Uses < CustomGameOptions.EngineerFixes;

        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            ImpostorText = () => "Maintain important systems on the ship";
            TaskText = () => "Vent and fix a sabotage from anywhere!";
            RoleType = RoleEnum.Engineer;

        }
    }
}
