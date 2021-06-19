using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnhollowerBaseLib;

namespace TownOfUs
{
    public static class Extensions
    {
        public static T Find<T>(this Il2CppReferenceArray<T> list, Func<T, bool> comparison) where T : Il2CppObjectBase
        {
            foreach (var item in list)
            {
                if (comparison(item)) return item;
            }
            return null;
        }
        public static bool All<T>(this Il2CppReferenceArray<T> list, Func<T, bool> comparison) where T : Il2CppObjectBase
        {
            foreach (var elem in list)
            {
                if (!comparison(elem)) return false;
            }
            return true;
        }
        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);
            foreach (KeyValuePair<byte, int> keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                {
                    tie = true;
                }
            }
            return result;
        }

        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0)
        {
            return new Texture2D(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
        }

        public static byte[] ReadFully(this Stream input)
        {
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                result = memoryStream.ToArray();
            }
            return result;
        }

        public static T DontDestroy<T>(this T obj) where T : UnityEngine.Object
        {
            ref T ptr = ref obj;
            ptr.hideFlags |= HideFlags.HideAndDontSave;
            UnityEngine.Object.DontDestroyOnLoad(obj);
            return obj;
        }
    }
}
