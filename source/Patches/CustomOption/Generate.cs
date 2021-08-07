using System;
using ExecTargetDead = TownOfUs.NeutralRoles.ExecutionerMod.OnTargetDead;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        #region Crewmate Roles
        public static CustomHeaderOption CrewmateRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption ButtonBarryOn;
        #endregion

        #region Neutral Roles
        public static CustomHeaderOption NeutralRoles;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PhantomOn;
        public static CustomNumberOption EraserOn;
        #endregion

        #region Impostor Roles
        public static CustomHeaderOption ImpostorRoles;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption AssassinOn;
        public static CustomNumberOption UnderdogOn;
        #endregion

        #region Modifiers
        public static CustomHeaderOption Modifiers;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption DiseasedOn;
        #endregion

        #region Custom Game Settings
        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption MaxImpostorRoles;
        public static CustomNumberOption MaxNeutralRoles;
        public static CustomNumberOption VanillaGame;
        #endregion

        #region Mayor
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorAnonymous;
        #endregion

        #region Lovers
        public static CustomToggleOption BothLoversDie;
        #endregion

        #region Sheriff
        public static CustomToggleOption ShowSheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffKeepsGameAlive;
        #endregion

        #region Engineer
        public static CustomStringOption EngineerPer;
        #endregion

        #region Medic
        public static CustomStringOption ShowShielded;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;
        #endregion

        #region The Glitch
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomStringOption GlitchHackDistanceOption;
        #endregion

        #region Morphling
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        #endregion

        #region Snitch
        public static CustomToggleOption SnitchOnLaunch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomToggleOption SnitchTargetsCrew;
        #endregion

        #region Altruist
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;
        #endregion

        #region Miner
        public static CustomNumberOption MineCooldown;
        #endregion

        #region Swooper
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;
        #endregion

        #region Executioner
        public static CustomStringOption OnTargetDead;
        #endregion

        #region Arsonist
        public static CustomNumberOption DouseCooldown;
        public static CustomToggleOption ArsonistGameEnd;
        #endregion

        #region Eraser
        public static CustomNumberOption EraseCooldown;
        public static CustomNumberOption EraserCrewPenalty;
        #endregion

        #region Undertaker
        public static CustomNumberOption DragCooldown;
        #endregion

        #region Assassin
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinGuessNeutrals;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinMultiKill;
        #endregion

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";

        private static CustomNumberOption CreateOptionGroup(int id, string name, string color, CustomOption[] children)
        {
            var option = new CustomNumberOption(true, id, $"<color={color}>{name}</color>", 0f, 0f, 100f, 10f, PercentFormat);
            if (children.Length > 0)
            {
                var allOptions = CustomOption.AllOptions;
                allOptions.Remove(option);
                allOptions.Insert(allOptions.IndexOf(children[0]), option);
                foreach (var child in children)
                {
                    var old = child.ShouldShow;
                    child.ShouldShow = () => old() && (float)option.Value > 0f;
                    child.Parent = option;
                }
            }
            return option;
        }

        private static CustomNumberOption CreateCooldownOption(
            int id, string name,
            float value, float min, float max, float increment,
            Func<bool> ShouldShow = null
        ) => new CustomNumberOption(id, name, value, min, max, increment, CooldownFormat) {
            ShouldShow = ShouldShow ?? (() => true)
        };
        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new ImportExport(num++, "Save Custom Settings");
            Patches.ImportButton = new ImportExport(num++, "Load Custom Settings");

            CrewmateRoles = new CustomHeaderOption(num++, "Crewmate Roles");
            MayorOn = CreateOptionGroup(num++, "Mayor", "#704FA8FF", new CustomOption[] {
                MayorVoteBank = new CustomNumberOption(num++, "Initial Mayor Vote Bank", 1, 1, 5, 1),
                MayorAnonymous = new CustomToggleOption(num++, "Mayor Votes Show Anonymous", false) {
                    ShouldShow = () => !PlayerControl.GameOptions.AnonymousVotes
                },
            });
            LoversOn = CreateOptionGroup(num++, "Lovers", "#FF66CCFF", new CustomOption[] {
                BothLoversDie = new CustomToggleOption(num++, "Both Lovers Die", true),
            });
            SheriffOn = CreateOptionGroup(num++, "Sheriff", "#FFFF00FF", new CustomOption[] {
                ShowSheriff = new CustomToggleOption(num++, "Show Sheriff", false),
                SheriffKillOther = new CustomToggleOption(num++, "Sheriff Miskill Kills Crewmate", false),
                SheriffKillsJester = new CustomToggleOption(num++, "Sheriff Kills Jester", false) {
                    ShouldShow = () => CustomGameOptions.JesterOn > 0f || (
                        CustomGameOptions.ExecutionerOn > 0f && CustomGameOptions.OnTargetDead == ExecTargetDead.Jester
                    )
                },
                SheriffKillsArsonist = new CustomToggleOption(num++, "Sheriff Kills Arsonist", false) {
                    ShouldShow = () => CustomGameOptions.ArsonistOn > 0f
                },
                SheriffKeepsGameAlive = new CustomToggleOption(num++, "Sheriff Keeps Game Alive", true),
            });
            EngineerOn = CreateOptionGroup(num++, "Engineer", "#FFA60AFF", new CustomOption[] {
                EngineerPer = new CustomStringOption(num++, "Engineer Fix Per", new[] {"Round", "Game"}),
            });
            SwapperOn = CreateOptionGroup(num++, "Swapper", "#66E666FF", new CustomOption[] { });
            Func<bool> reportsEnabled = () => CustomGameOptions.ShowReports;
            MedicOn = CreateOptionGroup(num++, "Medic", "#006600FF", new CustomOption[] {
                ShowShielded = new CustomStringOption(num++, "Show Shielded Player", new[] {"Self", "Medic", "Self+Medic", "Everyone"}),
                MedicReportSwitch = new CustomToggleOption(num++, "Show Medic Reports", true),
                MedicReportNameDuration = CreateCooldownOption(num++, "Time Where Medic Reports Will Have Name", 0, 0, 60, 2.5f, reportsEnabled),
                MedicReportColorDuration = CreateCooldownOption(num++, "Time Where Medic Reports Will Have Color Type", 15, 0, 120, 2.5f, reportsEnabled),
                WhoGetsNotification = new CustomStringOption(num++, "Who gets murder attempt indicator", new[] { "Medic", "Shielded", "Everyone", "Nobody" }),
                ShieldBreaks = new CustomToggleOption(num++, "Shield breaks on murder attempt", false),
            });
            AltruistOn = CreateOptionGroup(num++, "Altruist", "#660000FF", new CustomOption[] {
                ReviveDuration = CreateCooldownOption(num++, "Altruist Revive Duration", 10, 1, 30, 1f),
                AltruistTargetBody = new CustomToggleOption(num++, "Target's body disappears on beginning of revive", false),
            });

            NeutralRoles = new CustomHeaderOption(num++, "Neutral Roles");

            JesterOn = CreateOptionGroup(num++, "Jester", "#FFBFCCFF", new CustomOption[] { });
            GlitchOn = CreateOptionGroup(num++, "The Glitch", "#00FF00FF", new CustomOption[] {
                MimicCooldownOption = CreateCooldownOption(num++, "Mimic Cooldown", 30, 10, 120, 2.5f),
                MimicDurationOption = CreateCooldownOption(num++, "Mimic Duration", 10, 1, 30, 1f),
                HackCooldownOption = CreateCooldownOption(num++, "Hack Cooldown", 30, 10, 120, 2.5f),
                HackDurationOption = CreateCooldownOption(num++, "Hack Duration", 10, 1, 30, 1f),
                GlitchHackDistanceOption = new CustomStringOption(num++, "Glitch Hack Distance", new[] { "Short", "Normal", "Long" }),
            });
            ExecutionerOn = CreateOptionGroup(num++, "Executioner", "#8C4005FF", new CustomOption[] {
                OnTargetDead = new CustomStringOption(num++, "Executioner becomes on Target Dead", new[] {"Crew", "Jester"}),
            });
            ArsonistOn = CreateOptionGroup(num++, "Arsonist", "#FF4D00FF", new CustomOption[] {
                DouseCooldown = CreateCooldownOption(num++, "Douse Cooldown", 25, 10, 40, 2.5f),
                ArsonistGameEnd = new CustomToggleOption(num++, "Game keeps going so long as Arsonist is alive", false),
            });
            EraserOn = CreateOptionGroup(num++, "Eraser", "#FF33CC", new CustomOption[] {
                EraseCooldown = CreateCooldownOption(num++, "Erase Cooldown", 30, 30, 60, 2.5f),
                EraserCrewPenalty = CreateCooldownOption(num++, "Eraser Extra Cooldown for erasing vanilla Crewmate", 2f, 1f, 4f, 1f),
            });
            PhantomOn = CreateOptionGroup(num++, "Phantom", "#662962", new CustomOption[] { });

            ImpostorRoles = new CustomHeaderOption(num++, "Impostor Roles");

            AssassinOn = CreateOptionGroup(num++, "Assassin", "#FF0000FF", new CustomOption[] {
                AssassinKills = new CustomNumberOption(num++, "Number of Assassin Kills", 1, 1, 5, 1),
                AssassinCrewmateGuess = new CustomToggleOption(num++, "Assassin can Guess \"Crewmate\"", false),
                AssassinGuessNeutrals = new CustomToggleOption(num++, "Assassin can Guess Neutral roles", false),
                AssassinMultiKill = new CustomToggleOption(num++, "Assassin can kill more than once per meeting", true) {
                    ShouldShow = () => CustomGameOptions.AssassinKills > 1
                },
            });
            JanitorOn = CreateOptionGroup(num++, "Janitor", "#FF0000FF", new CustomOption[] { });
            MorphlingOn = CreateOptionGroup(num++, "Morphling", "#FF0000FF", new CustomOption[] {
                MorphlingCooldown = CreateCooldownOption(num++, "Morphling Cooldown", 25, 10, 40, 2.5f),
                MorphlingDuration = CreateCooldownOption(num++, "Morphling Duration", 10, 5, 15, 1f),
            });
            MinerOn = CreateOptionGroup(num++, "Miner", "#FF0000FF", new CustomOption[] {
                MineCooldown = CreateCooldownOption(num++, "Mine Cooldown", 25, 10, 40, 2.5f),
            });
            SwooperOn = CreateOptionGroup(num++, "Swooper", "#FF0000FF", new CustomOption[] {
                SwoopCooldown = CreateCooldownOption(num++, "Swoop Cooldown", 25, 10, 40, 2.5f),
                SwoopDuration = CreateCooldownOption(num++, "Swoop Duration", 10, 5, 15, 1f),
            });
            UndertakerOn = CreateOptionGroup(num++, "Undertaker", "#FF0000FF", new CustomOption[] {
                DragCooldown = CreateCooldownOption(num++, "Drag Cooldown", 25, 10, 40, 2.5f),
            });
            UnderdogOn = CreateOptionGroup(num++, "Underdog", "#FF0000FF", new CustomOption[] { });

            Modifiers = new CustomHeaderOption(num++, "Modifiers");

            SnitchOn = CreateOptionGroup(num++, "Snitch", "#D4AF37FF", new CustomOption[] {
                SnitchOnLaunch = new CustomToggleOption(num++, "Snitch knows who they are on Game Start", false),
                SnitchSeesNeutrals = new CustomToggleOption(num++, "Snitch sees neutral roles", false),
                SnitchTargetsCrew = new CustomToggleOption(num++, "Snitch prioritizes vanilla crew", false),
            });
            TorchOn = CreateOptionGroup(num++, "Torch", "#FFFF99FF", new CustomOption[] { });
            DiseasedOn = CreateOptionGroup(num++, "Diseased", "#808080FF", new CustomOption[] { });
            ButtonBarryOn = CreateOptionGroup(num++, "Button Barry", "#E600FFFF", new CustomOption[] { });

            CustomGameSettings = new CustomHeaderOption(num++, "Custom Game Settings");
            ImpostorSeeRoles = new CustomToggleOption(num++, "Impostors can see the roles of their team", false);

            DeadSeeRoles = new CustomToggleOption(num++, "Dead can see everyone's roles", false);

            MaxImpostorRoles = new CustomNumberOption(num++, "Max Impostor Roles", 1f, 1f, 3f, 1f);
            MaxNeutralRoles = new CustomNumberOption(num++, "Max Neutral Roles", 1f, 1f, 5f, 1f);
            VanillaGame = new CustomNumberOption(num++, "Probability of a completely vanilla game", 0f, 0f, 100f, 5f, PercentFormat);
        }
    }
}
