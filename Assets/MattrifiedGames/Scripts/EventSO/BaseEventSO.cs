using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.EventSO
{

    public abstract class BaseEventSO : ScriptableObject
    {
        [SerializeField()]
        private UnityEvent unityEvent;

        public void Raise()
        {
            unityEvent.Invoke();
        }

        public void AddListener(UnityAction act)
        {
            unityEvent.AddListener(act);
        }

        public void RemoveListener(UnityAction act)
        {
            unityEvent.RemoveListener(act);
        }

        public void ClearAllListeners()
        {
            unityEvent.RemoveAllListeners();
        }
    }

    public abstract class BaseEventSO<T, U> : ScriptableObject where U : UnityEvent<T>
    {
        [SerializeField()]
        private U unityEvent;

        public void Raise(T item)
        {
            unityEvent.Invoke(item);
        }

        public void AddListener(UnityAction<T> act)
        {
            unityEvent.AddListener(act);
        }

        public void RemoveListener(UnityAction<T> act)
        {
            unityEvent.RemoveListener(act);
        }

        public void ClearAllListeners()
        {
            unityEvent.RemoveAllListeners();
        }
    }
}