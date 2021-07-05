using System;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        public static CustomHeaderOption CrewmateRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption ButtonBarryOn;


        public static CustomHeaderOption NeutralRoles;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PhantomOn;
        public static CustomNumberOption EraserOn;

        public static CustomHeaderOption ImpostorRoles;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption AssassinOn;
        public static CustomNumberOption UnderdogOn;

        public static CustomHeaderOption Modifiers;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption DiseasedOn;

        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption MaxImpostorRoles;
        public static CustomNumberOption MaxNeutralRoles;
        public static CustomNumberOption VanillaGame;

        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorVoteBank;
        public static CustomToggleOption MayorAnonymous;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;

        public static CustomHeaderOption Sheriff;
        public static CustomToggleOption ShowSheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffKeepsGameAlive;

        public static CustomHeaderOption Engineer;
        public static CustomStringOption EngineerPer;

        public static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;

        public static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomStringOption GlitchHackDistanceOption;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;

        public static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;

        public static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchOnLaunch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomToggleOption SnitchTargetsCrew;

        public static CustomHeaderOption Altruist;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;

        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomToggleOption ArsonistGameEnd;

        public static CustomHeaderOption Eraser;
        public static CustomNumberOption EraseCooldown;
        public static CustomNumberOption EraserCrewPenalty;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinGuessNeutrals;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinMultiKill;
        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new ImportExport(num++, "Save Custom Settings");
            Patches.ImportButton = new ImportExport(num++, "Load Custom Settings");

            CrewmateRoles = new CustomHeaderOption(num++, "Crewmate Roles");
            MayorOn = new CustomNumberOption(true, num++, "<color=#704FA8FF>Mayor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            LoversOn = new CustomNumberOption(true, num++, "<color=#FF66CCFF>Lovers</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SheriffOn = new CustomNumberOption(true, num++, "<color=#FFFF00FF>Sheriff</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            EngineerOn = new CustomNumberOption(true, num++, "<color=#FFA60AFF>Engineer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(true, num++, "<color=#66E666FF>Swapper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(true, num++, "<color=#006600FF>Medic</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            AltruistOn = new CustomNumberOption(true, num++, "<color=#660000FF>Altruist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);


            NeutralRoles = new CustomHeaderOption(num++, "Neutral Roles");
            JesterOn = new CustomNumberOption(true, num++, "<color=#FFBFCCFF>Jester</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GlitchOn = new CustomNumberOption(true, num++, "<color=#00FF00FF>The Glitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ExecutionerOn = new CustomNumberOption(true, num++, "<color=#8C4005FF>Executioner</color>", 0f, 0f, 100f,
                10f, PercentFormat);
            ArsonistOn = new CustomNumberOption(true, num++, "<color=#FF4D00FF>Arsonist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomOn = new CustomNumberOption(true, num++, "<color=#662962>Phantom</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            EraserOn = new CustomNumberOption(true, num++, "<color=#FF33CC>Eraser</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorRoles = new CustomHeaderOption(num++, "Impostor Roles");
            AssassinOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Assassin</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JanitorOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Janitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Morphling</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MinerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Miner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Swooper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Undertaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            
            UnderdogOn = new CustomNumberOption(true, num++, "<color=#FF0000FF>Underdog</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            Modifiers = new CustomHeaderOption(num++, "Modifiers");
            SnitchOn = new CustomNumberOption(true, num++, "<color=#D4AF37FF>Snitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TorchOn = new CustomNumberOption(true, num++, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedOn =
                new CustomNumberOption(true, num++, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f,
                    PercentFormat);
            ButtonBarryOn =
                new CustomNumberOption(true, num++, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
                    PercentFormat);


            CustomGameSettings =
                new CustomHeaderOption(num++, "Custom Game Settings");
            ImpostorSeeRoles = new CustomToggleOption(num++, "Impostors can see the roles of their team", false);

            DeadSeeRoles =
                new CustomToggleOption(num++, "Dead can see everyone's roles", false);

            MaxImpostorRoles =
                new CustomNumberOption(num++, "Max Impostor Roles", 1f, 1f, 3f, 1f);
            MaxNeutralRoles =
                new CustomNumberOption(num++, "Max Neutral Roles", 1f, 1f, 5f, 1f);
            VanillaGame = new CustomNumberOption(num++, "Probability of a completely vanilla game", 0f, 0f, 100f, 5f,
                PercentFormat);

            Mayor =
                new CustomHeaderOption(num++, "<color=#704FA8FF>Mayor</color>");

            MayorVoteBank =
                new CustomNumberOption(num++, "Initial Mayor Vote Bank", 1, 1, 5, 1);

            MayorAnonymous =
                new CustomToggleOption(num++, "Mayor Votes Show Anonymous", false);

            Lovers =
                new CustomHeaderOption(num++, "<color=#FF66CCFF>Lovers</color>");
            BothLoversDie = new CustomToggleOption(num++, "Both Lovers Die");

            Sheriff =
                new CustomHeaderOption(num++, "<color=#FFFF00FF>Sheriff</color>");
            ShowSheriff = new CustomToggleOption(num++, "Show Sheriff", false);

            SheriffKillOther =
                new CustomToggleOption(num++, "Sheriff Miskill Kills Crewmate", false);

            SheriffKillsJester =
                new CustomToggleOption(num++, "Sheriff Kills Jester", false);
            SheriffKillsArsonist =
                new CustomToggleOption(num++, "Sheriff Kills Arsonist", false);
            SheriffKeepsGameAlive =
                new CustomToggleOption(num++, "Sheriff Keeps Game Alive", true);

            Engineer =
                new CustomHeaderOption(num++, "<color=#FFA60AFF>Engineer</color>");
            EngineerPer =
                new CustomStringOption(num++, "Engineer Fix Per", new[] {"Round", "Game"});

            Medic =
                new CustomHeaderOption(num++, "<color=#006600FF>Medic</color>");

            ShowShielded =
                new CustomStringOption(num++, "Show Shielded Player",
                    new[] {"Self", "Medic", "Self+Medic", "Everyone"});

            MedicReportSwitch = new CustomToggleOption(num++, "Show Medic Reports");

            MedicReportNameDuration =
                new CustomNumberOption(num++, "Time Where Medic Reports Will Have Name", 0, 0, 60, 2.5f,
                    CooldownFormat);

            MedicReportColorDuration =
                new CustomNumberOption(num++, "Time Where Medic Reports Will Have Color Type", 15, 0, 120, 2.5f,
                    CooldownFormat);

            WhoGetsNotification =
                new CustomStringOption(num++, "Who gets murder attempt indicator",
                    new[] {"Medic", "Shielded", "Everyone", "Nobody"});

            ShieldBreaks = new CustomToggleOption(num++, "Shield breaks on murder attempt", false);

            Snitch = new CustomHeaderOption(num++, "<color=#D4AF37FF>Snitch</color>");
            SnitchOnLaunch =
                new CustomToggleOption(num++, "Snitch knows who they are on Game Start", false);
            SnitchSeesNeutrals = new CustomToggleOption(num++, "Snitch sees neutral roles", false);
            SnitchTargetsCrew =
                new CustomToggleOption(num++, "Snitch prioritizes vanilla crew", false);

            Altruist = new CustomHeaderOption(num++, "<color=#660000FF>Altruist</color>");
            ReviveDuration =
                new CustomNumberOption(num++, "Altruist Revive Duration", 10, 1, 30, 1f, CooldownFormat);
            AltruistTargetBody =
                new CustomToggleOption(num++, "Target's body disappears on beginning of revive", false);

            TheGlitch =
                new CustomHeaderOption(num++, "<color=#00FF00FF>The Glitch</color>");
            MimicCooldownOption = new CustomNumberOption(num++, "Mimic Cooldown", 30, 10, 120, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, "Mimic Duration", 10, 1, 30, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, "Hack Cooldown", 30, 10, 120, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, "Hack Duration", 10, 1, 30, 1f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, "Glitch Hack Distance", new[] {"Short", "Normal", "Long"});

            Executioner =
                new CustomHeaderOption(num++, "<color=#8C4005FF>Executioner</color>");
            OnTargetDead = new CustomStringOption(num++, "Executioner becomes on Target Dead",
                new[] {"Crew", "Jester"});

            Arsonist = new CustomHeaderOption(num++, "<color=#FF4D00FF>Arsonist</color>");
            DouseCooldown =
                new CustomNumberOption(num++, "Douse Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            ArsonistGameEnd = new CustomToggleOption(num++, "Game keeps going so long as Arsonist is alive", false);

            Eraser = new CustomHeaderOption(num++, "<color=#FF33CC>Eraser</color>");
            EraseCooldown =
                new CustomNumberOption(num++, "Erase Cooldown", 30, 30, 60, 2.5f, CooldownFormat);
            EraserCrewPenalty =
                new CustomNumberOption(num++, "Eraser Extra Cooldown for erasing vanilla Crewmate", 2f, 1f, 4f, 1f, CooldownFormat);

            Morphling =
                new CustomHeaderOption(num++, "<color=#FF0000FF>Morphling</color>");
            MorphlingCooldown =
                new CustomNumberOption(num++, "Morphling Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, "Morphling Duration", 10, 5, 15, 1f, CooldownFormat);

            Miner = new CustomHeaderOption(num++, "<color=#FF0000FF>Miner</color>");

            MineCooldown =
                new CustomNumberOption(num++, "Mine Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            Swooper = new CustomHeaderOption(num++, "<color=#FF0000FF>Swooper</color>");

            SwoopCooldown =
                new CustomNumberOption(num++, "Swoop Cooldown", 25, 10, 40, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, "Swoop Duration", 10, 5, 15, 1f, CooldownFormat);

            Undertaker = new CustomHeaderOption(num++, "<color=#FF0000FF>Undertaker</color>");
            DragCooldown = new CustomNumberOption(num++, "Drag Cooldown", 25, 10, 40, 2.5f, CooldownFormat);

            Assassin = new CustomHeaderOption(num++, "<color=#FF0000FF>Assassin</color>");
            AssassinKills = new CustomNumberOption(num++, "Number of Assassin Kills", 1, 1, 5, 1);
            AssassinCrewmateGuess = new CustomToggleOption(num++, "Assassin can Guess \"Crewmate\"", false);
            AssassinGuessNeutrals = new CustomToggleOption(num++, "Assassin can Guess Neutral roles", false);
            AssassinMultiKill = new CustomToggleOption(num++, "Assassin can kill more than once per meeting");
        }
    }
}
