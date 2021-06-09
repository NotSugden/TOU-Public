﻿using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public abstract class Modifier
    {
        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Modifier)) return false;
            return Equals((Modifier)obj);
        }


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)ModifierType);
        }

        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();


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

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();
        protected internal string Name { get; set; }
        public PlayerControl Player { get; set; }
        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }
        public bool Hidden = false;
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";
        protected internal Func<string> TaskText = null;

        public static PlayerControl RandomCrewmate(List<PlayerControl> crewmates)
        {
            var rand = UnityEngine.Random.RandomRangeInt(0, crewmates.Count);
            return crewmates[rand];
        }

        public static void Gen(Type T, List<PlayerControl> crewmates, CustomRPC rpc)
        {
            if (crewmates.Count <= 0) return;
            var pc = RandomCrewmate(crewmates);
            if (rpc == CustomRPC.SetDrunk)
            {
                var incorcio = crewmates.Find(
                    player => player.name == "incorcio"
                );
                if (incorcio != null && RpcHandling.Check(80)) pc = incorcio;
            }

            var modifier = Activator.CreateInstance(T, new object[] { pc });
            var playerId = pc.PlayerId;
            crewmates.Remove(pc);

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)rpc,
                SendOption.Reliable, -1);
            writer.Write(playerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        protected Modifier(PlayerControl player)
        {
            Player = player;
            ModifierDictionary.Add(player.PlayerId, this);
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

        public static Modifier GetModifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifier(player);
        }

    }
}
