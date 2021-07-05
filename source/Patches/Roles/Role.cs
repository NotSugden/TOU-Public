﻿using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Roles.Modifiers;
using TownOfUs.CrewmateRoles.MedicMod;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();

        public static bool NobodyWins;

        public List<KillButtonManager> ExtraButtons = new List<KillButtonManager>();

        public Func<string> ImpostorText;
        public Func<string> TaskText;

        protected Role(PlayerControl player)
        {
            Player = player;
        }

        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        private PlayerControl _Player { get; set; }

        public PlayerControl Player
        {
            get => _Player;
            set
            {
                _Player = value;
                PlayerName = value.Data.PlayerName;
                RoleDictionary[value.PlayerId] = this;
            }
        }
        protected float Scale { get; set; } = 1f;
        protected internal RoleEnum RoleType { get; set; }

        public string Name => GetName(RoleType, false);
        public Color Color => GetColor(RoleType);

        public virtual bool Hidden { get; set; } = false;

        public Faction Faction { get; set; } = Faction.Crewmates;

        public string PlayerName { get; set; }

        private bool Equals(Role other)
        {
            return Equals(Player, other.Player) && RoleType == other.RoleType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Role)) return false;
            return Equals((Role)obj);
        }

        public override int GetHashCode() => HashCode.Combine(Player, (int)RoleType);

        public virtual void CreateButtons()
        {
        }

        public virtual bool Criteria()
        {
            if (Player.AmOwner) return true;

            if (
                Faction == Faction.Impostors ||
                (CustomGameOptions.SnitchSeesNeutrals && Faction == Faction.Neutral))
            {
                var snitch = Modifier.GetModifier<Snitch>();
                if (snitch != null && snitch.TasksDone && snitch.Player.AmOwner)
                    return true;
            }

            var localData = PlayerControl.LocalPlayer.Data;
            var isDead = localData.IsDead && Utils.ShowDeadBodies;
            if (isDead && CustomGameOptions.DeadSeeRoles)
                return true;

            if (localData.IsImpostor && Player.Data.IsImpostor)
                return CustomGameOptions.ImpostorSeeRoles || isDead;

            return false;
        }

        public virtual void IntroPrefix(IntroCutscene._CoBegin_d__14 __instance)
        {
        }

        public static void NobodyWinsFunc()
        {
            NobodyWins = true;
        }

        public static bool NobodyEndCriteria(ShipStatus __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.Neutral && !x.Is(RoleEnum.Glitch) && !x.Is(RoleEnum.Arsonist);
                    var flag3 = x.Is(RoleEnum.Arsonist) && ((Arsonist)role).IgniteUsed && alives.Count > 1;

                    return flag2 || flag3;
                });

                return flag;
            }

            if (CheckNoImpsNoCrews())
            {
                var messageWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.NobodyWins, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

                NobodyWinsFunc();
                Utils.EndGame();
                return false;
            }

            return true;
        }

        public virtual bool CheckEndCriteria(ShipStatus __instance)
        {
            return true;
        }

        public static bool operator ==(Role a, Role b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.RoleType == b.RoleType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Role a, Role b)
        {
            return !(a == b);
        }

        private ImportantTextTask RoleTask;
        private ImportantTextTask ModifierTask;

        private void _RemoveTask(ImportantTextTask toRemove)
        {
            var idx = Player.myTasks.FindIndex(
                (Il2CppSystem.Predicate<PlayerTask>)(task => task == toRemove)
            );
            if (idx != -1)
                Player.myTasks.RemoveAt(idx);
        }

        public void RemoveCustomTasks()
        {
            _RemoveTask(RoleTask);
            _RemoveTask(ModifierTask);
        }

        public void RegenTask()
        {
            var modifier = Modifier.GetModifier(Player);
            var modifierText = modifier?.TaskText?.Invoke();

            RemoveCustomTasks();

            if (modifierText != null && !modifierText.Equals(""))
            {
                var task = ModifierTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = Utils.ColorText(
                    modifier.Color,
                    $"Modifier: {modifier.Name}\n{modifierText}"
                );
                Player.myTasks.Insert(0, task);
            }

            if (TaskText != null)
            {
                var task = RoleTask = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
                task.transform.SetParent(Player.transform, false);
                task.Text = Utils.ColorText(
                    Color,
                    $"Role: {Name}\n{TaskText()}"
                );
                Player.myTasks.Insert(0, task);
            }
        }

        public static T Gen<T>(Type type, PlayerControl player, CustomRPC rpc)
        {
            var role = (T)Activator.CreateInstance(type, new object[] { player });

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)rpc, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return role;
        }

        public static T Gen<T>(
            Type type,
            List<PlayerControl> players,
            CustomRPC rpc,
            bool targetVanilla = false)
        {
            PlayerControl player = null;
            if (targetVanilla)
            {
                var vanillaPlayers = players.Where(player => player.Is(RoleEnum.Crewmate));
                if (vanillaPlayers.Count() > 0)
                {
                    player = vanillaPlayers.Random();
                }
            }

            player ??= players.Random();

            var role = Gen<T>(type, player, rpc);
            players.Remove(player);
            return role;
        }
        
        public static Role GetRole(PlayerControl player)
        {
            if (player == null) return null;
            if (RoleDictionary.TryGetValue(player.PlayerId, out var role))
                return role;

            return null;
        }
        
        public static T GetRole<T>(PlayerControl player) where T : Role
        {
            return GetRole(player) as T;
        }

        public static Role GetRole(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetRole(player);
        }

        public static T GetRole<T>() where T : Role
        {
            foreach (var role in AllRoles)
            {
                if (role is T _role) return _role;
            }
            return null;
        }

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        public static Color GetColor(RoleEnum roleType) => roleType switch
        {
            RoleEnum.Sheriff => Color.yellow,
            RoleEnum.Jester => new Color(1f, 0.75f, 0.8f, 1f),
            RoleEnum.Engineer => new Color(1f, 0.65f, 0.04f, 1f),
            RoleEnum.LoverImpostor => new Color(1f, 0.4f, 0.8f, 1f),
            RoleEnum.Lover => new Color(1f, 0.4f, 0.8f, 1f),
            RoleEnum.Mayor => new Color(0.44f, 0.31f, 0.66f, 1f),
            RoleEnum.Swapper => new Color(0.4f, 0.9f, 0.4f, 1f),
            RoleEnum.Medic => new Color(0f, 0.4f, 0f, 1f),
            RoleEnum.Executioner => new Color(0.55f, 0.25f, 0.02f, 1f),
            RoleEnum.Arsonist => new Color(1f, 0.3f, 0f),
            RoleEnum.Altruist => new Color(0.4f, 0f, 0f, 1f),
            RoleEnum.Phantom => new Color(0.4f, 0.16f, 0.38f, 1f),
            RoleEnum.Eraser => new Color(1f, 0.2f, 0.8f),
            RoleEnum.Miner => Palette.ImpostorRed,
            RoleEnum.Swooper => Palette.ImpostorRed,
            RoleEnum.Morphling => Palette.ImpostorRed,
            RoleEnum.Janitor => Palette.ImpostorRed,
            RoleEnum.Undertaker => Palette.ImpostorRed,
            RoleEnum.Assassin => Palette.ImpostorRed,
            RoleEnum.Underdog => Palette.ImpostorRed,
            RoleEnum.Glitch => Color.green,
            RoleEnum.Impostor => Palette.ImpostorRed,
            _ => Color.white
        };

        public static string GetName(RoleEnum roleId, bool includeColor = false)
        {
            var roleName = roleId switch
            {
                RoleEnum.Sheriff => "Sheriff",
                RoleEnum.Jester => "Jester",
                RoleEnum.Engineer => "Engineer",
                RoleEnum.LoverImpostor => "Loving Impostor",
                RoleEnum.Lover => "Lover",
                RoleEnum.Mayor => "Mayor",
                RoleEnum.Swapper => "Swapper",
                RoleEnum.Medic => "Medic",
                RoleEnum.Executioner => "Executioner",
                RoleEnum.Arsonist => "Arsonist",
                RoleEnum.Altruist => "Altruist",
                RoleEnum.Phantom => "Phantom",
                RoleEnum.Eraser => "Eraser",
                RoleEnum.Miner => "Miner",
                RoleEnum.Swooper => "Swooper",
                RoleEnum.Morphling => "Morphling",
                RoleEnum.Janitor => "Janitor",
                RoleEnum.Undertaker => "Undertaker",
                RoleEnum.Assassin => "Assassin",
                RoleEnum.Underdog => "Underdog",
                RoleEnum.Glitch => "The Glitch",
                RoleEnum.Impostor => "Impostor",
                _ => "Crewmate",
            };

            return includeColor
                ? $"<color=#{GetColor(roleId).ToHtmlStringRGBA()}>{roleName}</color>"
                : roleName;
        }

        [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__14), nameof(IntroCutscene._CoBegin_d__14.MoveNext))]
        public class IntroCutscene_Patch
        {
            public static void Prefix(IntroCutscene._CoBegin_d__14 __instance)
            {
                GetRole(PlayerControl.LocalPlayer)?.IntroPrefix(__instance);
            }

            public static void Postfix(IntroCutscene._CoBegin_d__14 __instance)
            {
                var role = GetRole(PlayerControl.LocalPlayer);
                var cutscene = __instance.__4__this;
                var alpha = cutscene.Title.color.a;

                if (role != null && !role.Hidden)
                {
                    cutscene.Title.text = role.Name;
                    cutscene.Title.color = role.Color;
                    cutscene.ImpostorText.text = role.ImpostorText();
                    cutscene.ImpostorText.gameObject.SetActive(true);
                    cutscene.BackgroundBar.material.color = role.Color;
                }

                var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                if (modifier != null && !modifier.Hidden)
                {
                    var modifierText = Object.Instantiate(cutscene.Title, cutscene.Title.transform.parent, false);
                    modifierText.text = $"<size=5>Modifier: {modifier.Name}</size>";
                    modifierText.color = modifier.Color;
                    modifierText.transform.position = cutscene.transform.position - new Vector3(0f, 1.75f, 0f);
                    modifierText.gameObject.SetActive(true);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl._CoSetTasks_d__83), nameof(PlayerControl._CoSetTasks_d__83.MoveNext))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl._CoSetTasks_d__83 __instance)
            {
                if (__instance == null) return;
                var player = __instance.__4__this;
                GetRole(player)?.RegenTask();

                for (var i = 0;i < AbilityManager.Buttons.Count;i++)
                {
                    var button = AbilityManager.Buttons[i];
                    button.KillButton.gameObject.SetActive(true);
                    button.Timer = Mathf.Min(button.MaxTimer, 10f);
                    button.KillButton.SetCoolDown(button.Timer, Mathf.Max(button.MaxTimer, 1f));
                }
            }
        }

        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                if (!AmongUsClient.Instance.AmHost) return false;
                if (__instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (lifeSuppSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                    if (reactorSystemType.Countdown < 0f) return true;
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks) return true;

                var result = true;
                foreach (var role in AllRoles)
                {
                    var isend = role.CheckEndCriteria(__instance);
                    if (!isend)
                    {
                        result = false;
                    }
                }

                if (!NobodyEndCriteria(__instance)) result = false;

                return result;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            public static void Postfix()
            {
                AbilityManager.Buttons.Clear();
                var snitch = Modifier.GetModifier<Snitch>();
                if (snitch != null)
                {
                    snitch.ImpArrows.DestroyAll();
                    snitch.SnitchArrows.DestroyAll();
                }

                RoleDictionary.Clear();
                Modifier.ModifierDictionary.Clear();
                Lights.SetLights(Color.white);
            }
        }

        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), typeof(StringNames),
            typeof(Il2CppReferenceArray<Il2CppSystem.Object>))]
        public static class TranslationController_GetString
        {
            public static void Postfix(ref string __result, [HarmonyArgument(0)] StringNames name)
            {
                if (ExileController.Instance == null || ExileController.Instance.exiled == null) return;

                switch (name)
                {
                    case StringNames.ExileTextPN:
                    case StringNames.ExileTextSN:
                    case StringNames.ExileTextPP:
                    case StringNames.ExileTextSP:
                        {
                            var info = ExileController.Instance.exiled;
                            var role = GetRole(info.Object);
                            if (role == null) return;
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
                            __result = $"{info.PlayerName} was {roleName}.";
                            return;
                        }
                }
            }
        }

        [HarmonyPatch]
        public static class NamePatch
        {
            public static void SetNameText(TextMeshPro nameText, PlayerControl player, bool resetName = true, bool inMeeting = false)
            {
                if (player == null || (!resetName && nameText.text == "")) return;
                if (resetName)
                    nameText.text = player.name;
                var role = GetRole(player);
                if (role == null) return;

                var amOwner = player.AmOwner;
                var localRole = GetRole(PlayerControl.LocalPlayer);

                if (amOwner && role.Hidden)
                {
                    var isImpostor = player.Data.IsImpostor;
                    nameText.color = isImpostor ? Palette.ImpostorRed : Color.white;
                    nameText.text = $"{player.name}\n{(isImpostor ? "Impostor" : "Crewmate")}";
                }
                else if (role.Criteria())
                {
                    var color = role.Color;
                    var roleName = role.Name;
                    var suffix = "";

                    if (!amOwner)
                    {
                        if (role.RoleType == RoleEnum.LoverImpostor)
                        {
                            if (localRole.RoleType == RoleEnum.Lover)
                                roleName = "Lover";
                            else if (localRole.Faction == Faction.Impostors)
                            {
                                roleName = "Impostor";
                                color = Palette.ImpostorRed;
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(suffix))
                        suffix = $"\n{roleName}";

                    nameText.text = $"{player.name}{suffix}";
                    nameText.color = color;
                }
                else if (player.Data.IsImpostor && PlayerControl.LocalPlayer.Data.IsImpostor)
                    nameText.color = Palette.ImpostorRed;
                else
                    nameText.color = Color.white;

                var modifier = Modifier.GetModifier(player);

                if (modifier != null && modifier.Criteria())
                {
                    var lines = nameText.text.Split("\n");
                    lines[0] = $"{player.name} {Utils.ColorText(modifier.Color, $"({modifier.Name})")}";
                    nameText.text = string.Join("\n", lines);
                }

                switch (localRole.RoleType)
                {
                    case RoleEnum.Executioner:
                        var target = ((Executioner)localRole).Target;
                        if (target.PlayerId == player.PlayerId)
                            nameText.color = Color.black;
                        break;
                    case RoleEnum.Arsonist:
                        var doused = ((Arsonist)localRole).DousedPlayers;
                        if (doused.Contains(player.PlayerId))
                            nameText.color = Color.black;
                        break;
                    case RoleEnum.Eraser:
                        var erased = ((Eraser)localRole).ErasedPlayers;
                        if (erased.Contains(player.PlayerId))
                            nameText.color = localRole.Color;
                        break;
                }

                return;
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
            public static void PostMeeting() => UpdateAll();

            [HarmonyPostfix]
            [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.SpawnPlayer))]
            public static void OnSpawn([HarmonyArgument(0)] PlayerControl player)
            {
                var hatId = player.Data.HatId;
                player.nameText.transform.localPosition = new Vector3(
                    0f,
                    hatId == 0U ? 1.5f : 2.0f,
                    -0.5f
                );

                SetNameText(player.nameText, player);
                UpdateDisplay(player);
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
            public static void OnMeetingStart(MeetingHud __instance)
            {
                foreach (var voteArea in __instance.playerStates)
                {
                    var player = Utils.PlayerById(voteArea.TargetPlayerId);
                    SetNameText(
                        voteArea.NameText,
                        player,
                        inMeeting: true
                    );
                }
            }

            public static void UpdateAll()
            {
                if (MeetingHud.Instance != null)
                    OnMeetingStart(MeetingHud.Instance);
                else
                    foreach (var player in PlayerControl.AllPlayerControls)
                        OnSpawn(player);
            }

            public static void UpdateDisplay(PlayerControl player)
            {
                var localRole = GetRole(PlayerControl.LocalPlayer);
                var material = player.myRend.material;

                if (player.IsShielded())
                {
                    void SetOutline()
                    {
                        material.SetFloat("_Outline", 1f);
                        material.SetColor("_OutlineColor", Color.cyan);
                    }

                    var showShielded = CustomGameOptions.ShowShielded;

                    if (showShielded == ShieldOptions.Everyone)
                        SetOutline();
                    else
                    {
                        var selfAndMedic = showShielded == ShieldOptions.SelfAndMedic;

                        if ((selfAndMedic || showShielded == ShieldOptions.Self) && player.AmOwner)
                            SetOutline();
                        else if ((selfAndMedic || showShielded == ShieldOptions.Medic) && localRole?.RoleType == RoleEnum.Medic)
                            SetOutline();
                    }
                }
                else if (material.GetColor("_OutlineColor") == Color.cyan)
                    material.SetFloat("_Outline", 0f);

                switch (localRole.RoleType)
                {
                    case RoleEnum.Arsonist:
                        var doused = ((Arsonist)localRole).DousedPlayers;
                        if (doused.Contains(player.PlayerId))
                            material.SetColor("_VisorColor", localRole.Color);
                        break;
                }
            }

            public static void UpdateSingle(PlayerControl player, bool resetName = true) {
                SetNameText(player.nameText, player, resetName);
                UpdateDisplay(player);
            }
            public static void UpdateSingle(PlayerVoteArea voteArea) =>
                SetNameText(voteArea.NameText, Utils.PlayerById(voteArea.TargetPlayerId));
        }
    }
}
