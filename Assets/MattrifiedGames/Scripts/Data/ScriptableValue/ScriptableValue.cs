using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    public abstract class ScriptableValue<T, U> : ScriptableValueBase where U : UnityEvent<T>, new()
    {
        [SerializeField()]
        protected T defaultValue;

        [System.NonSerialized()]
        protected T _value;

        [System.NonSerialized()]
        protected bool assigned = false;

        [System.NonSerialized()]
        protected U onValueSetEvent = new U();

        public override void Clear()
        {
            _value = defaultValue;
            assigned = false;
            onValueSetEvent.Invoke(_value);
        }

        public void AddOnValueChangedEvent(UnityAction<T> unityAction)
        {
            onValueSetEvent.AddListener(unityAction);
        }

        public void TriggerOnValueChangedEvent()
        {
            onValueSetEvent.Invoke(Value);
        }

        public void RemoveOnSetEvent(UnityAction<T> unityAction)
        {
            onValueSetEvent.RemoveListener(unityAction);
        }

        public void ClearAllListeners()
        {
            onValueSetEvent.RemoveAllListeners();
        }

        

        public virtual T Value
        {
            get
            {
                if (!assigned)
                {
                    AssignDefault();
                }
                return _value;
            }

            set
            {
                _value = value;
                assigned = true;
                onValueSetEvent.Invoke(_value);
            }
        }

        public virtual void CopyTo(ref T inValue)
        {
            if (!assigned)
                AssignDefault();
            inValue = _value;
        }

        protected virtual void AssignDefault()
        {
            _value = defaultValue;
            assigned = true;
        }

        public override string Save()
        {
            return Value.ToString();
        }

#if UNITY_EDITOR
        public void SetDefaultValue(T value)
        {
            defaultValue = value;
        }
#endif
    }
}