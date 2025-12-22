using MattrifiedGames.MenuSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RectLayoutHelper : EditorWindow
{
    [MenuItem("Tools/Rect Layout Helper")]
    static void Create()
    {
        GetWindow<RectLayoutHelper>();
    }

    List<RectTransform> children = new List<RectTransform>();

    List<MenuElementBase> menuElements = new List<MenuElementBase>();

    public Vector2 minOffset;
    public Vector2 maxOffset;

    GameObject go;

    private void OnGUI()
    {
        if (Selection.activeGameObject != null)
        {
            if (GUILayout.Button("Build"))
            {
                go = Selection.activeGameObject;

                children = new List<RectTransform>();
                foreach (Transform t in Selection.activeGameObject.transform)
                {
                    if (t is RectTransform)
                        children.Add((RectTransform)t);
                }

                menuElements = new List<MenuElementBase>();
                foreach (Transform t in Selection.activeGameObject.transform)
                {
                    MenuElementBase meb = t.GetComponent<MenuElementBase>();
                    if (meb != null)
                        menuElements.Add(meb);
                }
            }
        }

        if (go == null)
        {
            return;
        }

        GUILayout.Label("Building for:  " + go.name);

        minOffset = EditorGUILayout.Vector2Field("Min Offset", minOffset);
        maxOffset = EditorGUILayout.Vector2Field("Max Offset", maxOffset);

        if (children == null || children.Count <= 0)
            return;

        if (GUILayout.Button("Hori"))
        {
            float min = 1f / children.Count;

            for (int i = 0; i < children.Count; i++)
            {
                children[i].anchorMin = new Vector2(i * min, 0);
                children[i].anchorMax = new Vector2((i + 1) * min, 1);

                children[i].offsetMin = minOffset;
                children[i].offsetMax = -maxOffset;
            }
        }

        if (GUILayout.Button("Vert - Top to Bottom"))
        {
            float min = 1f / children.Count;

            for (int i = 0; i < children.Count; i++)
            {
                children[i].anchorMin = new Vector2(0, (children.Count - 1 - i) * min);
                children[i].anchorMax = new Vector2(1, ((children.Count - i)) * min);

                children[i].offsetMin = minOffset;
                children[i].offsetMax = -maxOffset;
            }
        }

        if (GUILayout.Button("Vert - Bottom to Top"))
        {
            float min = 1f / children.Count;

            for (int i = 0; i < children.Count; i++)
            {
                children[i].anchorMin = new Vector2(0, i * min);
                children[i].anchorMax = new Vector2(1, (i + 1) * min);

                children[i].offsetMin = minOffset;
                children[i].offsetMax = -maxOffset;
            }
        }

        if (GUILayout.Button("Menu Elements Left Right"))
        {
            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].upSibling = null;
                menuElements[i].downSibling = null;

                menuElements[i].leftSibling = menuElements[i - 1 < 0 ? menuElements.Count - 1 : i - 1];
                menuElements[i].rightSibling = menuElements[i + 1 >= menuElements.Count ? 0 : i + 1];
            }
        }

        if (GUILayout.Button("Menu Elements Top Bottom"))
        {
            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].leftSibling = null;
                menuElements[i].rightSibling = null;

                menuElements[i].upSibling = menuElements[i - 1 < 0 ? menuElements.Count - 1 : i - 1];
                menuElements[i].downSibling = menuElements[i + 1 >= menuElements.Count ? 0 : i + 1];
            }
        }
    }
}
