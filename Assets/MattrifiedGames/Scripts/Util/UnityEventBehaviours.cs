using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.Utility
{
    public class UnityEventBehaviours : MonoBehaviour
    {
        public enum UnityActionType
        {
            OnEnable = 0,
            OnAwake = 1,
            OnStart = 2,
            OnDisable = 3,
            OnDestroy = 4,
        }

        public UnityActionType actionType;
        public UnityEvent e;

        private void OnEnable()
        {
            if (actionType == UnityActionType.OnEnable)
                e.Invoke();
        }

        private void Start()
        {
            if (actionType == UnityActionType.OnStart)
                e.Invoke();
        }

        private void Awake()
        {
            if (actionType == UnityActionType.OnAwake)
                e.Invoke();
        }

        private void OnDisable()
        {
            if (actionType == UnityActionType.OnDisable)
                e.Invoke();
        }

        private void OnDestroy()
        {
            if (actionType == UnityActionType.OnDestroy)
                e.Invoke();
        }

    }
}
