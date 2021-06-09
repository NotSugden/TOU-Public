using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Rewired;
using UnhollowerBaseLib;
using UnityEngine;

namespace TownOfUs.CustomHats
{
    public class HatCreation
    {
        private static bool modded = false;


        protected internal struct HatData
        {
            public bool bounce;
            public string name;
            public bool highUp;
            public Vector2 offset;
            public string author;
            public bool new_hat;
        }

        private static List<HatData> _hatDatas = new List<HatData>()
        {
            new HatCreation.HatData
            { name = "5up", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },
            new HatCreation.HatData
            { name = "aria", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true },
            new HatCreation.HatData
            { name = "artisthat", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "ash hat", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "bimbus", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "bingus", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "birb", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "blaustoise", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "boxhat", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "brizzyears", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "carrot", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "cheyLR", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "corpse", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "couve", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "crumb", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "dream", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "eye", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "frenchtoast", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "gh00stie_f", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "gorillaphent_f", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "janet", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "jodi", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "kris", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "leah", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "lightbulb", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "lily", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "ludwig", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "masayoshi", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "mika", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "miyoung", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "pancakes", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "Paule_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "penguin", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "penguinface_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "peter", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "poki", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "rae", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "rayc", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "revmeerkat_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "steampunk_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "toast", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "vince_foot_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "Vince_front", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "Voteme", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "waffle", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "wendy", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "wrench_f", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "Zemo_hat", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "ZeroXFusionz", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "_", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "shiki", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "rocky", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "pepe", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "jerm", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "frog", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "squirrel", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "crunchy", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "charizard", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "br00d", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "bird", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "", new_hat = true
            },new HatCreation.HatData
            { name = "basetrade_3", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "Ressnie", new_hat = true
            },new HatCreation.HatData
            { name = "basetrade_2", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "Ressnie", new_hat = true
            },new HatCreation.HatData
            { name = "basetrade_1", bounce = false, offset = new Vector2(-0.1f, 0.45f), author = "Ressnie", new_hat = true
            },new HatCreation.HatData
            { name = "glitch", bounce = false, highUp = false, offset = new Vector2(0f, 0.1f), author = "PhasmoFireGod"
            },new HatCreation.HatData
            { name = "firegod", bounce = false, highUp = false, offset = new Vector2(0.1f, 0.1f), author = "PhasmoFireGod"
            },new HatCreation.HatData
            { name = "giacomo", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin", new_hat = true
            },new HatCreation.HatData
            { name = "rhythian", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "ravs", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "nilesy", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "clara", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.35f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "boba", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "sips", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "ped", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "lewis", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "duncan", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "ben", bounce = false, highUp = false, offset = new Vector2(0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "tomcraven", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "dawko", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "dyto", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "betsy", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "justjames", bounce = false, highUp = false, offset = new Vector2(0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "raflp", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "lawhoo", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.4f), author = "LaWhooligan"
            },new HatCreation.HatData
            { name = "jose", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "kara", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.4f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "jamobo", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.35f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "garbage", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.45f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "axilla", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "brittzey", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "pastaroni", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "ozza", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "mindy", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "Ressnie"
            },new HatCreation.HatData
            { name = "ash_2", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "ash_1", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "aphex", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "junkyard", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "cheesy", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "shubble", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "aplatypuss", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "zeroyalviking", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "chilledchaos", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.2f), author = "Nassegris"
            },new HatCreation.HatData
            { name = "phoebe", bounce = true, highUp = false, offset = new Vector2(-0.1f, 0.5f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "toby", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "harrie", bounce = true, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "razzbowski", bounce = true, highUp = true, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "kay", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "zylus", bounce = true, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "anniefuschia", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "annamaja", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "nerdout", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "Taamoy"
            },new HatCreation.HatData
            { name = "bloody", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "ellum", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "stumpy", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "breeh", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.2f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "vikram_1", bounce = true, highUp = true, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "vikram_2", bounce = true, highUp = true, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "dizzilulu", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.3f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "freyzplayz", bounce = true, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "lexiemarie", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "slushie", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "falcone_1", bounce = true, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "wolfabelle", bounce = true, highUp = false, offset = new Vector2(-0.1f, 0.4f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "bisexual", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "asexual", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "gay", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "pansexual", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "nonbinary", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "trans_1", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "trans_4", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.5f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "trans_3", bounce = false, highUp = false, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            },new HatCreation.HatData
            { name = "trans_2", bounce = false, highUp = true, offset = new Vector2(-0.1f, 0.1f), author = "TheLastShaymin"
            }
        };

        public static List<uint> TallIds = new List<uint>();

        protected internal static Dictionary<uint, HatData> IdToData = new Dictionary<uint, HatData>();

        private static HatBehaviour CreateHat(HatData hat, int id)
        {
            var rsPath = $"TownOfUs.Resources.Hats.hat_{hat.name}.png";
            var sprite = TownOfUs.CreateSprite(rsPath, true, hat.new_hat);
            var newHat = ScriptableObject.CreateInstance<HatBehaviour>();
            newHat.MainImage = sprite;
            newHat.ProductId = hat.name;
            newHat.Order = 99 + id;
            newHat.InFront = true;
            newHat.NoBounce = !hat.bounce;
            newHat.ChipOffset = hat.offset;

            return newHat;
        }

        private static IEnumerable<HatBehaviour> CreateAllHats()
        {

            var i = 0;
            foreach (var hat in _hatDatas)
            {
                yield return CreateHat(hat, ++i);
            }
        }

        [HarmonyPatch(typeof(HatManager), nameof(HatManager.GetHatById))]
        public static class HatManagerPatch
        {
            static bool Prefix(HatManager __instance)
            {
                try
                {
                    if (!modded)
                    {
                        modded = true;
                        var id = 0;
                        foreach (var hatData in _hatDatas)
                        {
                            var hat = CreateHat(hatData, id++);
                            __instance.AllHats.Add(hat);
                            if (hatData.highUp)
                            {
                                TallIds.Add((uint)(__instance.AllHats.Count - 1));
                            }
                            IdToData.Add((uint)__instance.AllHats.Count - 1, hatData);
                        }
                    }
                    return true;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("During Prefix, an exception occured");
                    System.Console.WriteLine("------------------------------------------------");
                    System.Console.WriteLine(e);
                    System.Console.WriteLine("------------------------------------------------");
                    throw;
                }
            }
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetHat))]
        public static class PlayerControl_SetHat
        {
            public static void Postfix(PlayerControl __instance, uint __0, int __1)
            {
                __instance.nameText.transform.localPosition = new Vector3(
                    0f,
                    __0 == 0U ? 0.7f : TallIds.Contains(__0) ? 1.2f : 1.05f,
                    -0.5f
                );
            }
        }
        
        
    }
}
