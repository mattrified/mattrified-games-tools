using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// Abstract class for animator state behaviours
    /// </summary>
    public abstract class AnimatorManagerStateBehaviour : StateMachineBehaviour
    {
        [Ignore()]
        public int behaviourIndex;

        [Ignore()]
        public int behaviourIndexBit;

        [Ignore()]
        public int behaviourIndexBitInvert;

        public virtual void OnManagedEnter(AnimatorManagerPlayer player)
        {

        }

        public virtual bool OnManagedUpdate(AnimatorManagerPlayer player)
        {
            return false;
        }

        public virtual void OnManagedExit(AnimatorManagerPlayer player)
        {

        }

#if UNITY_EDITOR

        public virtual void DrawSceneGUI(AnimatorManagerEditor animatorManagerEditor)
        {

        }

        public virtual void DrawGizmo(AnimatorManagerEditor ame)
        {

        }

        public virtual void DrawEditorGUI()
        {

        }

        public virtual void Setup(AnimatorManagerData data, AnimatorManagerDataState state)
        {
            // TODO:  Defines unique data about the state here.
            // For example, if a state wants all of its transitions, do that here.
        }

        public virtual void Copy()
        {
            Debug.LogWarning("Copy not setup.");
        }

        public virtual void Paste()
        {
            Debug.LogWarning("Paste not setup.");
        }

        public virtual void ProcessDeltaPosList(List<Vector3> deltaList)
        {
            
        }

        public virtual void EndPlaythroughProcessing(AnimatorManagerEditor animatorManagerEditor)
        {

        }

        public virtual void BeginPlaythroughProcessing(AnimatorManagerEditor animatorManagerEditor)
        {
            
        }

        public virtual void ProcessDeltaEulerList(List<Vector3> deltaList)
        {
            
        }

        public virtual void ProcessPreFrame(AnimatorManagerEditor animatorManagerEditor)
        {
            
        }

        public virtual void ProcessPostFrame(AnimatorManagerEditor animatorManagerEditor)
        {
            
        }
#endif
    }
}