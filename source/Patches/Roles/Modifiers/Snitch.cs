using System;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Snitch : Modifier
    {

        public int TasksLeft = Int32.MaxValue;
        public bool OneTaskLeft => TasksLeft <= 1;
        public bool TasksDone => TasksLeft <= 0;

        public List<PlayerControl> SnitchTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> SnitchArrows = new List<ArrowBehaviour>();

        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public Snitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            Color = new Color(0.83f, 0.69f, 0.22f, 1f);
            ModifierType = ModifierEnum.Snitch;
            TaskText = () =>
            {
                if (TasksDone) return "Find the arrows pointing to the Impostors!";
                else if (OneTaskLeft) return "Complete your last task to find the Impostors!";
                else if (CustomGameOptions.SnitchOnLaunch) return "Complete all your tasks to discover the Impostors!";
                else return "";
            };
            Hidden = !CustomGameOptions.SnitchOnLaunch;
        }

        public void AddSnitchArrow(PlayerControl player)
        {
            AddArrow(SnitchArrows);
            SnitchTargets.Add(player);
        }
        public void AddImpArrow()
        {
            AddArrow(ImpArrows);
        }
        private void AddArrow(List<ArrowBehaviour> arrowList)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
            var renderer = gameObj.AddComponent<SpriteRenderer>();
            renderer.sprite = TownOfUs.Arrow;
            arrow.image = renderer;
            gameObj.layer = 5;
            arrowList.Add(arrow);
        }
    }
}
