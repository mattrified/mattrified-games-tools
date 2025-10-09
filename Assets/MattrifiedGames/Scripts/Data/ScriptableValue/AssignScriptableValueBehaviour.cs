using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    public abstract class AssignScriptableValueBehaviour<T, U, V> : MonoBehaviour where T : ScriptableValue<U, V> where V : UnityEvent<U>, new()
    {
        [SerializeField()]
        protected T scriptableObjectValue;

        [SerializeField()]
        protected U @value;

        [SerializeField()]
        protected bool assignOnAwake;

        [SerializeField()]
        protected bool clearOnDestroy;

        protected virtual void Awake()
        {
            if (assignOnAwake)
                scriptableObjectValue.Value = @value;

        }

        private void OnDestroy()
        {
            if (clearOnDestroy && scriptableObjectValue.Value.Equals(@value))
                scriptableObjectValue.Clear();
        }

        public void Assign()
        {
            scriptableObjectValue.Value = value;
        }
    }
}