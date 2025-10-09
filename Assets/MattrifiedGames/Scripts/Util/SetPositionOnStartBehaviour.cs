using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.Utility
{
    public class SetPositionOnStartBehaviour : MonoBehaviour
    {
        [SerializeField()]
        bool useWorldSpace;

        [SerializeField()]
        Vector3 position;

        [SerializeField()]
        public UnityEvent<float> setValuesOnStart;

        [ContextMenu("Place Object")]
        protected virtual void Start()
        {
            if (useWorldSpace)
                transform.position = position;
            else
                transform.localPosition = position;

            // Passes 0 for simplicity
            setValuesOnStart.Invoke(0f);
        }

        private void OnDestroy()
        {
            setValuesOnStart.RemoveAllListeners();
            setValuesOnStart = null;
        }
    }
}
