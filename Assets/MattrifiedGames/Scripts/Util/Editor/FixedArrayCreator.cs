using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FixedArrayCreator : EditorWindow
{
    public string className;
    public int count = 8;

    readonly string mainBase = "using UnityEngine;\npublic struct {0}{5}{1}public {2}\nthis[int i]{5}get{5}switch(i){5}{3}{6}{6}set{5}switch(i){5}{4}{6}{6}{6}{6}";

    [MenuItem("Window/FixedArrayCreator")]
    public static void GetWindow()
    {
        GetWindow<FixedArrayCreator>();
    }

    private void OnGUI()
    {
        className = EditorGUILayout.TextField("Class Name", className);
        count = EditorGUILayout.IntField("Count", count);

        if (GUILayout.Button("Build"))
        {
            string _0 = className + "_" + count;

            string _1 = string.Empty;

            for (int i = 0; i < count; i++)
            {
                _1 += "\n\t private " + className + " v_" + i + ";";
            }

            string _2 = string.Empty;

            for (int i = 0; i < count; i++)
            {
                _2 += "\n\t case " + i + ":  return v_" + i + ";";
            }
            _2 += "default:  Debug.LogError(\"Out of Range.  Returning 0 item\"); return v_" + 0 + ";";

            string _3 = string.Empty;

            for (int i = 0; i < count; i++)
            {
                _3 += "\n\t case " + i + ":  v_" + i + " = value; break;";
            }
            _3 += "default:  Debug.LogError(\"Out of Range\"); break;";

            string finalString = string.Format(mainBase, _0, _1, className, _2, _3, '{', '}');

            Debug.Log(finalString);

            MattrifiedGames.Utility.UnityFileWriter.WriteFile("", _0 + ".cs", finalString);

            AssetDatabase.Refresh();
        }
    }
}
