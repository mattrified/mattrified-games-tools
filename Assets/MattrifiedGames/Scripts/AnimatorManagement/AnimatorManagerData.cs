using UnityEngine;
using System.Collections.Generic;
using System;

// Wrapped in a UNITY_EDITOR pre-compiler since some editor-only functions are called.  Not doing this will cause compilation issues.
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

namespace MattrifiedGames.ManagedAnimation
{
    /// <summary>
    /// The data used by a managed animator to go to animation and update other types of data.
    /// </summary>
    [CreateAssetMenu(menuName = "Fighter Sample/Animator Manager Data")]
    public class AnimatorManagerData : ScriptableObject
    {
        [Tooltip("The animator controller this data is parsing.")]
        public RuntimeAnimatorController runtimeController;

        /// <summary>
        /// The list of states.
        /// </summary>
        public List<AnimatorManagerDataState> states;

        public List<AnimatorManagerParameter> parameters;

        public AnimatorManagerTransitions anyStateTransitions;

        public bool useNormalizedTime;

        public bool writeDefaults = false;

        /// <summary>
        /// Gets the state data at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public AnimatorManagerDataState this[int index]
        {
            get
            {
                return states[index];
            }
        }

        public T GetState<T>(int index) where T : AnimatorManagerDataState
        {
            return (T)states[index];
        }

        public int Count
        {
            get
            {
                return states.Count;
            }
        }

        internal int FindParameterIndex(string v)
        {
            return parameters.FindIndex(x => x.Name == v);
        }




        // Wrapped in a UNITY_EDITOR pre-compiler since some editor-only functions are called.
#if UNITY_EDITOR


        /// <summary>
        /// When combining Unity assets, this character forces them to go under the assets they are a part of.
        /// </summary>
        public const char LAST_CHAR = '\uE83A';

        [ContextMenu("Parse Animator Controller")]
        public void ParseControllerCM()
        {
            ParseController();
        }

        [ContextMenu("Fix Names")]
        protected void FixRenames()
        {
            bool foundMissing = false;

            states.RemoveAll(x => x == null);

            // First, fixes names
            for (int i = 0; i < states.Count; i++)
            {
                if (states[i].animatorState == null)
                {
                    DestroyImmediate(states[i], true);
                    states.RemoveAt(i);
                    i--;
                    foundMissing = true;
                }

                if (states[i].name != states[i].animatorState.name)
                {
                    states[i].name = states[i].animatorState.name;
                    states[i].animationName.Name = states[i].animatorState.name;
                }
            }

            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(this));

            // We found a missing state so we are reparsing info.
            if (foundMissing)
            {
                ParseController();
            }
        }

        [ContextMenu("Clean-Up")]
        protected void CleanUp()
        {
            UnityEngine.Object[] objs = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(UnityEditor.AssetDatabase.GetAssetPath(this));
            foreach (UnityEngine.Object obj in objs)
            {
                // We don't want to delete the main object.
                if (obj == this)
                    continue;

                if (obj is AnimatorManagerDataState)
                {
                    if (this.states.Contains((AnimatorManagerDataState)obj))
                        continue;

                    DestroyImmediate(obj, true);
                }
            }

            UnityEditor.AssetDatabase.ImportAsset(UnityEditor.AssetDatabase.GetAssetPath(this));

            ParseControllerCM();
        }

        /// <summary>
        /// Parses the animator controller.
        /// </summary>
        protected virtual void ParseController()
        {
            // We create the data and populate the manager.
            UnityEditor.Animations.AnimatorController controller = runtimeController as UnityEditor.Animations.AnimatorController;

            // Gets the number of controller parameters.
            parameters = new List<AnimatorManagerParameter>(controller.parameters.Length);
            for (int i = 0; i < controller.parameters.Length; i++)
            {
                parameters.Add(new AnimatorManagerParameter(controller.parameters[i]));
            }
            parameters.RemoveAll(x => x.Name.StartsWith("*"));

            var layers = controller.layers;
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].name.StartsWith("*"))
                    continue;

                ParseLayer(layers[i]);
            }

            foreach (AnimatorManagerDataState state in states)
            {
                state.PostParse(this);

                try
                {
                    int behaviourIndex = 0;
                    state.amStateBehaviours.RemoveAll(x => x == null);
                    foreach (var behaviour in state.amStateBehaviours)
                    {
                        behaviour.behaviourIndex = behaviourIndex;
                        behaviour.behaviourIndexBit = 1 << behaviourIndex;
                        behaviour.behaviourIndexBitInvert = ~behaviour.behaviourIndexBit;
                        behaviour.Setup(this, state);
                        behaviourIndex++;
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError("Error on state:  " + state);
                }
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        /// <summary>
        /// Gets the length of the requested state.
        /// </summary>
        /// <param name="index">THe state index</param>
        /// <returns>The length of the state in seconds</returns>
        internal float GetStateLength(int index)
        {
            return GetStateLength(this[index].animatorState);
        }

        /// <summary>
        /// Finds the index of the specified animator state.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected int FindStateIndex(AnimatorState state)
        {
            return states.FindIndex(x => x.animationName.Name == state.name);
        }

        /// <summary>
        /// Gets the length of the animator statae
        /// </summary>
        /// <param name="state">The specified state</param>
        /// <returns>The length of the animation in seconds</returns>
        protected float GetStateLength(AnimatorState state)
        {
            if (state.motion == null)
                return 1f;

            return state.motion.averageDuration / Mathf.Abs(state.speed);
        }

        /// <summary>
        /// Parses the animation layer
        /// </summary>
        protected void ParseLayer(AnimatorControllerLayer layer)
        {
            var anyStateTransitions = layer.stateMachine.anyStateTransitions;
            foreach (var t in anyStateTransitions)
                t.mute = true;

            var stateMachine = layer.stateMachine;
            var animStateMachines = stateMachine.stateMachines;
            var animStates = stateMachine.states;

            ParseAnimationStates(animStates);
            ParseStateMachines(animStateMachines);

            this.anyStateTransitions.Setup(this, null, useNormalizedTime);
        }

        /// <summary>
        /// Parses the provided array of animator state machines.
        /// </summary>
        private void ParseStateMachines(ChildAnimatorStateMachine[] animStateMachines)
        {
            for (int i = 0; i < animStateMachines.Length; i++)
            {
                var childStates = animStateMachines[i].stateMachine.states;
                ParseAnimationStates(childStates);

                var childStateMachines = animStateMachines[i].stateMachine.stateMachines;
                ParseStateMachines(childStateMachines);
            }
        }

        /// <summary>
        /// Parses the provided array of child animator states
        /// </summary>
        private void ParseAnimationStates(ChildAnimatorState[] animStates)
        {
            for (int i = 0; i < animStates.Length; i++)
            {
                int index = states.FindIndex(x => x.animationName.Name == animStates[i].state.name);
                AnimatorManagerDataState ams;
                if (index < 0)
                {
                    ams = CreateState(animStates[i].state.name);
                    
                    UnityEditor.AssetDatabase.AddObjectToAsset(ams, this);
                    states.Add(ams);
                    ams.index = states.Count - 1;
                }
                else
                {
                    ams = states[index];
                    ams.index = index;
                }
                
                ParseState(animStates[i].state, ams);
            }
        }

        /// <summary>
        /// Parses the specified state.
        /// </summary>
        /// <param name="state">The animator state being parsed.</param>
        /// <param name="ams">The animator manager data state parsed data will be put into</param>
        protected virtual void ParseState(AnimatorState state, AnimatorManagerDataState ams)
        {
            ams.animationName = new StringHash() { Name = state.name };
            ams.amStateBehaviours = new List<AnimatorManagerStateBehaviour>();
            ams.animatorState = state;

            state.writeDefaultValues = writeDefaults;

            // Mutes all transitions;
            var transitions = state.transitions;
            foreach (var t in transitions)
            {
                t.mute = true;
            }

            // Defines transitions
            ams.transitions.Setup(this, ams, useNormalizedTime);

            for (int i = 0; i < state.behaviours.Length; i++)
            {
                if (state.behaviours[i] is AnimatorManagerStateBehaviour)
                {
                    AnimatorManagerStateBehaviour behaviour = (AnimatorManagerStateBehaviour)state.behaviours[i];
                    ams.amStateBehaviours.Add(behaviour);
                }
            }
        }

        /// <summary>
        /// Creates animator manager data from the specified string name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual AnimatorManagerDataState CreateState(string name)
        {
            AnimatorManagerDataState newState = CreateInstance<AnimatorManagerDataState>();
            newState.name = LAST_CHAR + name;
            return newState;
        }

        internal int FindStateByTag_EDITOR(string v)
        {
            return states.FindIndex(x => x.animatorState.tag == v);
        }

        internal int FindStateByName_EDITOR(string v)
        {
            return states.FindIndex(x => x.animatorState.name == v);
        }

#endif

    }

    /// <summary>
    /// A class made to store the hash values generated by Animator.StringToHash.
    /// </summary>
    [System.Serializable()]
    public class StringHash
    {
        [SerializeField(), Tooltip("The name or string whose hash is requested.")]
        protected string name;

        /// <summary>
        /// The Animator retrieved hash value.
        /// </summary>
        [System.NonSerialized()]
        protected int hash;

        /// <summary>
        /// If true, the hash has been created.  False, if not.
        /// </summary>
        [System.NonSerialized()]
        protected bool hasCreatedHashed;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                hasCreatedHashed = false;
            }
        }

        public int Hash
        {
            get
            {
                if (!hasCreatedHashed)
                {
                    hash = Animator.StringToHash(name);
                    hasCreatedHashed = true;
                }

                return hash;
            }
        }
    }

    [System.Serializable()]
    public class AnimatorManagerParameter : StringHash
    {
        [SerializeField()]
        float parameterValue;

        [SerializeField()]
        AnimatorControllerParameterType type;

        public AnimatorManagerParameter(AnimatorManagerParameter previous)
        {
            name = previous.name;
            type = previous.type;
            parameterValue = previous.parameterValue;
        }

        public AnimatorManagerParameter(AnimatorControllerParameter animatorControllerParameter)
        {
            this.name = animatorControllerParameter.name;
            this.type = animatorControllerParameter.type;

            switch (type)
            {
                case AnimatorControllerParameterType.Bool: parameterValue = animatorControllerParameter.defaultBool ? 1 : 0; break;
                case AnimatorControllerParameterType.Int: parameterValue = animatorControllerParameter.defaultInt; break;
                case AnimatorControllerParameterType.Trigger:
                    Debug.LogWarning("Triggers are not recommended.  Will be treated as a  false bool");
                    parameterValue = 0; break;
                case AnimatorControllerParameterType.Float: parameterValue = animatorControllerParameter.defaultFloat; break;
            }
        }

        public bool BoolValue
        {
            get
            {
                return parameterValue > 0.5f;
            }

            set
            {
                parameterValue = value ? 1 : 0;
            }
        }

        public int IntValue
        {
            get
            {
                return Mathf.RoundToInt(parameterValue);
            }

            set
            {
                parameterValue = value;
            }
        }

        public float FloatValue
        {
            get
            {
                return parameterValue;
            }

            set
            {
                parameterValue = value;
            }
        }

        public void GetFromAnimator(AnimatorManager manager)
        {
            GetFromAnimator(manager.Anim);
        }

        public void GetFromAnimator(AnimatorManagerPlayer player)
        {
            GetFromAnimator(player.manager.Anim);
        }

        public void GetFromAnimator(Animator anim)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Bool: parameterValue = anim.GetBool(Hash) ? 1 : 0; break;
                case AnimatorControllerParameterType.Int: parameterValue = anim.GetInteger(Hash); break;
                case AnimatorControllerParameterType.Trigger:
                    Debug.LogWarning("Triggers are not recommended.  Will be treated as a  false bool");
                    parameterValue = 0; break;
                case AnimatorControllerParameterType.Float: parameterValue = anim.GetFloat(Hash); break;
            }
        }

        public void SetToAnimator(AnimatorManager manager)
        {
            SetToAnimator(manager.Anim);
        }

        public void SetToAnimator(AnimatorManagerPlayer player)
        {
            SetToAnimator(player.manager.Anim);
        }

        public void SetToAnimator(Animator anim)
        {
            switch (type)
            {
                case AnimatorControllerParameterType.Bool: anim.SetBool(Hash, parameterValue > 0.5f); break;
                case AnimatorControllerParameterType.Int: anim.SetInteger(Hash, Mathf.RoundToInt(parameterValue)); break;
                case AnimatorControllerParameterType.Trigger: anim.SetTrigger(Hash);break;
                case AnimatorControllerParameterType.Float: anim.SetFloat(Hash, parameterValue); break;
            }
        }

        internal void SetToAnimator(bool value, AnimatorManagerPlayer player)
        {
            BoolValue = value;
            SetToAnimator(player);
        }

        internal void SetToAnimator(int value, AnimatorManagerPlayer player)
        {
            IntValue = value;
            SetToAnimator(player);
        }

        internal void SetToAnimator(float value, AnimatorManagerPlayer player)
        {
            FloatValue = value;
            SetToAnimator(player);
        }
    }
}

