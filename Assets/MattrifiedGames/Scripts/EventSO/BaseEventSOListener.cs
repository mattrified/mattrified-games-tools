using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.EventSO
{

    public abstract class BaseEventSOListener : MonoBehaviour
    {
        [SerializeField(), Tooltip("If true, the listener will be updated on awake and destroy; otherwise, enable and disable.")]
        public bool onAwakeDestroy = true;

        public UnityEvent unityEvent;
        public BaseEventSO eventSO;

        protected void Awake()
        {
            if (onAwakeDestroy)
            {
                eventSO.AddListener(unityEvent.Invoke);
            }
        }

        protected void OnDestroy()
        {
            if (onAwakeDestroy)
            {
                eventSO.RemoveListener(unityEvent.Invoke);
            }
        }

        protected void OnEnable()
        {
            if (!onAwakeDestroy)
            {
                eventSO.AddListener(unityEvent.Invoke);
            }
        }

        protected void OnDisable()
        {
            if (!onAwakeDestroy)
            {
                eventSO.RemoveListener(unityEvent.Invoke);
            }
        }
    }

    public abstract class BaseEventSOListener<T, U, V> : MonoBehaviour where U : BaseEventSO<T, V> where V : UnityEvent<T>
    {
        public bool onAwakeDestroy;
        public V unityEvent;
        public U eventSO;

        protected void Awake()
        {
            if (onAwakeDestroy)
            {
                eventSO.AddListener(unityEvent.Invoke);
            }
        }

        protected void OnDestroy()
        {
            if (onAwakeDestroy)
            {
                eventSO.RemoveListener(unityEvent.Invoke);
            }
        }

        protected void OnEnable()
        {
            if (!onAwakeDestroy)
            {
                eventSO.AddListener(unityEvent.Invoke);
            }
        }

        protected void OnDisable()
        {
            if (!onAwakeDestroy)
            {
                eventSO.RemoveListener(unityEvent.Invoke);
            }
        }
    }
}