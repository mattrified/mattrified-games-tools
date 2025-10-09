using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    public class AnimatorManagerDataState : ScriptableObject
    {
        [Tooltip("The name of the animation.")]
        public StringHash animationName;

        [Ignore(), Tooltip("The index of the state within the data set")]
        public int index;

        [Tooltip("Transition information")]
        public AnimatorManagerTransitions transitions = new AnimatorManagerTransitions();

        /// <summary>
        /// A list of animator state behaviours.
        /// </summary>
        public List<AnimatorManagerStateBehaviour> amStateBehaviours;

        public T GetStateBehaviourByType<T>() where T : AnimatorManagerStateBehaviour
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                if (amStateBehaviours[i] is T)
                {
                    return (T)amStateBehaviours[i];
                }
            }
            return null;
        }

        // Wrapped in a UNITY_EDITOR pre-compiler since some editor-only functions are called.  Not doing this will cause compilation issues.
#if UNITY_EDITOR
        public UnityEditor.Animations.AnimatorState animatorState;

        /// <summary>
        /// Virtual function for processing a list of position deltas.
        /// </summary>
        public virtual void ProcessDeltaPosList(List<Vector3> deltaList)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].ProcessDeltaPosList(deltaList);
            }
        }

        /// <summary>
        /// Virtual function for processing a list of position deltas.
        /// </summary>
        public virtual void ProcessDeltaEulerList(List<Vector3> deltaList)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].ProcessDeltaEulerList(deltaList);
            }
        }

        /// <summary>
        /// Draws the gizmos from the specified animator managed editor
        /// </summary>
        public virtual void DrawGizmos(AnimatorManagerEditor editor)
        {

        }

        public virtual void DrawSceneEditorInfo(AnimatorManagerEditor editor)
        {

        }

        /// <summary>
        /// Draws the editor view
        /// </summary>
        /// <param name="animatorManagerEditorWindow">The animator window</param>
        /// <param name="table">A hashtable of various parameters</param>
        public virtual void DrawEditorView(AnimatorManagerEditorWindow animatorManagerEditorWindow, ref Hashtable table)
        {
            UnityEditor.EditorGUI.indentLevel++;
            UnityEditor.EditorGUILayout.LabelField("State:  " + this.name);
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                UnityEditor.EditorGUI.indentLevel++;

                //amStateBehaviours[i].DrawEditorView(animatorManagerEditorWindow, ref table);
                UnityEditor.EditorGUILayout.LabelField(amStateBehaviours[i].name);

                UnityEditor.EditorGUI.indentLevel++;
                amStateBehaviours[i].DrawEditorGUI();
                UnityEditor.EditorGUI.indentLevel--;

                UnityEditor.EditorGUI.indentLevel--;
            }
            UnityEditor.EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draws any SceneGUI (handles, etc.)
        /// </summary>
        /// <param name="animatorManagerEditor"></param>
        public virtual void DrawSceneGUI(AnimatorManagerEditor animatorManagerEditor)
        {

        }

        /// <summary>
        /// Creates a new hit box at the specified frame
        /// </summary>
        /// <param name="frame">The frame of the animation</param>
        /// <param name="b">The bounding box</param>
        public virtual void ProcessNewHitBox(int frame, Bounds b)
        {

        }

        public virtual void ProcessPreFrame(AnimatorManagerEditor animatorManagerEditor)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].ProcessPreFrame(animatorManagerEditor);
            }
        }

        public virtual void ProcessPostFrame(AnimatorManagerEditor animatorManagerEditor)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].ProcessPostFrame(animatorManagerEditor);
            }
        }

        public virtual void BeginPlaythroughProcessing(AnimatorManagerEditor animatorManagerEditor)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].BeginPlaythroughProcessing(animatorManagerEditor);
            }
        }

        public virtual void EndPlaythroughProcessing(AnimatorManagerEditor animatorManagerEditor)
        {
            for (int i = 0; i < amStateBehaviours.Count; i++)
            {
                amStateBehaviours[i].EndPlaythroughProcessing(animatorManagerEditor);
            }
        }

        public virtual void PostParse(AnimatorManagerData animatorManagerData)
        {

        }

#endif


    }
}