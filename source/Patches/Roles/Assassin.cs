using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TownOfUs.Roles
{
    public class Assassin : Impostor
    {
        public int TimesKilled = 0;
        public int KilledThisMeeting = 0;
        public Dictionary<byte, (GameObject, GameObject, TextMeshPro)> Buttons =
            new Dictionary<byte, (GameObject, GameObject, TextMeshPro)>();
        public Dictionary<byte, int> Guesses = new Dictionary<byte, int>();

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
