using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation.Edit
{
    [CustomEditor(typeof(AnimatorManagerStateBehaviour), true)]
    public class AnimatorManagerStateBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AnimatorManagerStateBehaviour behaviour = target as AnimatorManagerStateBehaviour;

            behaviour.DrawEditorGUI();
        }
    }
}