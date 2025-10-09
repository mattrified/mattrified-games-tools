using MattrifiedGames.ManagedAnimation;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ExpressionManager))]
public class ExpressionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        ExpressionManager em = target as ExpressionManager;

        if (GUILayout.Button("Open Editor Window"))
        {
            var bsw = EditorWindow.GetWindow<BlendShapeWindow>();
            bsw.AssignEM(em);
        }

        base.OnInspectorGUI();

        
    }
}

public class BlendShapeWindow : EditorWindow
{
    public ExpressionManager em;
    public Vector2 vec;

    public void AssignEM(ExpressionManager em)
    {
        this.em = em;
    }

    private void OnGUI()
    {
        if (em == null)
            return;

        if (GUILayout.Button("Select EM - " + em.name))
        {
            Selection.activeGameObject = em.gameObject;
        }

        if (em.otherTransforms != null && em.otherTransforms.Length > 0)
        {
            if (GUILayout.Button("Select Transform"))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < em.otherTransforms.Length;i++)
                {
                    if (em.otherTransforms[i] == null)
                        continue;

                    int index = i;
                    menu.AddItem(new GUIContent(em.otherTransforms[i].name), Selection.activeGameObject == em.otherTransforms[index].gameObject, () =>
                    { Selection.activeGameObject = em.otherTransforms[index].gameObject; });
                }

                // display the menu
                menu.ShowAsContext();
            }
        }

        vec = EditorGUILayout.BeginScrollView(vec);
        foreach (SkinnedMeshRenderer smr in em.skinnedRenderers)
        {
            if (GUILayout.Button(smr.name))
            {
                // create the menu and add items to it
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < em.otherTransforms.Length; i++)
                {
                    if (em.otherTransforms[i] == null)
                        continue;

                    menu.AddItem(new GUIContent("Select"), Selection.activeGameObject == smr.gameObject, () =>
                    { Selection.activeGameObject = smr.gameObject; });

                    menu.AddItem(new GUIContent("Reset Blend"),
                        false,
                        () =>
                        {
                            for (int m = 0; m < smr.sharedMesh.blendShapeCount; m++)
                            {
                                smr.SetBlendShapeWeight(m, 0f);
                            }
                        });
                }

                // display the menu
                menu.ShowAsContext();
            }

            var mesh = smr.sharedMesh;

            for (int i = 0; i < mesh.blendShapeCount; i += 2)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                var n0 = mesh.GetBlendShapeName(i);
                var n1 = string.Empty;
                if (i + 1 < mesh.blendShapeCount)
                {
                    n1 = mesh.GetBlendShapeName(i + 1);
                }
                EditorGUILayout.LabelField(n0);
                EditorGUILayout.LabelField(n1);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();


                em.skinnedRenderers[0].SetBlendShapeWeight(i, EditorGUILayout.Slider(smr.GetBlendShapeWeight(i), 0f, 100f));
                if (!string.IsNullOrEmpty(n1))
                {
                    em.skinnedRenderers[0].SetBlendShapeWeight(i + 1, EditorGUILayout.Slider(smr.GetBlendShapeWeight(i + 1), 0f, 100f));
                }
                else
                {
                    EditorGUILayout.LabelField(n1);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUILayout.EndScrollView();
    }
}
