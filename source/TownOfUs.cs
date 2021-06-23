using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using HarmonyLib;
using Reactor;
using TownOfUs.RainbowMod;
using UnhollowerBaseLib;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TownOfUs
{
    [BepInPlugin("com.slushiegoose.townofus", "Town Of Us", Version)]
    [BepInDependency(ReactorPlugin.Id)]
    public class TownOfUs : BasePlugin
    {
        public const string Version = "4.0.0-dev30";
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
        public static Sprite DragSprite;
        public static Sprite DropSprite;

        public static void LogMessage(object message)
        {
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage(message);
        }

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
            DragSprite = LoadResource("Drag");
            DropSprite = LoadResource("Drop");

            PalettePatch.Load();
            ClassInjector.RegisterTypeInIl2Cpp<RainbowBehaviour>();

            this._harmony.PatchAll();

/*
            var customServer = new ServerInfo("CustomServer-1", "among-us.sugden.cf", 22023);
            var customRegion = new DnsRegionInfo(customServer.Ip, "Custom Servers", StringNames.NoTranslation, new ServerInfo[] {
                customServer
            }).Cast<IRegionInfo>();
            var newRegions = new IRegionInfo[] { customRegion };

            ServerManager.DefaultRegions = ServerManager.DefaultRegions.Concat(newRegions).ToArray();
            var serverManager = ServerManager.Instance;
            serverManager.AvailableRegions = ServerManager.DefaultRegions;
            serverManager.CurrentRegion = customRegion;
            serverManager.CurrentServer = customServer;
            serverManager.SaveServers();*/

            SceneManager.add_sceneLoaded(
                (UnityEngine.Events.UnityAction<Scene, LoadSceneMode>)delegate (Scene scene, LoadSceneMode loadSceneMode)
                {
                    DestroyableSingleton<ModManager>.Instance.ShowModStamp();
                }
            );
        }

        private static Sprite LoadResource(string name) => CreateSprite($"TownOfUs.Resources.{name}.png");

        /*public static Sprite CreateSprite(string name, bool hat = false, bool newHat = false)
        {
            var pixelsPerUnit = hat ? 225f : 100f;
            var pivot = (hat && !newHat) ? new Vector2(0.5f, 0.8f) : new Vector2(0.5f, 0.5f);

            var dimensions = newHat ? 128 : 0;
            var assembly = Assembly.GetExecutingAssembly();
            var tex = new Texture2D(dimensions, dimensions, TextureFormat.Alpha8, false);
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = newHat
                ? Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot)
                : Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite.DontUnload();
        }*/

        public static Sprite CreateSprite(string name, bool hat = false)
        {
            var pixelsPerUnit = hat ? 225f : 100f;
            var pivot = hat ? new Vector2(0.5f, 0.8f) : new Vector2(0.5f, 0.5f);
            var executingAssembly = Assembly.GetExecutingAssembly();
            var texture2D = Extensions.CreateEmptyTexture(0, 0);
            var manifestResourceStream = executingAssembly.GetManifestResourceStream(name);
            var data = manifestResourceStream.ReadFully();
            LoadImage(texture2D, data, true);
            texture2D.DontDestroy();
            var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), pivot, pixelsPerUnit);
            sprite.DontDestroy();
            return sprite;
        }

        public static Sprite CreatePolusHat(string name)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var manifestResourceStream = executingAssembly.GetManifestResourceStream(name);
            var data = manifestResourceStream.ReadFully();
            var texture2D = new Texture2D(128, 128, TextureFormat.Alpha8, false);
            LoadImage(texture2D, data, false);
            texture2D.DontDestroy();
            var sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            sprite.DontDestroy();
            return sprite;
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
