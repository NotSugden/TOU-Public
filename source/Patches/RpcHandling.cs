using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
//using Il2CppSystem;
using Reactor;
using Reactor.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.UI;
using Coroutine = TownOfUs.JanitorMod.Coroutine;

namespace TownOfUs
{
    public static class RpcHandling
    {
        //public static readonly System.Random Rand = new System.Random();

        private static readonly List<(Type, CustomRPC, int)> CrewmateRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ImpostorRoles = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CrewmateModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static bool LoversOn = false;

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {

                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetMayor:
                        readByte = reader.ReadByte();
                        new Roles.Mayor(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetJester:
                        readByte = reader.ReadByte();
                        new Roles.Jester(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetSheriff:
                        readByte = reader.ReadByte();
                        new Roles.Sheriff(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetEngineer:
                        readByte = reader.ReadByte();
                        new Roles.Engineer(Utils.PlayerById(readByte));
                        break;


                    case CustomRPC.SetJanitor:
                        new Roles.Janitor(Utils.PlayerById(reader.ReadByte()));

                        break;

                    case CustomRPC.SetSwapper:
                        readByte = reader.ReadByte();
                        new Roles.Swapper(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetShifter:
                        readByte = reader.ReadByte();
                        new Roles.Shifter(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetInvestigator:
                        readByte = reader.ReadByte();
                        new Roles.Investigator(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTimeLord:
                        readByte = reader.ReadByte();
                        new Roles.TimeLord(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetTorch:
                        readByte = reader.ReadByte();
                        new Torch(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetDiseased:
                        readByte = reader.ReadByte();
                        new Diseased(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetFlash:
                        readByte = reader.ReadByte();
                        new Flash(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetMedic:
                        readByte = reader.ReadByte();
                        new Roles.Medic(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.SetMorphling:
                        readByte = reader.ReadByte();
                        new Roles.Morphling(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.SetAssassin:
                        readByte = reader.ReadByte();
                        new Roles.Assassin(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Lover>(winnerlover).Win();
                        break;


                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Jester)
                            {
                                ((Jester) role).Loses();
                            }
                        }

                        break;

                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Glitch)
                            {
                                ((Glitch) role).Loses();
                            }
                        }

                        break;

                    case CustomRPC.ShifterLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Shifter)
                            {
                                ((Shifter) role).Loses();
                            }
                        }

                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Executioner)
                            {
                                ((Executioner) role).Loses();
                            }
                        }

                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var b1 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);

                        var roleLover1 = new Roles.Lover(lover1, 1, b1 == 0);
                        var roleLover2 = new Roles.Lover(lover2, 2, b1 == 0);

                        roleLover1.OtherLover = roleLover2;
                        roleLover2.OtherLover = roleLover1;

                        break;

                    case CustomRPC.Start:
                        /*
                        EngineerMod.PerformKill.UsedThisRound = false;
                        EngineerMod.PerformKill.SabotageTime = DateTime.UtcNow.AddSeconds(-100);
                        */
                        Utils.ShowDeadBodies = false;
                        MedicMod.Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        TimeLordMod.RecordRewind.points.Clear();
                        AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Roles.Role.GetRole<Roles.Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == readByte)
                            {
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));
                            }
                        }

                        break;
                    case CustomRPC.EngineerFix:
                        var engineer = Utils.PlayerById(reader.ReadByte());
                        Roles.Role.GetRole<Roles.Engineer>(engineer).Uses++;
                        break;



                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.SetExtraVotes:

                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Roles.Role.GetRole<Mayor>(mayor);
                        mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();
                        if (!mayor.Is(RoleEnum.Mayor))
                        {
                            mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;
                        }

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapperMod.SwapVotes.Swap1 =
                            MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapperMod.SwapVotes.Swap2 =
                            MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == readSByte2);
                        break;

                    case CustomRPC.Shift:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var shifter = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        ShifterMod.PerformKillButton.Shift(Role.GetRole<Shifter>(shifter), other);
                        break;
                    case CustomRPC.Rewind:
                        readByte = reader.ReadByte();
                        var TimeLordPlayer = Utils.PlayerById(readByte);
                        var TimeLordRole = Role.GetRole<Roles.TimeLord>(TimeLordPlayer);
                        TimeLordMod.StartStop.StartRewind(TimeLordRole);
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.RewindRevive:
                        readByte = reader.ReadByte();
                        TimeLordMod.RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        MedicMod.StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.SetGlitch:
                        byte GlitchId = reader.ReadByte();
                        PlayerControl GlitchPlayer = Utils.PlayerById(GlitchId);
                        new Roles.Glitch(GlitchPlayer);
                        break;
                    case CustomRPC.BypassKill:
                        PlayerControl killer = Utils.PlayerById(reader.ReadByte());
                        PlayerControl target = Utils.PlayerById(reader.ReadByte());
                        bool showBody = reader.ReadBoolean();
                        Utils.MurderPlayer(killer, target, showBody);
                        if (MeetingHud.Instance?.isActiveAndEnabled == true)
                        {
                            foreach (PlayerVoteArea _voteArea in MeetingHud.Instance.playerStates)
                            {
                                PlayerControl _player = Utils.PlayerById((byte) _voteArea.TargetPlayerId);
                                if (_player == null || _player.Data.IsDead) continue;
                                MeetingHud.Instance.HandleDisconnect(_player, InnerNet.DisconnectReasons.Custom);
                            }
                        }
                        break;
                    case CustomRPC.SetMimic:
                        PlayerControl glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        PlayerControl mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Roles.Role.GetRole<Roles.Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Utils.Morph(glitchPlayer, mimicPlayer);
                        break;
                    case CustomRPC.RpcResetAnim:
                        PlayerControl animPlayer = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Roles.Role.GetRole<Roles.Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Utils.Unmorph(theGlitchRole.Player);
                        break;
                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Roles.Glitch) theGlitch)?.Wins();
                        break;
                    case CustomRPC.SetHacked:
                        PlayerControl hackPlayer = Utils.PlayerById(reader.ReadByte());
                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            ((Roles.Glitch) glitch)?.SetHacked(hackPlayer);
                        }

                        break;
                    case CustomRPC.Investigate:
                        var seer = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        Roles.Role.GetRole<Roles.Seer>(seer).Investigated.Add(otherPlayer.PlayerId);
                        Roles.Role.GetRole<Roles.Seer>(seer).LastInvestigated = DateTime.UtcNow;
                        break;
                    case CustomRPC.SetSeer:
                        new Roles.Seer(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Roles.Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.SetExecutioner:
                        new Roles.Executioner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetTarget:
                        var executioner = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Roles.Role.GetRole<Executioner>(executioner);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetCamouflager:
                        new Roles.Camouflager(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Roles.Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;
                    case CustomRPC.SetSpy:
                        new Roles.Spy(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.ExecutionerToJester:
                        ExecutionerMod.TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetSnitch:
                        new Roles.Modifiers.Snitch(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetMiner:
                        new Roles.Miner(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Roles.Role.GetRole<Roles.Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        MinerMod.PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.SetSwooper:
                        new Roles.Swooper(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Swoop:
                        var swooper = Utils.PlayerById(reader.ReadByte());
                        var swooperRole = Role.GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.SetTiebreaker:
                        new Tiebreaker(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetDrunk:
                        new Drunk(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPhantom:
                        var playerId = reader.ReadByte();
                        PhantomMod.SetPhantom.WillBePhantom = playerId == byte.MaxValue ? null : Utils.PlayerById(playerId);
                        break;
                    case CustomRPC.ForceSetPhantom:
                        var player = PhantomMod.SetPhantom.WillBePhantom;
                        Role.RoleDictionary.Remove(player.PlayerId);
                        var phantom = new Phantom(player);
                        phantom.RegenTask();
                        player.gameObject.layer = LayerMask.NameToLayer("Players");
                        PhantomMod.SetPhantom.RemoveTasks(player);
                        PhantomMod.SetPhantom.AddCollider(phantom);
                        PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantom_ = Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte()));
                        phantom_.Caught = true;
                        break;
                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Roles.Role.GetRole<Roles.Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;
                        
                        break;
                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Roles.Role.GetRole<Roles.Arsonist>(theArsonist);
                        ArsonistMod.PerformKill.Ignite(theArsonistRole);
                        break;

                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Roles.Arsonist) theArsonistTheRole)?.Wins();
                        break;
                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Arsonist)
                            {
                                ((Arsonist) role).Loses();
                            }
                        }

                        break;
                    case CustomRPC.PhantomWin:
                        var phantomRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Phantom);
                        ((Phantom)phantomRole).CompletedTasks = true;
                        break;
                    case CustomRPC.PhantomLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Phantom)
                            {
                                ((Phantom) role).Loses();
                            }
                        }

                        break;
                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SyncCustomSettings:
                        CustomOption.Rpc.ReceiveRpc(reader);
                        break;
                    case CustomRPC.SetAltruist:
                        new Altruist(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetBigBoi:
                        new BigBoi(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Roles.Role.GetRole<Roles.Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = UnityEngine.Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies)
                        {
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                {
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color, CustomGameOptions.ReviveDuration, 0.5f));
                                }

                                Coroutines.Start(AltruistMod.Coroutine.AltruistRevive(body, altruistRole));
                            }
                        }

                        break;
                    case CustomRPC.FixAnimation:
                        var aniPlayer = Utils.PlayerById(reader.ReadByte());
                        aniPlayer.MyPhysics.ResetMoveState(true);
                        aniPlayer.Collider.enabled = true;
                        aniPlayer.moveable = true;
                        aniPlayer.NetTransform.enabled = true;
                        break;
                    case CustomRPC.SetButtonBarry:
                        new ButtonBarry(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());
                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                            if (ShipStatus.Instance.CheckTaskCompletion())
                            {
                                return;
                            }
                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }

                        break;
                    case CustomRPC.KillDuringMeeting:
                        var deadPlayerId = reader.ReadByte();
                        Utils.KillDuringMeeting(Utils.PlayerById(deadPlayerId));
                        break;
                }
            }
        }

        internal static bool Check(int probability)
        {
            if (probability == 100) return true;
            var num = UnityEngine.Random.RandomRangeInt(0,  101) + 1;
            return num <= probability;
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
        public static class RpcSetInfected
        {
            public static void Prefix([HarmonyArgument(0)] Il2CppReferenceArray<GameData.PlayerInfo> infected)
            {
                Utils.ShowDeadBodies = false;
                Role.NobodyWins = false;
                CrewmateRoles.Clear();
                NeutralRoles.Clear();
                ImpostorRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();

                TimeLordMod.RecordRewind.points.Clear();
                MedicMod.Murder.KilledPlayers.Clear();
                AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);

                LoversOn = Check(CustomGameOptions.LoversOn);

                var list = CrewmateRoles;

                void CheckRole(int chance, Type role, CustomRPC rpc)
                {
                    if (Check(chance)) list.Add((role, rpc, chance));
                }

                #region Crewmate Roles
                CheckRole(CustomGameOptions.MayorOn, typeof(Mayor), CustomRPC.SetMayor);
                CheckRole(CustomGameOptions.SheriffOn, typeof(Sheriff), CustomRPC.SetSheriff);
                CheckRole(CustomGameOptions.EngineerOn, typeof(Engineer), CustomRPC.SetEngineer);
                CheckRole(CustomGameOptions.SwapperOn, typeof(Swapper), CustomRPC.SetSwapper);
                if (PlayerControl.GameOptions.MapId != 4)
                    CheckRole(CustomGameOptions.InvestigatorOn, typeof(Investigator), CustomRPC.SetInvestigator);
                CheckRole(CustomGameOptions.TimeLordOn, typeof(TimeLord), CustomRPC.SetTimeLord);
                CheckRole(CustomGameOptions.MedicOn, typeof(Medic), CustomRPC.SetMedic);
                CheckRole(CustomGameOptions.SeerOn, typeof(Seer), CustomRPC.SetSeer);
                CheckRole(CustomGameOptions.SpyOn, typeof(Spy), CustomRPC.SetSpy);
                CheckRole(CustomGameOptions.AltruistOn, typeof(Altruist), CustomRPC.SetAltruist);
                #endregion
                list = NeutralRoles;
                #region Neutral Roles
                CheckRole(CustomGameOptions.JesterOn, typeof(Jester), CustomRPC.SetJester);
                CheckRole(CustomGameOptions.ArsonistOn, typeof(Arsonist), CustomRPC.SetArsonist);
                CheckRole(CustomGameOptions.ShifterOn, typeof(Shifter), CustomRPC.SetShifter);
                CheckRole(CustomGameOptions.ExecutionerOn, typeof(Executioner), CustomRPC.SetExecutioner);
                CheckRole(CustomGameOptions.GlitchOn, typeof(Glitch), CustomRPC.SetGlitch);
                #endregion
                list = ImpostorRoles;
                #region Impostor Roles
                CheckRole(CustomGameOptions.MorphlingOn, typeof(Morphling), CustomRPC.SetMorphling);
                CheckRole(CustomGameOptions.AssassinOn, typeof(Assassin), CustomRPC.SetAssassin);
                CheckRole(CustomGameOptions.MinerOn, typeof(Miner), CustomRPC.SetMiner);
                CheckRole(CustomGameOptions.SwooperOn, typeof(Swooper), CustomRPC.SetSwooper);
                CheckRole(CustomGameOptions.JanitorOn, typeof(Janitor), CustomRPC.SetJanitor);
                #endregion
                list = CrewmateModifiers;
                #region Crewmate Modifiers
                CheckRole(CustomGameOptions.TorchOn, typeof(Torch), CustomRPC.SetTorch);
                CheckRole(CustomGameOptions.DiseasedOn, typeof(Diseased), CustomRPC.SetDiseased);
                CheckRole(CustomGameOptions.SnitchOn, typeof(Snitch), CustomRPC.SetSnitch);
                CheckRole(CustomGameOptions.PhantomOn, typeof(Phantom), CustomRPC.SetPhantom);
                #endregion
                list = GlobalModifiers;
                #region Global Modifiers
                CheckRole(CustomGameOptions.TiebreakerOn, typeof(Tiebreaker), CustomRPC.SetTiebreaker);
                CheckRole(CustomGameOptions.FlashOn, typeof(Flash), CustomRPC.SetFlash);
                CheckRole(CustomGameOptions.DrunkOn, typeof(Drunk), CustomRPC.SetDrunk);
                CheckRole(CustomGameOptions.BigBoiOn, typeof(BigBoi), CustomRPC.SetBigBoi);
                CheckRole(CustomGameOptions.ButtonBarryOn, typeof(ButtonBarry), CustomRPC.SetButtonBarry);
                #endregion
                GenEachRole(infected.ToList());

            }
        }


        private static void GenExe(List<GameData.PlayerInfo> infected, List<PlayerControl> crewmates)
        {
            PlayerControl pc;
            var targets = Utils.getCrewmates(infected).Where(x =>
            {
                var role = Role.GetRole(x);
                return role == null || role.Faction == Faction.Crewmates;
            }).ToList();
            if (targets.Count > 1)
            {
                var rand = UnityEngine.Random.RandomRangeInt(0, targets.Count);
                pc = targets[rand];
                var role = Role.Gen(typeof(Executioner), crewmates.Where(x => x.PlayerId != pc.PlayerId).ToList(),
                    CustomRPC.SetExecutioner);
                if (role != null)
                {
                    crewmates.Remove(role.Player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte) CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(pc.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    ((Executioner) role).target = pc;
                }
            }
        }

        private static void ShuffleAndSort(List<(Type, CustomRPC, int)> list)
        {
            list.Shuffle();
            list.Sort(delegate(ValueTuple<Type, CustomRPC, int> a, ValueTuple<Type, CustomRPC, int> b)
            {
                return (a.Item3 == 100 ? 0 : 100).CompareTo(b.Item3 == 100 ? 0 : 100);
            });
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            ShuffleAndSort(CrewmateRoles);
            ShuffleAndSort(NeutralRoles);
            ShuffleAndSort(ImpostorRoles);
            ShuffleAndSort(CrewmateModifiers);
            ShuffleAndSort(GlobalModifiers);
            var neutralRoles = NeutralRoles.Take(CustomGameOptions.MaxNeutralRoles).ToList();
            var crewAndNeutRoles = neutralRoles;
            crewAndNeutRoles.AddRange(CrewmateRoles);
            ShuffleAndSort(crewAndNeutRoles);
            
            
            if (Check(CustomGameOptions.VanillaGame))
            {
                CrewmateRoles.Clear();
                NeutralRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                LoversOn = false;
                ImpostorRoles.Clear();
                crewAndNeutRoles.Clear();
            }

            var crewmates = Utils.getCrewmates(infected);
            var impostors = Utils.getImpostors(infected);
            var impRoles = ImpostorRoles.Take(CustomGameOptions.MaxImpostorRoles);

            var executionerOn = false;

            foreach (var (role, rpc, _) in crewAndNeutRoles)
            {
                if (rpc == CustomRPC.SetExecutioner)
                {
                    executionerOn = true;
                    continue;
                }
                Role.Gen(role, crewmates, rpc);
            }

            if (executionerOn)
            {
                GenExe(infected, crewmates);
            }
            
            foreach (var (role, rpc, _) in impRoles)
            {
                Role.Gen(role, impostors, rpc);
            }

            var crewmates2 = Utils.getCrewmates(infected).Where(x => !x.Is(RoleEnum.Glitch)).ToList();
            var snitchOn = false;
            var phantomOn = false;
            foreach (var (modifier, rpc, _) in CrewmateModifiers)
            {
                if (rpc == CustomRPC.SetSnitch)
                {
                    snitchOn = true;
                    continue;
                }
                if (rpc == CustomRPC.SetPhantom)
                {
                    phantomOn = true;
                    continue;
                }
                Modifier.Gen(modifier, crewmates2, rpc);
            }
            if (snitchOn)
            {
                var crewmates3 = crewmates2.Where(x => {
                    var role = Role.GetRole(x);
					return role == null || (
					    role.Faction == Faction.Crewmates &&
                        role.RoleType != RoleEnum.Altruist
					);
                }).ToList();
                Modifier.Gen(typeof(Snitch), crewmates3, CustomRPC.SetSnitch);
            }
            var phantom = byte.MaxValue;
            if (phantomOn)
            {
                var crew = Utils.getCrewmates(infected).Where(x => {
					var role = Role.GetRole(x);
					return role == null || role.Faction != Faction.Neutral;
				}).ToList();
                crew.Shuffle();
                var player = crew.Random();
                phantom = (PhantomMod.SetPhantom.WillBePhantom = player).PlayerId;
                System.Console.WriteLine($"Phantom is {player.name}");
            }
            else
            {
                PhantomMod.SetPhantom.WillBePhantom = null;
            }
            var writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                (byte) CustomRPC.SetPhantom,
                SendOption.Reliable,
                -1
            );
            writer.Write(phantom);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            var global = PlayerControl.AllPlayerControls.ToArray().Where(x => Modifier.GetModifier(x) == null && !x.Is(RoleEnum.Glitch)).ToList();
            foreach (var (modifier, rpc, _) in GlobalModifiers)
            {
                Modifier.Gen(modifier, global, rpc);
            }
            
            if (LoversOn)
            {
                Lover.Gen(crewmates, impostors);
            }

            while (true)
            {
                if (crewmates.Count == 0) break;
                Role.Gen(typeof(Crewmate), crewmates, CustomRPC.SetCrewmate);
            }

            while (true)
            {
                if (impostors.Count == 0) break;
                Role.Gen(typeof(Impostor), impostors, CustomRPC.SetImpostor);
            }
        }
    }
}
