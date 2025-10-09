using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorPreviewer : MonoBehaviour
{
    public Animator anim;

    [Delayed()]
    public string animName;

    public float time;

    float preTime;

    private void Update()
    {
        try
        {
            if (preTime != time)
            {
                anim.PlayInFixedTime(animName, 0, time / 60f);
                preTime = time;
                anim.Update(0);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(AnimatorPreviewer))]
public class AnimatorPreviewerEditor : UnityEditor.Editor
{
    public string[] animations;


    private void OnEnable()
    {
        AnimatorPreviewer ap = target as AnimatorPreviewer;

        UnityEditor.Animations.AnimatorController ac = ap.anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;

        List<string> states = new List<string>();
        GetStateNames(ac.layers[0].stateMachine, states);

        animations = states.ToArray();
    }

    public override void OnInspectorGUI()
    {
        AnimatorPreviewer ap = target as AnimatorPreviewer;
        for (int i = 0; i < animations.Length; i++)
        {
            if (i % 5 == 0)
            {
                GUILayout.BeginHorizontal();
            }

            var c = GUI.color;
            GUI.color = ap.animName == animations[i] ? Color.green : c;
            if (GUILayout.Button(animations[i]))
            {
                ap.animName = animations[i];
                ap.time = 0f;
            }

            if (i % 5 == 4 || i == animations.Length - 1)
            {
                GUILayout.EndHorizontal();
            }

            GUI.color = c;
        }

        base.OnInspectorGUI();
    }

    private void GetStateNames(UnityEditor.Animations.AnimatorStateMachine stateMachine, List<string> states)
    {
        foreach (var s in stateMachine.states)
            states.Add(s.state.name);

        foreach (var sm in stateMachine.stateMachines)
            GetStateNames(sm.stateMachine, states);
    }
}

#endif