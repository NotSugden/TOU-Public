using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Assassin : Role
    {
        public readonly List<GameObject> GameObjects = new List<GameObject>();
        public int timesKilled = 0;

        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            ImpostorText = () => "Kill during meetings";
            TaskText = () => "Sneakily kill the crewmates by guessing their roles";
            RoleType = RoleEnum.Assassin;
            Faction = Faction.Impostors;
        }

    }
}