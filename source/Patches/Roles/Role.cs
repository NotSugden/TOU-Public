using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Extensions;
using TMPro;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace TownOfUs.Roles
{
    public abstract class Role
    {
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


        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)RoleType);
        }

        public static readonly Dictionary<byte, Role> RoleDictionary = new Dictionary<byte, Role>();
        public static IEnumerable<Role> AllRoles => RoleDictionary.Values.ToList();
        protected internal string Name { get; set; }

        private PlayerControl _player { get; set; }

        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null)
                {
                    _player.nameText.color = Color.white;
                }

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }

        protected Func<string> ImpostorText;
        protected Func<string> TaskText;
        protected float Scale { get; set; } = 1f;
        protected internal Color Color { get => GetRoleColor(RoleType); }
        protected internal RoleEnum RoleType { get; set; }

        protected internal bool Hidden { get; set; } = false;

        //public static Faction Faction;
        protected internal Faction Faction { get; set; } = Faction.Crewmates;

        protected internal Color FactionColor
        {
            get
            {
                return Faction switch
                {
                    Faction.Crewmates => Color.green,
                    Faction.Impostors => Palette.ImpostorRed,
                    Faction.Neutral => CustomGameOptions.NeutralRed ? Color.red : Color.grey,
                    _ => Color.white
                };
            }
        }

        public static bool NobodyWins = false;

        public List<KillButtonManager> ExtraButtons = new List<KillButtonManager>();
        public static uint NetId => PlayerControl.LocalPlayer.NetId;
        public string PlayerName { get; set; }

        //public static T Gen<T>()

        public virtual bool Criteria()
        {
            Player.nameText.transform.localPosition = new Vector3(
                0f,
                Player.Data.HatId == 0U ? 0.7f : 1.1f,
                -0.5f
            );
            if (PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles) return Utils.ShowDeadBodies;
            if (Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor && CustomGameOptions.ImpostorSeeRoles) return true;
            return GetRole(PlayerControl.LocalPlayer) == this || AmongUsClient.Instance?.GameMode == GameModes.LocalGame;
        }

        protected virtual void IntroPrefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourteam)
        {
        }

        public static void NobodyWinsFunc()
        {

            Role.NobodyWins = true;
        }

        internal static bool NobodyEndCriteria(ShipStatus __instance)
        {
            bool CheckNoImpsNoCrews()
            {
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (alives.Count == 0) return false;
                var flag = alives.All(x =>
                {
                    var role = GetRole(x);
                    if (role == null) return false;
                    var flag2 = role.Faction == Faction.Neutral && !x.Is(RoleEnum.Glitch) && !x.Is(RoleEnum.Arsonist) && !x.Is(RoleEnum.Phantom);
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

        internal virtual bool CheckEndCriteria(ShipStatus __instance)
        {
            return true;
        }

        protected virtual string NameText(PlayerVoteArea player = null)
        {

            if (CamouflageMod.CamouflageUnCamouflage.IsCamoed && player == null)
            {
                return "";
            }

            if (Player == null) return "";

            if (player != null && (MeetingHud.Instance.state == MeetingHud.VoteStates.Proceeding ||
                MeetingHud.Instance.state == MeetingHud.VoteStates.Results)) return Player.name;

            if (!CustomGameOptions.RoleUnderName && player == null) return Player.name;

            Player.nameText.transform.localPosition = new Vector3(
                0f,
                (Player.Data.HatId == 0U) ? 1.05f :
                CustomHats.HatCreation.TallIds.Contains(Player.Data.HatId) ? 1.6f : 1.4f,
                -0.5f
            );
            return Player.name + "\n" + Name;
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

        public static Role Gen(Type T, PlayerControl player, CustomRPC rpc)
        {
            var role = (Role) Activator.CreateInstance(T, new object[] { player });
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)rpc,
                SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            System.Console.WriteLine($"{player.name} is {role.Name}");
            return role;
        }

        public static Role Gen(Type T, List<PlayerControl> crewmates, CustomRPC rpc)
        {
            if (crewmates.Count <= 0) return null;
            var rand = UnityEngine.Random.RandomRangeInt(0, crewmates.Count);
            var pc = crewmates[rand];
            crewmates.Remove(pc);

            return Gen(T, pc, rpc);
        }

        protected Role(PlayerControl player)
        {
            Player = player;
            RoleDictionary.Add(player.PlayerId, this);
        }

        public static Color GetRoleColor(RoleEnum roleId)
        {
            return roleId switch
            {
                RoleEnum.Sheriff => Color.yellow,
                RoleEnum.Jester => new Color(1f, 0.75f, 0.8f, 1f),
                RoleEnum.Engineer => new Color(1f, 0.65f, 0.04f, 1f),
                RoleEnum.LoverImpostor => new Color(1f, 0.4f, 0.8f, 1f),
                RoleEnum.Lover => new Color(1f, 0.4f, 0.8f, 1f),
                RoleEnum.Mayor => new Color(0.44f, 0.31f, 0.66f, 1f),
                RoleEnum.Swapper => new Color(0.4f, 0.9f, 0.4f, 1f),
                RoleEnum.Investigator => new Color(0f, 0.7f, 0.7f, 1f),
                RoleEnum.TimeLord => new Color(0f, 0f, 1f, 1f),
                RoleEnum.Shifter => new Color(0.6f, 0.6f, 0.6f, 1f),
                RoleEnum.Medic => new Color(0f, 0.4f, 0f, 1f),
                RoleEnum.Seer => new Color(1f, 0.8f, 0.5f, 1f),
                RoleEnum.Executioner => new Color(0.55f, 0.25f, 0.02f, 1f),
                RoleEnum.Spy => new Color(0.8f, 0.64f, 0.8f, 1f),
                RoleEnum.Arsonist => new Color(1f, 0.3f, 0f),
                RoleEnum.Phantom => new Color(0.4f, 0.16f, 0.38f, 1f),
                RoleEnum.Altruist => new Color(0.4f, 0f, 0f, 1f),
                RoleEnum.Assassin => Palette.ImpostorRed,
                RoleEnum.Miner => Palette.ImpostorRed,
                RoleEnum.Swooper => Palette.ImpostorRed,
                RoleEnum.Morphling => Palette.ImpostorRed,
                RoleEnum.Camouflager => Palette.ImpostorRed,
                RoleEnum.Janitor => Palette.ImpostorRed,
                RoleEnum.Undertaker => Palette.ImpostorRed,
                RoleEnum.Glitch => Color.green,
                RoleEnum.Impostor => Palette.ImpostorRed,
                _ => Color.white,
            };
        }

        public static string GetRoleName(RoleEnum roleId, bool includeColor = false)
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
                RoleEnum.Investigator => "Investigator",
                RoleEnum.TimeLord => "Time Lord",
                RoleEnum.Shifter => "Shifter",
                RoleEnum.Medic => "Medic",
                RoleEnum.Seer => "Seer",
                RoleEnum.Executioner => "Executioner",
                RoleEnum.Spy => "Spy",
                RoleEnum.Arsonist => "Arsonist",
                RoleEnum.Altruist => "Altruist",
                RoleEnum.Assassin => "Assassin",
                RoleEnum.Miner => "Miner",
                RoleEnum.Swooper => "Swooper",
                RoleEnum.Morphling => "Morphling",
                RoleEnum.Camouflager => "Camouflager",
                RoleEnum.Janitor => "Janitor",
                RoleEnum.Undertaker => "Undertaker",
                RoleEnum.Glitch => "The Glitch",
                RoleEnum.Impostor => "Impostor",
                _ => "Crewmate",
            };

            return includeColor
                ? Utils.ColorText(GetRoleColor(roleId), roleName)
                : roleName;
        }


        public static Role GetRole(PlayerControl player)
        {
            return (from entry in RoleDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
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

        public static IEnumerable<Role> GetRoles(RoleEnum roletype)
        {
            return AllRoles.Where(x => x.RoleType == roletype);
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginImpostor))]
        public class IntroCutscene_Impostor
        {
            public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
            {
                IntroCutscene_Patch.Prefix(__instance, ref yourTeam);
            }

            public static void Postfix(IntroCutscene __instance)
            {
                IntroCutscene_Patch.Postfix(__instance);
            }
        }

        [HarmonyPatch(typeof(IntroCutscene), nameof(IntroCutscene.BeginCrewmate))]
        public class IntroCutscene_Patch
        {
            public static void Prefix(IntroCutscene __instance, ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
            {
                var role = GetRole(PlayerControl.LocalPlayer);
                if (role != null)
                {
                    role.IntroPrefix(__instance, ref yourTeam);
                }
            }

            public static void Postfix(IntroCutscene __instance)
            {
                var role = GetRole(PlayerControl.LocalPlayer);
                var alpha = __instance.Title.color.a;

                if (role != null && !role.Hidden)
                {
                    __instance.Title.text = role.Name;
                    __instance.Title.color = role.Color;
                    __instance.ImpostorText.text = role.ImpostorText();
                    __instance.ImpostorText.gameObject.SetActive(true);
                    __instance.BackgroundBar.material.color = role.Color;

                }
                else if (role.RoleType == RoleEnum.Crewmate || role.Hidden)
                {
                    __instance.ImpostorText.text = "Haha imagine being a boring old crewmate";
                }

                var modifier = Modifier.GetModifier(PlayerControl.LocalPlayer);
                if (modifier != null)
                {
                    if (modifier.Hidden) return;
                    var modifierText = Object.Instantiate(__instance.Title, __instance.Title.transform.parent, false);
                    modifierText.text = $"<size=5>Modifier: {modifier.Name}</size>";
                    modifierText.color = modifier.Color;
                    modifierText.transform.position = __instance.transform.position - new Vector3(0f, 1.75f, 0f);
                    modifierText.gameObject.SetActive(true);
                }
            }
        }

        public void RemoveCustomTasks()
        {
            var tasks = Player.myTasks;
            if (tasks.Count == 0) return;
            var task = tasks[0].TryCast<ImportantTextTask>();

            if (task?.Text.Contains("Role:") == true)
            {
                tasks.RemoveAt(0);
                if (tasks.Count == 0) return;
                task = tasks[0].TryCast<ImportantTextTask>();
            }

            if (task?.Text.Contains("Modifier:") == true)
            {
                tasks.RemoveAt(0);
            }
        }

        public void RegenTask()
        {
            var modifier = Modifier.GetModifier(Player);
            var modifierText = modifier?.TaskText();

            RemoveCustomTasks();

            if (modifierText != null && !modifierText.Equals(""))
            {
                var modTask = new GameObject(modifier.Name + "Task").AddComponent<ImportantTextTask>();
                modTask.transform.SetParent(Player.transform, false);
                modTask.Text =
                    Utils.ColorText(modifier.Color, $"Modifier: {modifier.Name}\n{modifierText}");
                Player.myTasks.Insert(0, modTask);
            }

            if (Hidden) return;

            var roleTask = new GameObject(Name + "Task").AddComponent<ImportantTextTask>();
            roleTask.transform.SetParent(Player.transform, false);
            roleTask.Text = Utils.ColorText(Color, $"Role: {Name}\n{TaskText()}");
            Player.myTasks.Insert(0, roleTask);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetTasks))]
        public static class PlayerControl_SetTasks
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance == null) return;
                var role = GetRole(__instance);

                role?.RegenTask();
            }
        }

        /*[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
        public static class ButtonsFix
        {
            public static void Postfix(PlayerControl __instance)
            {
                if (__instance != PlayerControl.LocalPlayer) return;

                var role = GetRole(PlayerControl.LocalPlayer);
                if (role == null) return;
                var instance = DestroyableSingleton<HudManager>.Instance;
                var position = instance.KillButton.transform.position;
                foreach (var button in role.ExtraButtons)
                {
                    button.transform.position = new Vector3(position.x,
                        instance.ReportButton.transform.position.y, position.z);
                }

            }
        }*/




        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CheckEndCriteria))]
        public static class ShipStatus_KMPKPPGPNIH
        {
            public static bool Prefix(ShipStatus __instance)
            {
                if (!AmongUsClient.Instance.AmHost) return false;
                if (__instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = __instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (lifeSuppSystemType.Countdown < 0f)
                    {
                        return true;
                    }
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (reactorSystemType.Countdown < 0f)
                    {
                        return true;
                    }
                }

                if (__instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = __instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();
                    if (reactorSystemType.Countdown < 0f)
                    {
                        return true;
                    }
                }

                if (GameData.Instance.TotalTasks <= GameData.Instance.CompletedTasks)
                {
                    return true;
                }

                var result = true;
                foreach (var role in AllRoles)
                {
                    var isend = role.CheckEndCriteria(__instance);
                    if (!isend)
                    {
                        result = false;
                    }
                }

                if (!NobodyEndCriteria(__instance))
                {
                    result = false;
                }

                return result;
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        public static class LobbyBehaviour_Start
        {
            private static void Postfix(LobbyBehaviour __instance)
            {
                foreach (var _modifier in Modifier.AllModifiers.Where(x => x.ModifierType == ModifierEnum.Snitch))
                {
                    var modifier = (Snitch)_modifier;
                    modifier.ImpArrows.DestroyAll();
                    modifier.ImpArrows.Clear();
                    modifier.SnitchArrows.DestroyAll();
                    modifier.SnitchArrows.Clear();
                }

                RoleDictionary.Clear();
                Modifier.ModifierDictionary.Clear();
            }
        }



        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString),
            new[] { typeof(StringNames), typeof(Il2CppReferenceArray<Il2CppSystem.Object>) })]
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
                            var role = Role.GetRole(info.Object);
                            if (role == null) return;
                            var roleName = role.RoleType == RoleEnum.Glitch ? role.Name : $"The {role.Name}";
                            __result = $"{info.PlayerName} was {roleName}.";
                            return;
                        }
                }
            }
        }

        public static void UpdateRoleNames()
        {
            if (MeetingHud.Instance) ShowRoleNames.Update(MeetingHud.Instance);
        }

        public static class ShowRoleNames
        {
            public static List<GameObject> roleNameTextList = new List<GameObject>();
            public static void Hide()
            {
                foreach (var gameObject in roleNameTextList)
                {
                    gameObject.Destroy();
                }
                roleNameTextList.Clear();
            }
            public static void Update(MeetingHud __instance)
            {
                Hide();
                var localPlayer = PlayerControl.LocalPlayer;
                var localRole = GetRole(localPlayer);
                var localIsSnitch = localPlayer.Is(ModifierEnum.Snitch);
                var completedTasks = localIsSnitch && Modifier.GetModifier<Snitch>(localPlayer).TasksDone;
                foreach (var player in __instance.playerStates)
                {
                    var role = GetRole(player);
                    var originalPos = player.NameText.transform.position;
                    player.NameText.transform.position = new Vector3(
                        originalPos.x - 0.27f,
                        originalPos.y + 0.07f,
                        originalPos.z
                    );
                    if (role == null) continue;
                    var modifier = Modifier.GetModifier(player);

                    var isSnitch = modifier != null && modifier?.ModifierType == ModifierEnum.Snitch;
                    var isDead = localPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && Utils.ShowDeadBodies;
                    

                    if (role.Criteria() || (completedTasks && (
                        role.Faction == Faction.Impostors || (
                            CustomGameOptions.SnitchCanSeeNeutrals && role.Faction == Faction.Neutral
                        )
                    )))
                    {
                        player.NameText.color = role.Color;
                        var roleNameText = Object.Instantiate(player.NameText, player.transform);
                        roleNameText.transform.position = player.NameText.transform.position - new Vector3(0f, 0.25f, 0f);
                        roleNameText.text = role.Name;
                        roleNameText.color = role.Color;
                        roleNameTextList.Add(roleNameText.gameObject);
                    }
                    if (
                        isSnitch && (isDead || (
                            localRole.Faction == Faction.Impostors
                            || (CustomGameOptions.SnitchCanSeeNeutrals && localRole.Faction == Faction.Neutral)
                        ) && ((Snitch)modifier).TasksDone)
                    )
                    {
                        player.NameText.text = $"{role.Player.name} {Utils.ColorText(modifier.Color, "(Snitch)")}";
                        if (!isDead) player.NameText.color = modifier.Color;
                    }
                    else
                    {
                        player.NameText.text = role.Player.name;
                    }
                }
            }
            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
            public static class MeetingHud_Start
            {
                public static void Postfix(MeetingHud __instance) => Update(__instance);
            }

            [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
            public static class MeetingHud_VotingComplete
            {
                public static void Postfix(MeetingHud __instance) => Hide();
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class HudManager_Update
        {
            [HarmonyPriority(Priority.First)]
            private static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;

                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!(player.Data.IsImpostor && PlayerControl.LocalPlayer.Data.IsImpostor))
                    {
                        player.nameText.text = player.name;
                        player.nameText.color = Color.white;
                    }

                    var role = GetRole(player);
                    if (role != null && role.Criteria())
                    {
                        player.nameText.color = role.Color;
                        player.nameText.text = role.NameText();
                        continue;
                    }
                }

            }
        }
    }
}
