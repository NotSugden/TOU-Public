using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using Reactor.Extensions;
using Reactor.Unstrip;
using TownOfUs.CustomOption;
using TownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace TownOfUs
{
    [BepInPlugin("com.slushiegoose.townofus", "Town Of Us", Version)]
    [BepInDependency(ReactorPlugin.Id)]
    public class TownOfUs : BasePlugin
    {
        public const string Version = "4.0.0-dev5";
        public ConfigEntry<string> Ip { get; set; }
        public ConfigEntry<ushort> Port { get; set; }

        public static Sprite JanitorClean;
        public static Sprite EngineerFix;
        public static Sprite SwapperSwitch;
        public static Sprite SwapperSwitchDisabled;
        public static Sprite Shift;
        public static Sprite Footprint;
        public static Sprite Rewind;
        public static Sprite NormalKill;
        public static Sprite GreyscaleKill;
        public static Sprite ShiftKill;
        public static Sprite MedicSprite;
        public static Sprite SeerSprite;
        public static Sprite SampleSprite;
        public static Sprite MorphSprite;
        public static Sprite Camouflage;
        public static Sprite Arrow;
        public static Sprite Abstain;
        public static Sprite MineSprite;
        public static Sprite SwoopSprite;
        public static Sprite DouseSprite;
        public static Sprite IgniteSprite;
        public static Sprite ReviveSprite;
        public static Sprite ButtonSprite;
        public static Sprite CycleSprite;
        public static Sprite GuessSprite;

        public override void Load()
        {
            this._harmony = new Harmony("com.slushiegoose.townofus");

            CustomOption.Generate.GenerateAll();

            JanitorClean = LoadResource("Janitor");
            EngineerFix = LoadResource("Engineer");
            SwapperSwitch = LoadResource("SwapperSwitch");
            SwapperSwitchDisabled = LoadResource("SwapperSwitchDisabled");
            Shift = LoadResource("Shift");
            Footprint = LoadResource("Footprint");
            Rewind = LoadResource("Rewind");
            NormalKill = LoadResource("NormalKill");
            GreyscaleKill = LoadResource("GreyscaleKill");
            ShiftKill = LoadResource("ShiftKill");
            MedicSprite = LoadResource("Medic");
            SeerSprite = LoadResource("Seer");
            SampleSprite = LoadResource("Sample");
            MorphSprite = LoadResource("Morph");
            Camouflage = LoadResource("Camouflage");
            Arrow = LoadResource("Arrow");
            Abstain = LoadResource("Abstain");
            MineSprite = LoadResource("Mine");
            SwoopSprite = LoadResource("Swoop");
            DouseSprite = LoadResource("Douse");
            IgniteSprite = LoadResource("Ignite");
            ReviveSprite = LoadResource("Revive");
            ButtonSprite = LoadResource("Button");
            CycleSprite = LoadResource("Cycle");
            GuessSprite = LoadResource("Guess");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            this._harmony.PatchAll();
        }

        private static Sprite LoadResource(string name) => CreateSprite($"TownOfUs.Resources.{name}.png");

        public static Sprite CreateSprite(string name, bool hat = false, bool newHat = false)
        {
            var pixelsPerUnit = (hat && !newHat) ? 225f : 100f;
            var pivot = (hat && !newHat) ? new Vector2(0.5f, 0.8f) : new Vector2(0.5f, 0.5f);

            var dimensions = newHat ? 128 : 0;
            var assembly = Assembly.GetExecutingAssembly();
            var tex = GUIExtensions.CreateEmptyTexture(dimensions, dimensions);
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, (float)tex.width, (float)tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite.DontUnload();
        }

        private static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>)data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        private static DLoadImage _iCallLoadImage;

        private Harmony _harmony;
    }
}
