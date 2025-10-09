using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    public abstract class ScriptableValueBehaviour<T, U, V> : MonoBehaviour where T : ScriptableValue<U, V> where V : UnityEvent<U>, new()
    {
        [SerializeField()]
        protected bool invokeOnAwake;

        [SerializeField()]
        protected T scriptableValue;

        [SerializeField()]
        protected V onChangeEvent;

        protected virtual void Awake()
        {
            if (invokeOnAwake)
                onChangeEvent.Invoke(scriptableValue.Value);

            scriptableValue.AddOnValueChangedEvent(onChangeEvent.Invoke);
        }

        protected virtual void OnDestroy()
        {
            scriptableValue.RemoveOnSetEvent(onChangeEvent.Invoke);
        }

        public void SetValue(U value)
        {
            scriptableValue.Value = value;
        }

        public void SwapScriptableValue(T newValue)
        {
            scriptableValue.RemoveOnSetEvent(onChangeEvent.Invoke);

            scriptableValue = newValue;

            scriptableValue.AddOnValueChangedEvent(onChangeEvent.Invoke);

            onChangeEvent.Invoke(scriptableValue.Value);
        }
    }
}