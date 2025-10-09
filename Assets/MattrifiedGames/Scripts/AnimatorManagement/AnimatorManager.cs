using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// Managed animation more directly for a Unity animator.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AnimatorManager : MonoBehaviour
    {
        

        [SerializeField(), Tooltip("The data used by this animator manager.")]
        public AnimatorManagerData data;

        [SerializeField(), Tooltip("The current index of the state.")]
        int index;

        [SerializeField(), Tooltip("The current time of the animation.")]
        public float time;

        //[SerializeField(), Tooltip("The rate at which the animation tries to catch up to the intime to prevent huge animation pops")]
        //public float moveTowardsRate = 3f / 60f;

        protected Animator animator;

        [Tooltip("The parameters used for this animator, cloned from the data.")]
        public List<AnimatorManagerParameter> parameters;

        public TextMeshPro stateText;
        public bool updateText;

        public bool debug;

        public int StateIndex
        {
            get
            {
                return index;
            }
        }

        public Animator Anim
        {
            get
            {
                return animator;
            }
        }

        private void Awake()
        {
            animator = GetComponent<Animator>();

            // Sets the runtime controller to the one associated with the player data.
            Anim.runtimeAnimatorController = data.runtimeController;

            // Creates a new list of parameters based on the current parameters.
            parameters = new List<AnimatorManagerParameter>();
            for (int i = 0; i < data.parameters.Count; i++)
                parameters.Add(new AnimatorManagerParameter(data.parameters[i]));

            // Makes sures to disable the animator as well as its root motion since this script will be controlling the animator and root motion directly.
            Anim.applyRootMotion = false;
            Anim.enabled = false;
        }

        public bool CheckState(AnimatorManagerDataState state)
        {
            return data[index].index == state.index;
        }

        internal bool CheckStateList(List<AnimatorManagerDataState> otherState)
        {
            for (int i = 0; i < otherState.Count; i++)
            {
                if (CheckState(otherState[i]))
                    return true;
            }
            return false;// otherState.Exists(CheckState);
        }

        public AS GetState<AS>(int index) where AS : AnimatorManagerDataState
        {
            return (AS)data[index];
        }

        /// <summary>
        /// Updates the animator
        /// </summary>
        /// <param name="inIndex">The animation index</param>
        /// <param name="inTime">The animation time</param>
        /// <param name="inTransitionTime">The length of the current transition</param>
        public void UpdateAnimator(int inIndex, float inTime, float inTransitionTime = 0)
        {
            // If the current index and the new index are the same, the animator is just updated
            if (index == inIndex)
            {
                // Limits how fast the timer moves to prevent popping
                //inTime = Mathf.MoveTowards(time, inTime, moveTowardsRate);

                inTime = Mathf.RoundToInt(inTime * 60 + Mathf.Epsilon) / 60f;

                // Updates the animator by the difference in time.
                animator.Update(inTime - time);

                // Sets the currnet time.
                time = inTime;
            }
            else
            {
                // Begins a cross fade to the new animation.
                animator.CrossFadeInFixedTime(data[inIndex].animationName.Hash,
                    inTransitionTime, 0, inTime);

                // Updates the animator.
                animator.Update(0);

                // Sets the new index and time.
                index = inIndex;
                time = inTime;
            }

            if(updateText)
            {
                stateText.text = data[inIndex].name;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Build Avatar")]
        void CreateAvatar()
        {
            var avatar = AvatarBuilder.BuildGenericAvatar(this.gameObject, "Root");
            UnityEditor.AssetDatabase.CreateAsset(avatar, "Assets/Test.asset");
        }
#endif
    }
}