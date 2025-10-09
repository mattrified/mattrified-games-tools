
#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// Editor class for updating how the animator manager editor is updated OnSceneGUI
    /// </summary>
    [CustomEditor(typeof(AnimatorManagerEditor))]
    public class AnimatorManagerEditorClass : Editor
    {
        private void OnSceneGUI()
        {
            AnimatorManagerEditor ame = (AnimatorManagerEditor)target;

            ame.OnSceneGUI();
        }
    }
}
#endif
