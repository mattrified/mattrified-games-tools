#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

namespace MattrifiedGames.Utility
{
    public static class AssetFinder
    {
        public static List<T> LoadAllAssetOfType<T>(params string[] folders) where T : UnityEngine.Object
        {
            List<T> list = new List<T>();

            string[] guids;
            if (folders == null || folders.Length == 0)
                guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            else
                guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, folders);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object[] loadedObjects = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int i = 0; i < loadedObjects.Length; i++)
                {
                    if (loadedObjects[i] is T)
                    {
                        T result = (T)loadedObjects[i];
                        if (list.Contains(result))
                            continue;
                        list.Add(result);
                    }
                }
            }

            return list;
        }
    }
}
#endif