using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using MattrifiedGames.ManagedAnimation;
using System;

[System.Serializable()]
public class AnimatorManagerTransitions
{
    //public bool allowAnyStateTransitions;
    //public FP checkTime = 0;

    /*public bool useLimit = false;
    public int ignoreLimitIndex;
    public FP limit = FP.Zero;*/

    public AnimatorManagerTransitionInfo[] transitionInfo = new AnimatorManagerTransitionInfo[0];

    public int Length
    {
        get
        {
            return transitionInfo.Length;
        }
    }
    public AnimatorManagerTransitionInfo this[int index]
    {
        get
        {
            return transitionInfo[index];
        }
    }

    /*public static void TransitionState(TransitionInfo transitionInfo, DragHerAnimationData data, ref DragHerFighter fighter, ref DragHerFrame frameID)
    {
        /*var behaviours = data.states[fighter.animIndex].amStateBehaviours;
        for (int j = 0; j < behaviours.Count; j++)
        {
            ((DragHerStateBehaviour)behaviours[j]).OnExit(data, ref fighter, ref frameID);
        }*/

    // fighter.animIndex = transitionInfo.nextStateIndex;
    //fighter.animTime = transitionInfo.nextStateStartTime;

    //behaviours = data.states[fighter.animIndex].amStateBehaviours;

    /*for (int j = 0; j < behaviours.Count; j++)
    {
        ((DragHerStateBehaviour)behaviours[j]).OnEnter(data, ref fighter, ref frameID);
    }*/
    //}

    //internal static void TransitionState(int nextAnimIndex, FP startTime, DragHerAnimationData data, ref DragHerFighter fighter, ref DragHerFrame frameID)
    //{
    /*var behaviours = data.states[fighter.animationIndex].amStateBehaviours;
    for (int j = 0; j < behaviours.Count; j++)
    {
        ((IFighterUpdate)behaviours[j]).OnExit(data, ref fighter, ref frameID);
    }*/

    //  fighter.animIndex = nextAnimIndex;
    //  fighter.animTime = startTime;

    /*behaviours = data.states[fighter.animationIndex].amStateBehaviours;

    for (int j = 0; j < behaviours.Count; j++)
    {
        ((IFighterUpdate)behaviours[j]).OnEnter(data, ref fighter, ref frameID);
    }*/
    // }*/
    internal bool TestAllTransitions(int frameNumber, FP animTime, Func<int, int, int> parameters, out AnimatorManagerTransitionInfo result)
    {
        for (int i = 0; i < transitionInfo.Length; i++)
        {
            if (transitionInfo[i].TestConditions(frameNumber, animTime, parameters))
            {
                result = transitionInfo[i];
                return true;
            }
        }

        result = null;
        return false;
    }

#if UNITY_EDITOR
    public void Setup(AnimatorManagerData data, AnimatorManagerDataState state, bool calculateNormalizedTime)
    {
        bool anyState = state == null;

        List<AnimatorManagerTransitionInfo> infoList = new List<AnimatorManagerTransitionInfo>();

        UnityEditor.Animations.AnimatorController acr = data.runtimeController as
            UnityEditor.Animations.AnimatorController;

        if (anyState)
        {
            var ast = acr.layers[0].stateMachine.anyStateTransitions;

            for (int i = 0; i < ast.Length; i++)
            {
                AnimatorManagerTransitionInfo ti = new AnimatorManagerTransitionInfo();
                ti.nextStateIndex = data.states.FindIndex(x => x.animatorState ==
                    ast[i].destinationState);

                ti.hasExitTime = ast[i].hasExitTime;

                var nS = data[ti.nextStateIndex];

                if (calculateNormalizedTime)
                {
                    ti.exitTime = ast[i].exitTime * Mathf.Abs(nS.animatorState.motion.averageDuration / nS.animatorState.speed);
                }
                else
                {
                    ti.exitTime = ast[i].exitTime;
                }

                ti.nextStateTransitionDuration = ast[i].duration;

                // Is this right?  Uh oh if not...
                ti.nextStateStartTime = ast[i].offset * Mathf.Abs(nS.animatorState.motion.averageDuration / nS.animatorState.speed);

                List<AnimatorManagerTransitionCondition> conditions = new List<AnimatorManagerTransitionCondition>();

                for (int j = 0; j < ast[i].conditions.Length; j++)
                {
                    var t = ast[i];
                    AnimatorManagerTransitionCondition tc = new AnimatorManagerTransitionCondition();
                    tc.parameterIndex = data.parameters.FindIndex(x => x.Name == t.conditions[j].parameter);
                    tc.bitmask = t.conditions[j].parameter.StartsWith("BIT_");
                    switch (t.conditions[j].mode)
                    {
                        case UnityEditor.Animations.AnimatorConditionMode.If:
                            tc.minValue = 1;
                            tc.maxValue = 1;
                            break;
                        case UnityEditor.Animations.AnimatorConditionMode.IfNot:
                            tc.minValue = 0;
                            tc.maxValue = 0;
                            break;
                        case UnityEditor.Animations.AnimatorConditionMode.Greater:
                            tc.minValue = Mathf.FloorToInt(t.conditions[j].threshold) + 1;
                            tc.maxValue = int.MaxValue;
                            break;
                        case UnityEditor.Animations.AnimatorConditionMode.Less:
                            tc.minValue = int.MinValue;
                            tc.maxValue = Mathf.FloorToInt(t.conditions[j].threshold) - 1;
                            break;
                        case UnityEditor.Animations.AnimatorConditionMode.Equals:
                            if (!tc.bitmask)
                                tc.minValue = Mathf.FloorToInt(t.conditions[j].threshold);
                            else
                                tc.minValue = 1 << Mathf.FloorToInt(t.conditions[j].threshold);
                            tc.maxValue = tc.minValue;
                            break;
                        case UnityEditor.Animations.AnimatorConditionMode.NotEqual:
                            if (!tc.bitmask)
                                Debug.LogWarning("Avoid NotEqual if possible.");
                            tc.minValue = 1 << Mathf.FloorToInt(t.conditions[j].threshold);
                            tc.maxValue = tc.minValue - 1;
                            break;
                    }
                    conditions.Add(tc);
                }

                ti.conditions = conditions.ToArray();

                infoList.Add(ti);
            }

            transitionInfo = infoList.ToArray();
            return;
        }

        for (int i = 0; i < state.animatorState.transitions.Length; i++)
        {
            var t = state.animatorState.transitions[i];

            AnimatorManagerTransitionInfo ti = new AnimatorManagerTransitionInfo();

            ti.nextStateIndex = data.states.FindIndex(x => x.animatorState == t.destinationState);

            ti.hasExitTime = t.hasExitTime;

            if (calculateNormalizedTime)
            {
                ti.exitTime = t.exitTime * Mathf.Abs(state.animatorState.motion.averageDuration / state.animatorState.speed);
            }
            else
            {
                ti.exitTime = t.exitTime;// * Mathf.Abs(state.animatorState.motion.averageDuration / state.animatorState.speed);
            }

            ti.nextStateTransitionDuration = t.duration;

            if (t.destinationState == null || t.destinationState.motion == null)
            {
                Debug.Log("ERROR:  " + state.animationName.Name);
            }

            if (calculateNormalizedTime)
            {
                ti.nextStateStartTime = t.offset *
                    Mathf.Abs(t.destinationState.motion.averageDuration / t.destinationState.speed);
            }
            else
            {
                ti.nextStateStartTime = t.offset;
            }

            List<AnimatorManagerTransitionCondition> conditions = new List<AnimatorManagerTransitionCondition>();

            for (int j = 0; j < t.conditions.Length; j++)
            {
                AnimatorManagerTransitionCondition tc = new AnimatorManagerTransitionCondition();
                tc.parameterIndex = data.parameters.FindIndex(x => x.Name == t.conditions[j].parameter);

                tc.bitmask = t.conditions[j].parameter.StartsWith("BIT_");
                switch (t.conditions[j].mode)
                {
                    case UnityEditor.Animations.AnimatorConditionMode.If:
                        tc.minValue = 1;
                        tc.maxValue = 1;
                        break;
                    case UnityEditor.Animations.AnimatorConditionMode.IfNot:
                        tc.minValue = 0;
                        tc.maxValue = 0;
                        break;
                    case UnityEditor.Animations.AnimatorConditionMode.Greater:
                        tc.minValue = Mathf.FloorToInt(t.conditions[j].threshold) + 1;
                        tc.maxValue = int.MaxValue;
                        break;
                    case UnityEditor.Animations.AnimatorConditionMode.Less:
                        tc.minValue = int.MinValue;
                        tc.maxValue = Mathf.FloorToInt(t.conditions[j].threshold) - 1;
                        break;
                    case UnityEditor.Animations.AnimatorConditionMode.Equals:
                        if (!tc.bitmask)
                            tc.minValue = Mathf.FloorToInt(t.conditions[j].threshold);
                        else
                            tc.minValue = 1 << Mathf.FloorToInt(t.conditions[j].threshold);
                        tc.maxValue = tc.minValue;
                        break;
                    case UnityEditor.Animations.AnimatorConditionMode.NotEqual:
                        if (!tc.bitmask)
                            Debug.LogWarning("Avoid NotEqual if possible.");
                        tc.minValue = 1 << Mathf.FloorToInt(t.conditions[j].threshold);
                        tc.maxValue = tc.minValue - 1;
                        break;
                }
                conditions.Add(tc);
            }

            ti.conditions = conditions.ToArray();
            infoList.Add(ti);
        }


        transitionInfo = infoList.ToArray();
    }




#endif
}
    

    [System.Serializable()]
    public class AnimatorManagerTransitionInfo
    {
        public int nextStateIndex;
        public FP nextStateStartTime;
        public FP nextStateTransitionDuration;

        public bool hasExitTime;
        public FP exitTime;

        public AnimatorManagerTransitionCondition[] conditions;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameNumber"></param>
    /// <param name="animTime"></param>
    /// <param name="GetParameterFunction">The fumnction that gets paramters and returns a rsult.  First index is frame nmber; second is parameter index.</param>
    /// <returns></returns>
        public bool TestConditions(int frameNumber, FP animTime, System.Func<int,int,int> GetParameterFunction)
        {
            if (hasExitTime && animTime < exitTime)
            {
                return false;
            }

            for (int i = 0; i < conditions.Length; i++)
            {
                if (!conditions[i].Test(GetParameterFunction(frameNumber, conditions[i].parameterIndex)))
                    return false;
            }

            return true;
        }
    }

/*public interface IAnimatorManagerInfo
{
    FP AnimTime { get; set; }
    int AnimIndex { get; set; }
    FP AnimTransition { get; set; }

    int Parameter(int frameNumber, int index);
}*/

[System.Serializable()]
public struct AnimatorManagerTransitionCondition
{
    public int parameterIndex;
    public int minValue;
    public int maxValue;
    public bool bitmask;

    public bool Test(int inValue)
    {
        if (!bitmask)
            return inValue >= minValue && inValue <= maxValue;
        else
        {
            return (minValue == maxValue) == (MattrifiedGames.Utility.StaticHelpers.ByteMaskCheck(inValue, minValue));
        }
    }
}