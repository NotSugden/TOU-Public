using System;

namespace TownOfUs.Roles
{
    public class Undertaker : Impostor
    {
        public DateTime LastDragged { get; set; }

        public DeadBody CurrentTarget { get; set; }

        public DeadBody CurrentlyDragging { get; set; }

        public KillButtonManager DragDropButton
        {
            get => _dragDropButton;
            set
            {
                _dragDropButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public Undertaker(PlayerControl player) : base(player)
        {
            Name = "Undertaker";
            ImpostorText = () => "Drag bodies and hide them";
            TaskText = () => "Drag bodies around to hide them from being reported";
            RoleType = RoleEnum.Undertaker;
            Faction = Faction.Impostors;
        }

        public float DragTimer()
        {
            var timeSpan = DateTime.UtcNow - this.LastDragged;
            var cooldown = (CustomGameOptions.DragCooldown * 1000f) - (float)timeSpan.TotalMilliseconds;
            if (cooldown < 0f)
                return 0f;
            return cooldown / 1000f;
        }
        public KillButtonManager _dragDropButton;
    }
}
