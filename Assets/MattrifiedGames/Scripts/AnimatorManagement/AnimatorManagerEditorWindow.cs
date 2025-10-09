#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// The editor window opened when a scene with a animator manager editor is opened.
    /// </summary>
    public class AnimatorManagerEditorWindow : EditorWindow
    {
        /// <summary>
        /// The animator managar editor being used by this window.
        /// </summary>
        public AnimatorManagerEditor ame;

        Hashtable table = new Hashtable();
        Vector2 scroll;

        protected virtual void OnGUI()
        {
            if (ame == null)
            {
                Close();
                return;
            }

            ame.hideGizmos = EditorGUILayout.Toggle("Hide Gizmos", ame.hideGizmos);

            if (ame.IsPlaying)
            {
                GUILayout.Label(ame.name + " is playing.");
                //Editor.gi EditorUtility.giz
                EditorUtility.audioMasterMute = true;


                return;
            }

            if (GUILayout.Button("Play Animation"))
            {
                ame.PlayAnimation();
                return;
            }

            if (GUILayout.Button("Play All Animations"))
            {
                ame.PlayAllAnimations();
                return;
            }


            if (ame.otherScenesToLoad.Length > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Load Scene");
                for (int i = 0; i < ame.otherScenesToLoad.Length; i++)
                {
                    if (GUILayout.Button(ame.otherScenesToLoad[i]))
                    {
                        UnityEngine.SceneManagement.SceneManager.LoadScene(ame.otherScenesToLoad[i]);
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);

            ame.time = EditorGUILayout.Slider("Time", ame.time, 0, ame.CurrentStateLength);
            int oldFrame = Mathf.FloorToInt(ame.time * 60f);
            int newFrame = EditorGUILayout.IntField("Frame", oldFrame);
            if (oldFrame != newFrame)
            {
                ame.time = newFrame / 60f;
            }
            UnityEditor.EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Frame"))
            {
                ame.time -= 1f / 60f;
                if (ame.time < 0)
                    ame.time = 0;
            }

            if (GUILayout.Button("Next Frame"))
            {
                ame.time += 1f / 60f;
                if (ame.time > ame.CurrentStateLength)
                    ame.time = ame.CurrentStateLength;
            }

            UnityEditor.EditorGUILayout.EndHorizontal();



            int oldIndex = ame.index;
            ame.index = EditorGUILayout.Popup("Animation", ame.index, ame.stateNames);

            UnityEditor.EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous Animation"))
            {
                ame.index--;
                if (ame.index < 0)
                    ame.index = ame.stateNames.Length - 1;
            }

            if (GUILayout.Button("Next Animation"))
            {
                ame.index++;
                if (ame.index >= ame.stateNames.Length)
                    ame.index = 0;
            }

            UnityEditor.EditorGUILayout.EndHorizontal();

            if (oldIndex != ame.index)
            {
                ame.ResetChar();
            }

            ame.UpdateAnimator();

            ame.manager.data[ame.index].DrawEditorView(this, ref table);

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif
