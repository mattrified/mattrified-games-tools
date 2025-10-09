using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextToTextMeshProUGUI : EditorWindow
{
    [MenuItem("Tools/Text to Text Mesh Pro UGUI")]
    public static void GetWindow()
    {
        GetWindow<TextToTextMeshProUGUI>();
    }

    Vector2 scroll;

    public List<Text> textList = new List<Text>();
    public List<TextMeshProUGUI> tmpList = new List<TextMeshProUGUI>();

    private void OnGUI()
    {
        if (GUILayout.Button("Build List"))
        {
            textList = new List<Text>();
            tmpList = new List<TextMeshProUGUI>();

            GameObject[] objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < objs.Length; i++)
            {
                Text[] tt = objs[i].GetComponentsInChildren<Text>(true);
                for (int j = 0; j < tt.Length; j++)
                {
                    if (textList.Contains(tt[j]))
                        continue;
                    textList.Add(tt[j]);
                }

                TextMeshProUGUI[] tmps = objs[i].GetComponentsInChildren<TextMeshProUGUI>(true);
                for (int j = 0; j < tmps.Length; j++)
                {
                    if (tmpList.Contains(tmps[j]))
                        continue;
                    tmpList.Add(tmps[j]);
                }
            }
        }


        scroll = EditorGUILayout.BeginScrollView(scroll);
        for (int i = 0; i < textList.Count; i++)
        {
            if (textList[i] == null)
            {
                textList.RemoveAt(i);
                i--;
                continue;
            }

            GUILayout.Label(textList[i].name);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Select"))
                Selection.activeGameObject = textList[i].gameObject;
            if (GUILayout.Button("Replace Text"))
            {
                var fontSize = textList[i].fontSize;
                var alignement = textList[i].alignment;
                var txt = textList[i].text;
                var color = textList[i].color;

                GameObject go = textList[i].gameObject;
                DestroyImmediate(textList[i]);

                var tmp = go.AddComponent<TextMeshProUGUI>();
                tmp.fontSize = fontSize;
                tmp.text = txt;
                tmp.color = color;

                switch (alignement)
                {
                    case TextAnchor.LowerCenter: tmp.alignment = TextAlignmentOptions.Bottom; break;
                    case TextAnchor.LowerLeft: tmp.alignment = TextAlignmentOptions.BottomLeft; break;
                    case TextAnchor.LowerRight: tmp.alignment = TextAlignmentOptions.BottomRight; break;

                    case TextAnchor.MiddleCenter: tmp.alignment = TextAlignmentOptions.Midline; break;
                    case TextAnchor.MiddleLeft: tmp.alignment = TextAlignmentOptions.MidlineLeft; break;
                    case TextAnchor.MiddleRight: tmp.alignment = TextAlignmentOptions.MidlineRight; break;

                    case TextAnchor.UpperCenter: tmp.alignment = TextAlignmentOptions.Capline; break;
                    case TextAnchor.UpperLeft: tmp.alignment = TextAlignmentOptions.CaplineLeft; break;
                    case TextAnchor.UpperRight: tmp.alignment = TextAlignmentOptions.CaplineRight; break;
                }

                textList.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(32);
        }

        GUILayout.Label("Text Mesh Pro Objects");

        for (int i = 0; i < tmpList.Count; i++)
        {
            if (GUILayout.Button(tmpList[i].name))
            {
                Selection.activeGameObject = tmpList[i].gameObject;
            }
        }
        

        EditorGUILayout.EndScrollView();
    }
}
