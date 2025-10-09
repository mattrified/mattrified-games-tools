using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.MGTweening
{
    public abstract class MattrifiedTweenBase : MonoBehaviour 
    {
        [SerializeField()]
        ClampedFloat tweenValue;

        public float rate;
        public AnimationCurve curve;

        public ConditionalUnityEvent onReachStartEvent;
        public ConditionalUnityEvent onReachEndEvent;

        [ContextMenu("Make Infinite")]
        public void MakeInfinite()
        {
            tweenValue.Min = float.NegativeInfinity;
            tweenValue.Max = float.PositiveInfinity;
        }

        public float TweenTime
        {
            get
            {
                return tweenValue.Value;
            }

            set
            {
                if (Mathf.Approximately(tweenValue.Value, value))
                    return;

                tweenValue.Value = value;

                UpdateTween();

                if (Mathf.Approximately(tweenValue.Value, tweenValue.Min))
                    onReachStartEvent.value.Invoke();

                if (Mathf.Approximately(tweenValue.Value, tweenValue.Max))
                    onReachEndEvent.value.Invoke();
            }
        }

        public void Flip()
        {
            rate *= -1f;
        }

        public void PlayBackwards()
        {
            rate = -1f * Mathf.Abs(rate);
        }

        public void PlayForward()
        {
            rate = Mathf.Abs(rate);
        }

        public void Stop()
        {
            rate = 0f;
        }

        public void Play(float newRate)
        {
            rate = newRate;
        }

        public void ResetAndPlay(float newRate)
        {
            TweenTime = 0;
            Play(newRate);
        }

        public float TweenTimeMin
        {
            get
            {
                return tweenValue.Min;
            }

            set
            {
                float oldValue = tweenValue.Value;
                tweenValue.Min = value;

                if (Mathf.Approximately(oldValue, tweenValue.Value))
                    return;

                UpdateTween();
            }
        }

        public float TweenTimeMax
        {
            get
            {
                return tweenValue.Max;
            }

            set
            {
                float oldValue = tweenValue.Value;
                tweenValue.Max = value;

                if (Mathf.Approximately(oldValue, tweenValue.Value))
                    return;

                UpdateTween();
            }
        }

        public float CurveValue
        {
            get
            {
                return curve.Evaluate(TweenTime);
            }
        }

        public void DeltaUpdate(float delta)
        {
            // If the rate is approximately 0, we don't bother updating the value.
            if (Mathf.Approximately(rate, 0f))
                return;

            TweenTime += delta * rate;
        }

        public void Update()
        {
            DeltaUpdate(Time.deltaTime);
        }

        public abstract void UpdateTween();
    }

    public abstract class MattrifiedTweenBase<IN_VALUE> : MattrifiedTweenBase
    {
        [SerializeField()]
        protected IN_VALUE valueA, valueB;

        public abstract IN_VALUE TweenedValue { get; }
    }

    public abstract class MattrifiedTweenBase<IN_VALUE, AFFECTED_OBJ> : MattrifiedTweenBase<IN_VALUE> where AFFECTED_OBJ : class
    {
        [SerializeField()]
        protected AFFECTED_OBJ target;

        private void OnValidate()
        {
            try
            {
                if (target == null)
                    target = GetComponent(typeof(AFFECTED_OBJ)) as AFFECTED_OBJ;
            }
            catch
            {

            }
        }
    }

    public abstract class MattrifiedTweenBaseFloat<T> : MattrifiedTweenBase<float, T> where T : class
    {
        public bool clampValue;

        public override float TweenedValue
        {
            get
            {
                return clampValue ? Mathf.Lerp(valueA, valueB, CurveValue) : Mathf.LerpUnclamped(valueA, valueB, CurveValue);
            }
        }
    }

    public abstract class MattrifiedTweenBaseColor<T> : MattrifiedTweenBase<Color, T> where T : class
    {
        public bool clampValue;

        public override Color TweenedValue
        {
            get
            {
                return clampValue ? Color.Lerp(valueA, valueB, CurveValue) : Color.LerpUnclamped(valueA, valueB, CurveValue);
            }
        }
    }

    public abstract class MattrifiedTweenBaseVector3<T> : MattrifiedTweenBase<Vector3, T> where T : class
    {
        public bool clampValue;

        public override Vector3 TweenedValue
        {
            get
            {
                return clampValue ? Vector3.Lerp(valueA, valueB, CurveValue) : Vector3.LerpUnclamped(valueA, valueB, CurveValue);
            }
        }
    }

    public abstract class MattrifiedTweenBaseVector2<T> : MattrifiedTweenBase<Vector2, T> where T : class
    {
        public bool clampValue;

        public override Vector2 TweenedValue
        {
            get
            {
                return clampValue ? Vector2.Lerp(valueA, valueB, CurveValue) : Vector2.LerpUnclamped(valueA, valueB, CurveValue);
            }
        }
    }

    public abstract class MattrifiedTweenBaseInt<T> : MattrifiedTweenBase<int, T> where T : class
    {
        public bool clampValue;

        public override int TweenedValue
        {
            get
            {
                return Mathf.RoundToInt(clampValue ? Mathf.Lerp(valueA, valueB, CurveValue) : Mathf.LerpUnclamped(valueA, valueB, CurveValue));
            }
        }
    }
}