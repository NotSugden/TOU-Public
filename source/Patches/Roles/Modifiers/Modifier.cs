using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();
        protected internal Func<string> TaskText;

        public bool Hidden = false;
        protected Modifier(PlayerControl player)
        {
            Player = player;
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();
        protected internal string Name { get; set; }
        private PlayerControl _Player;
        public PlayerControl Player
        {
            get => _Player;
            set
            {
                _Player = value;
                ModifierDictionary[value.PlayerId] = this;
            }
        }
        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }

        public virtual void CreateButtons()
        {
        }

        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Modifier)) return false;
            return Equals((Modifier) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int) ModifierType);
        }

        public void RegenTask() => Role.GetRole(Player)?.RegenTask();

        public virtual bool Criteria()
        {
            return (Player.AmOwner && (!Hidden || Player.Data.IsDead)) || (
                CustomGameOptions.DeadSeeRoles &&
                Utils.ShowDeadBodies &&
                PlayerControl.LocalPlayer.Data.IsDead
            );
        }


        public static bool operator ==(Modifier a, Modifier b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Modifier a, Modifier b)
        {
            return !(a == b);
        }

        public static Modifier GetModifier(PlayerControl player)
        {
            return (from entry in ModifierDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static T GetModifier<T>(PlayerControl player) where T : Modifier
        {
            return GetModifier(player) as T;
        }

        public static T GetModifier<T>() where T : Modifier
        {
            foreach (var modifier in AllModifiers)
            {
                if (modifier is T _modifier) return _modifier;
            }
            return null;
        }

        public static Modifier GetModifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifier(player);
        }
    }
}
