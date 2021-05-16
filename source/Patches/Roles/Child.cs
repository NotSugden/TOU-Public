using Hazel;

namespace TownOfUs.Roles
{
    public class Child : Role
    {
        public bool Dead = false;
        
        public Child(PlayerControl player) : base(player)
        {
            Name = "Child";
            ImpostorText = () => "No one will harm you";
            TaskText = () => "You won't be harmed";
            RoleType = RoleEnum.Child;
        }

        public void Wins()
        {
            Dead = true;

        }

        internal override bool CheckEndCriteria(ShipStatus __instance)
        {
            //System.Console.WriteLine("REACHES HERE2.75");
            if (!Dead) return true;
            //System.Console.WriteLine("REACHES HERE3");
            if (!Player.Data.IsDead) return false;
            //System.Console.WriteLine("REACHES HERE4");
            Utils.EndGame();
            return false;

        }
    }
}