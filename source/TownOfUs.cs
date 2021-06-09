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

			JanitorClean = CreateSprite("TownOfUs.Resources.Janitor.png");
			EngineerFix = CreateSprite("TownOfUs.Resources.Engineer.png");
			SwapperSwitch = CreateSprite("TownOfUs.Resources.SwapperSwitch.png");
			SwapperSwitchDisabled = CreateSprite("TownOfUs.Resources.SwapperSwitchDisabled.png");
			Shift = CreateSprite("TownOfUs.Resources.Shift.png");
			Footprint = CreateSprite("TownOfUs.Resources.Footprint.png");
			Rewind = CreateSprite("TownOfUs.Resources.Rewind.png");
			NormalKill = CreateSprite("TownOfUs.Resources.NormalKill.png");
			GreyscaleKill = CreateSprite("TownOfUs.Resources.GreyscaleKill.png");
			ShiftKill = CreateSprite("TownOfUs.Resources.ShiftKill.png");
			MedicSprite = CreateSprite("TownOfUs.Resources.Medic.png");
			SeerSprite = CreateSprite("TownOfUs.Resources.Seer.png");
			SampleSprite = CreateSprite("TownOfUs.Resources.Sample.png");
			MorphSprite = CreateSprite("TownOfUs.Resources.Morph.png");
			Camouflage = CreateSprite("TownOfUs.Resources.Camouflage.png");
			Arrow = CreateSprite("TownOfUs.Resources.Arrow.png");
			Abstain = CreateSprite("TownOfUs.Resources.Abstain.png");
			MineSprite = CreateSprite("TownOfUs.Resources.Mine.png");
			SwoopSprite = CreateSprite("TownOfUs.Resources.Swoop.png");
			DouseSprite = CreateSprite("TownOfUs.Resources.Douse.png");
			IgniteSprite = CreateSprite("TownOfUs.Resources.Ignite.png");
			ReviveSprite = CreateSprite("TownOfUs.Resources.Revive.png");
			ButtonSprite = CreateSprite("TownOfUs.Resources.Button.png");
			CycleSprite = CreateSprite("TownOfUs.Resources.Cycle.png");
			GuessSprite = CreateSprite("TownOfUs.Resources.Guess.png");

            PalettePatch.Load();
			ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();
			
			this._harmony.PatchAll();
		}

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
			var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, (float) tex.width, (float) tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
			return sprite.DontUnload();
		}

        private static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
		{
			_iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
			var il2CPPArray = (Il2CppStructArray<byte>) data;
			_iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
		}

		private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

		private static DLoadImage _iCallLoadImage;
		
		private Harmony _harmony; 
	}
}
