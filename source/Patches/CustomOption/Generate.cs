// This folder is a Stripped down version of Reactor-Essentials
// Please use https://github.com/DorCoMaNdO/Reactor-Essentials because it is more updated and less buggy

using System;
using System.Collections.Generic;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        public static Func<object, string> PercentFormat { get; } = (value) => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = (value) => $"{value:0.0#}s";

        public static CustomHeaderOption CrewmateRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption TimeLordOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption SeerOn;
        public static CustomNumberOption SpyOn;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption ButtonBarryOn;

        public static CustomHeaderOption NeutralRoles;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption ShifterOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption PhantomOn;

        public static CustomHeaderOption ImpostorRoles;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption CamouflagerOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption AssassinOn;
        public static CustomNumberOption UndertakerOn;

        public static CustomHeaderOption Modifiers;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption FlashOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption DrunkOn;
        public static CustomNumberOption BigBoiOn;

        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption MeetingColourblind;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption MaxImpostorRoles;
        public static CustomNumberOption MaxNeutralRoles;
        public static CustomToggleOption RoleUnderName;
        public static CustomNumberOption VanillaGame;
        public static CustomToggleOption VentAnimations;

        public static CustomHeaderOption Mayor;
        public static CustomNumberOption MayorVoteBank;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;

        public static CustomHeaderOption Sheriff;
        public static CustomToggleOption ShowSheriff;
        public static CustomToggleOption SheriffKillsCrewmate;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffBodyReport;
        public static CustomToggleOption SheriffKeepsGameAliveOn2;
        public static CustomToggleOption SheriffAlwaysKeepsGameAlive;

        public static CustomHeaderOption Shifter;
        public static CustomNumberOption ShifterCd;
        public static CustomStringOption WhoShifts;

        public static CustomHeaderOption Engineer;
        public static CustomNumberOption EngineerFixes;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;

        public static CustomHeaderOption TimeLord;
        public static CustomToggleOption RewindRevive;
        public static CustomNumberOption RewindDuration;
        public static CustomNumberOption RewindCooldown;
        public static CustomToggleOption TimeLordVitals;

        public static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;

        public static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomStringOption SeerInfo;
        public static CustomStringOption SeeReveal;
        public static CustomToggleOption NeutralRed;

        public static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomStringOption GlitchHackDistanceOption;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption AssassinMaxKills;
        public static CustomNumberOption AssassinMaxPerMeeting;
        public static CustomToggleOption AssassinCanGuessCrewmate;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;

        public static CustomHeaderOption Camouflager;
        public static CustomNumberOption CamouflagerCooldown;
        public static CustomNumberOption CamouflagerDuration;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;

        public static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;

        public static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchOnLaunch;
        public static CustomToggleOption SnitchCanSeeNeutrals;

        public static CustomHeaderOption Altruist;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;
        public static CustomStringOption WhoSeesAltruistArrows;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;

        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomToggleOption ArsonistGameEnd;

        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new ImportExport(num++, "Save Custom Settings");
            Patches.ImportButton = new ImportExport(num++, "Load Custom Settings");

            CustomNumberOption PercentageOption(string color, string name, bool indent = true) =>
                new CustomNumberOption(
                    indent, num++,
                    color != null ? Utils.ColorText(color, name) : name,
                    0f, 0f, 100f, 10f,
                    PercentFormat
                );
            CustomNumberOption ImpostorOption(string name) => PercentageOption("FF0000FF", name);
            CustomToggleOption ToggleOption(string name, bool defaultValue) =>
                new CustomToggleOption(num++, name, defaultValue);
            CustomNumberOption NumberOptionMinValue(string name, float initialValue, float minValue, float maxValue, float increment = 1f) =>
                new CustomNumberOption(num++, name, initialValue, minValue, maxValue, increment);
            CustomNumberOption NumberOption(string name, float initialValue, float maxValue, float increment = 1f) =>
                NumberOptionMinValue(name, initialValue, 0f, maxValue, increment);
            CustomNumberOption CooldownOption(
                string name, float initialValue, float minValue, float maxValue, float increment = 1f
            ) => new CustomNumberOption(num++, name, initialValue, minValue, maxValue, increment, CooldownFormat);
            CustomStringOption StringOption(string name, string[] values) =>
                new CustomStringOption(num++, name, values);

            CrewmateRoles = new CustomHeaderOption(num++, "Crewmate Roles");
            MayorOn = PercentageOption("704FA8FF", "Mayor");
            LoversOn = PercentageOption("FF66CCFF", "Lovers");
            SheriffOn = PercentageOption("FFFF00FF", "Sheriff");
            EngineerOn = PercentageOption("FFA60AFF", "Engineer");
            SwapperOn = PercentageOption("66E666FF", "Swapper");
            InvestigatorOn = PercentageOption("00B3B3FF", "Investigator");
            TimeLordOn = PercentageOption("0000FFFF", "Time Lord");
            MedicOn = PercentageOption("006600FF", "Medic");
            SeerOn = PercentageOption("FFCC80FF", "Seer");
            SpyOn = PercentageOption("CCA3CCFF", "Spy");
            AltruistOn = PercentageOption("660000FF", "Altruist");

            NeutralRoles = new CustomHeaderOption(num++, "Neutral Roles");
            JesterOn = PercentageOption("FFBFCCFF", "Jester");
            ShifterOn = PercentageOption("999999FF", "Shifter");
            GlitchOn = PercentageOption("00FF00FF", "The Glitch");
            ExecutionerOn = PercentageOption("8C4005FF", "Executioner");
            ArsonistOn = PercentageOption("FF4D00FF", "Arsonist");
            PhantomOn = PercentageOption("662962", "Phantom");

            ImpostorRoles = new CustomHeaderOption(num++, "Impostor Roles");
            AssassinOn = ImpostorOption("Assassin");
            JanitorOn = ImpostorOption("Janitor");
            MorphlingOn = ImpostorOption("Morphling");
            CamouflagerOn = ImpostorOption("Camouflager");
            MinerOn = ImpostorOption("Miner");
            SwooperOn = ImpostorOption("Swooper");
			UndertakerOn = ImpostorOption("Undertaker");

            Modifiers = new CustomHeaderOption(num++, "Modifiers");
            SnitchOn = PercentageOption("D4AF37FF", "Snitch");
            TorchOn = PercentageOption("FFFF99FF", "Torch");
            DiseasedOn = PercentageOption("808080FF", "Diseased");
            FlashOn = PercentageOption("FF8080FF", "Flash");
            TiebreakerOn = PercentageOption("99E699FF", "Tiebreaker");
            DrunkOn = PercentageOption("758000FF", "Drunk");
            BigBoiOn = PercentageOption("FF8080FF", "Giant");
            ButtonBarryOn = PercentageOption("E600FFFF", "Button Barry");

            CustomGameSettings = new CustomHeaderOption(num++, "Custom Game Settings");
            ColourblindComms = ToggleOption("Camouflaged Comms", false);
            MeetingColourblind = ToggleOption("Camouflaged Meetings", false);
            ImpostorSeeRoles = ToggleOption("Impostors can see the roles of their team", false);
            DeadSeeRoles = ToggleOption("Dead can see everyone's roles", false);
            MaxImpostorRoles = NumberOption("Max Impostor Roles", 2f, 3f);
            MaxNeutralRoles = NumberOption("Max Neutral Roles", 2f, 5f);
            RoleUnderName = ToggleOption("Role Appears Under Name", true);
            VanillaGame = PercentageOption(null, "Probability of a vanilla game", false);
            VentAnimations = ToggleOption("Show Vent Animations", true);

            Mayor = new CustomHeaderOption(num++, Utils.ColorText("704FA8FF", "Mayor"));
            MayorVoteBank = NumberOptionMinValue("Initial Mayor Vote Bank", 1f, 1f, 5f);

            Lovers = new CustomHeaderOption(num++, Utils.ColorText("FF66CCFF", "Lovers"));
            BothLoversDie = ToggleOption("Both Lovers Die", true);

            Sheriff = new CustomHeaderOption(num++, Utils.ColorText("FFFF00FF", "Sheriff"));
            ShowSheriff = ToggleOption("Show Sheriff", false);
            SheriffKillsCrewmate = ToggleOption("Sheriff Miskill Kills Crewmate", false);
            SheriffKillsJester = ToggleOption("Sheriff Kills Jester", false);
            SheriffKillsArsonist = ToggleOption("Sheriff Kills Arsonist", false);
            SheriffBodyReport = ToggleOption("Sheriff can report who they've killed", true);
            SheriffKeepsGameAliveOn2 = ToggleOption("Sheriff can kill in final 2", true);
            SheriffAlwaysKeepsGameAlive = ToggleOption("Game doesn't end if Sheriff is alive", true);

            Engineer = new CustomHeaderOption(num++, Utils.ColorText("FFA60AFF", "Engineer"));
            EngineerFixes = NumberOptionMinValue("Max Engineer Fixes, 1 per round", 1f, 1f, 5f, 1f);

            Investigator = new CustomHeaderOption(num++, "<color=#00B3B3FF>Investigator</color>");
            FootprintSize = NumberOptionMinValue("Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval = CooldownOption("Footprint Interval", 1f, 0.25f, 5f, 0.25f);
            FootprintDuration = CooldownOption("Footprint Duration", 10f, 1f, 10f, 0.5f);
            AnonymousFootPrint = ToggleOption("Anonymous Footprint", false);
            VentFootprintVisible = ToggleOption("Footprint Vent Visible", false);

            TimeLord = new CustomHeaderOption(num++, "<color=#0000FFFF>Time Lord</color>");
            RewindRevive = ToggleOption("Revive During Rewind", false);
            RewindDuration = CooldownOption("Rewind Duration", 3f, 3f, 15f, 0.5f);
            RewindCooldown = CooldownOption("Rewind Cooldown", 25f, 10f, 40f, 2.5f);
            TimeLordVitals = ToggleOption("Time Lord can use Vitals", false);

            Medic = new CustomHeaderOption(num++, "<color=#006600FF>Medic</color>");
            ShowShielded = StringOption("Show Shielded Player", new[] { "Self", "Medic", "Self+Medic", "Everyone" });
            MedicReportSwitch = ToggleOption("Show Medic Reports", true);
            MedicReportNameDuration = CooldownOption("Time Where Medic Reports Will Have Name", 0, 0, 60, 2.5f);
            MedicReportColorDuration = CooldownOption("Time Where Medic Reports Will Have Color Type", 15, 0, 120, 2.5f);
            WhoGetsNotification = StringOption("Who gets murder attempt indicator", new[] { "Medic", "Shielded", "Everyone", "Nobody" });
            ShieldBreaks = ToggleOption("Shield breaks on murder attempt", false);

            Seer = new CustomHeaderOption(num++, "<color=#FFCC80FF>Seer</color>");
            SeerCooldown = CooldownOption("Seer Cooldown", 25f, 10f, 100f, 2.5f);
            SeerInfo = StringOption("Info that Seer sees", new[] { "Role", "Team" });
            SeeReveal = StringOption("Who Sees That They Are Revealed", new[] { "Crew", "Imps+Neut", "All", "Nobody" });
            NeutralRed = ToggleOption("Neutrals show up as Impostors", false);

            Snitch = new CustomHeaderOption(num++, "<color=#D4AF37FF>Snitch</color>");
            SnitchOnLaunch = ToggleOption("Snitch knows who they are on Game Start", false);
            SnitchCanSeeNeutrals = ToggleOption("Snitch can see Neutral Roles", false);

            Altruist = new CustomHeaderOption(num++, "<color=#660000FF>Altruist</color>");
            ReviveDuration = CooldownOption("Altruist Revive Duration", 10, 1, 30, 1f);
            AltruistTargetBody = ToggleOption("Target's body disappears on beginning of revive", false);
            WhoSeesAltruistArrows = StringOption("Who sees the alert arrows", new[] { "Impostors", "Glitch", "Imps+Glitch" });

            Shifter = new CustomHeaderOption(num++, "<color=#999999FF>Shifter</color>");
            ShifterCd = CooldownOption("Shifter Cooldown", 25f, 10f, 40f, 2.5f);
            WhoShifts = StringOption("Who gets the Shifter role on Shift", new[] { "NoImps", "RegCrew", "Nobody" });


            TheGlitch = new CustomHeaderOption(num++, "<color=#00FF00FF>The Glitch</color>");
            MimicCooldownOption = CooldownOption("Mimic Cooldown", 30, 10, 120, 2.5f);
            MimicDurationOption = CooldownOption("Mimic Duration", 10, 1, 30, 1f);
            HackCooldownOption = CooldownOption("Hack Cooldown", 30, 10, 120, 2.5f);
            HackDurationOption = CooldownOption("Hack Duration", 10, 1, 30, 1f);
            GlitchHackDistanceOption = StringOption("Glitch Hack Distance", new[] { "Short", "Normal", "Long" });

            Executioner = new CustomHeaderOption(num++, "<color=#8C4005FF>Executioner</color>");
            OnTargetDead = StringOption("Executioner becomes on Target Dead", new[] { "Crew", "Jester" });

            Arsonist = new CustomHeaderOption(num++, "<color=#FF4D00FF>Arsonist</color>");
            DouseCooldown = CooldownOption("Douse Cooldown", 25, 10, 40, 2.5f);
            ArsonistGameEnd = ToggleOption("Game keeps going so long as Arsonist and killer is alive", false);

            Assassin = new CustomHeaderOption(num++, "<color=#FF0000FF>Assassin</color>");
            AssassinMaxKills = NumberOption("Max Role Guess Kills", 2, 9, 1);
            AssassinMaxPerMeeting = NumberOption("Max Kills Per Meeting", 1, 9, 1);
            AssassinCanGuessCrewmate = ToggleOption("Can Guess 'Crewmate'", false);

            Morphling = new CustomHeaderOption(num++, "<color=#FF0000FF>Morphling</color>");
            MorphlingCooldown = CooldownOption("Morphling Cooldown", 25, 10, 40, 2.5f);
            MorphlingDuration = CooldownOption("Morphling Duration", 10, 5, 15, 1f);

            Camouflager = new CustomHeaderOption(num++, "<color=#FF0000FF>Camouflager</color>");
            CamouflagerCooldown = CooldownOption("Camouflager Cooldown", 25, 10, 40, 2.5f);
            CamouflagerDuration = CooldownOption("Camouflager Duration", 10, 5, 15, 1f);

            Undertaker = new CustomHeaderOption(num++, "<color=#FF0000FF>Undertaker</color>");
            DragCooldown = CooldownOption("Drag Cooldown", 25, 10, 60, 2.5f);

            Miner = new CustomHeaderOption(num++, "<color=#FF0000FF>Miner</color>");
            MineCooldown = CooldownOption("Mine Cooldown", 25, 10, 40, 2.5f);

            Swooper = new CustomHeaderOption(num++, "<color=#FF0000FF>Swooper</color>");
            SwoopCooldown = CooldownOption("Swoop Cooldown", 25, 10, 40, 2.5f);
            SwoopDuration = CooldownOption("Swoop Duration", 10, 5, 15, 1f);
        }
    }
}
