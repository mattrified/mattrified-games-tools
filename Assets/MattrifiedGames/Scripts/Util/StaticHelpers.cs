using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MattrifiedGames.Utility
{
    public static class StaticHelpers
    {
        public static T GetItemFromListSafe<T>(int index, List<T> list)
        {
            if (list == null || list.Count == 0)
                return default(T);

            return list[SafeModolu(index, list.Count)];
        }

        internal static Rect LerpRect(Rect startValue, Rect endValue, float curveTime)
        {
            Rect r = startValue;
            r.min = Vector2.Lerp(startValue.min, endValue.min, curveTime);
            r.max = Vector2.Lerp(startValue.max, endValue.max, curveTime);

            return r;
        }

        public static Gradient WhiteGradient()
        {
            Gradient g = new Gradient()
            {
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1,0),
                    new GradientAlphaKey(1, 1)
                },

                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.white, 0),
                    new GradientColorKey(Color.white, 0)
                }
            };

            return g;
            
        }

        public static T[] AddToArray<T>(T[] oldArray, T newMember)
        {
            T[] newArray = new T[oldArray.Length + 1];
            oldArray.CopyTo(newArray, 0);
            newArray[oldArray.Length] = newMember;
            return newArray;
        }

        internal static T[] CreateArrayDefault<T>(int v1, T v2)
        {
            T[] newT = new T[v1];
            for (int i = 0; i < v1; i++)
                newT[i] = v2;
            return newT;
        }

        internal static bool SetChildText(Behaviour img, string v)
        {
            Text t = img.GetComponentInChildren<Text>();
            if (t != null)
            {
                t.text = v;
                return true;
            }

            return false;
        }

        public static T GetItemFromArraySafe<T>(int index, T[] list)
        {
            if (list == null || list.Length == 0)
                return default(T);

            return list[SafeModolu(index, list.Length)];
        }

        public static void RemoveFromArrayAt<T>(ref T[] array, int i)
        {
            if (i >= 0 && i < array.Length)
            {
                List<T> t = new List<T>(array);
                t.RemoveAt(i);
                array = t.ToArray();
            }
        }

        internal static int SafeModolu(int currentIndex, int len)
        {
            return (len + (currentIndex % len)) % len;
        }

        public static bool ByteMaskCheck(int mask, int bit)
        {
            return (mask & bit) == bit;
        }

        public static bool ByteMaskCheck(long mask, long bit)
        {
            return (mask & bit) == bit;
        }

        public static string CleanResourcePath(string path)
        {
            int index = path.IndexOf("Resources/");
            if (index >= 0)
            {
                path = path.Substring(index + 10);
            }

            int lastPeriod = path.LastIndexOf('.');
            if (lastPeriod >= 0)
                path = path.Substring(0, lastPeriod);

            return path;
        }

        public static int ByteMaskCheckInt(int mask, int bit)
        {
            return (ByteMaskCheck(mask, bit) ? 1 : 0);
        }

        public static int SafeLoop(int currentIndex, int min, int max)
        {
            if (currentIndex < min)
                return max;
            else if (currentIndex > max)
                return min;
            return currentIndex;
        }

        /// <summary>
        /// Destroys a given object safely
        /// </summary>
        /// <param name="obj">The object being destroyed</param>
        public static void SafeDestroy<T>(ref T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return;

            if (Application.isEditor && !Application.isPlaying)
                UnityEngine.Object.DestroyImmediate(obj);
            else
                UnityEngine.Object.Destroy(obj);
            obj = null;
        }

        public static bool FindChildByName(Transform t, string name, out Transform result)
        {
            result = null;
            if (t.name == name)
            {
                result = t;
                return true;
            }

            foreach (Transform child in t)
            {
                if (FindChildByName(child, name, out result))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Sets the specified object as dirty but will only work if we are in the editor.  Otherwise, a warning is thrown.
        /// </summary>
        /// <param name="obj"></param>
        internal static void SetDirty(UnityEngine.Object obj)
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(obj);
#else
            Debug.LogWarning("Can only set dirty in editor.");
#endif
        }
    }
}

