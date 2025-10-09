using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AnimationClipInspector : EditorWindow
{
    AnimationClip clip;

    public float time = 0f;
    AnimationClip instantiatedClip;


    [MenuItem("Tools/Animation Clip Inspector")]
    static void Open()
    {
        AnimationClipInspector clip = GetWindow<AnimationClipInspector>();
    }

    Vector2 scroll;

    Dictionary<string, bool> pathLabels;
    List<EditorCurveBinding> curveBinding;
    List<AnimationCurve> curves;

    private void OnGUI()
    {
        clip = (AnimationClip)EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false);

        if (clip == null)
            return;

        if (GUILayout.Button("Instantiate Clip"))
        {
            instantiatedClip = Instantiate(clip);

            RebuildCurveBindings();
        }

        if (instantiatedClip == null)
            return;

        instantiatedClip.name = EditorGUILayout.TextField("New Clip Name", instantiatedClip.name);

        time = EditorGUILayout.Slider("NormalizedTime", time, 0f, instantiatedClip.length + 1f / instantiatedClip.frameRate);

        scroll = EditorGUILayout.BeginScrollView(scroll);

        float width = Screen.width - 32f;

        // STEP 2
        try
        {
            for (int i = 0; i < curveBinding.Count; i++)
            {
                bool exit = false;
                EditorGUILayout.BeginHorizontal();

                GUILayout.Button(curveBinding[i].path + ", " + curveBinding[i].propertyName + ", " + curveBinding[i].type.ToString(),
                    GUILayout.Width(width * 0.5f));
                GUILayout.Label("Value at " + time + ":  " + curves[i].Evaluate(time), GUILayout.Width(width * 0.25f));

                curves[i] = EditorGUILayout.CurveField(curves[i], GUILayout.Width(width * 0.25f));

                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Delete"))
                {
                    instantiatedClip.ClearCurves();
                    for (int j = 0; j < curveBinding.Count; j++)
                    {
                        /*float mainProperty = 
                        if (curveBinding[j].path == curveBinding[0].path &&
                            curveBinding[j].propertyName.Substring(cur )


                        instantiatedClip.SetCurve(curveBinding[j].path, curveBinding[j].type, curveBinding[j].propertyName,
                            curves[j]);*/
                    }

                    RebuildCurveBindings();
                    exit = true;
                }

                if (exit)
                    break;
            }
        }
        catch (System.Exception e)
        {
            EditorGUILayout.LabelField(e.ToString());
        }

        EditorGUILayout.EndScrollView();
    }

    private void RebuildCurveBindings()
    {
        curveBinding = new List<EditorCurveBinding>(AnimationUtility.GetCurveBindings(instantiatedClip));
        curves = new List<AnimationCurve>();
        for (int i = 0; i < curveBinding.Count; i++)
        {
            curves.Add(AnimationUtility.GetEditorCurve(instantiatedClip, curveBinding[i]));
        }
    }
}
