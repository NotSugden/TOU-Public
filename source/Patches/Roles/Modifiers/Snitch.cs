﻿using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Snitch : Modifier
    {
        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public List<ArrowBehaviour> SnitchArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> SnitchTargets = new List<PlayerControl>();

        public int TasksLeft = int.MaxValue;

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            TaskText = () =>
                TasksDone
                    ? "Find the arrows pointing to the Impostors!"
                    : "Complete all your tasks to discover the Impostors!";
            Hidden = !CustomGameOptions.SnitchOnLaunch;
            ModifierType = ModifierEnum.Snitch;
            Color = new Color(0.83f, 0.69f, 0.22f, 1f);
        }

        public override bool Criteria()
        {
            var canSee = OneTaskLeft || TasksDone;

            var localFaction = Role.GetRole(PlayerControl.LocalPlayer)?.Faction;
            if (
                canSee && (localFaction == Faction.Impostors ||
                (CustomGameOptions.SnitchSeesNeutrals && localFaction == Faction.Neutral))
            ) return true;

            return (Player.AmOwner && (!Hidden || canSee || Player.Data.IsDead)) || (
                CustomGameOptions.DeadSeeRoles &&
                Utils.ShowDeadBodies &&
                PlayerControl.LocalPlayer.Data.IsDead
            );
        }

        public bool OneTaskLeft => TasksLeft <= 1;
        public bool TasksDone => TasksLeft <= 0;
    }
}
