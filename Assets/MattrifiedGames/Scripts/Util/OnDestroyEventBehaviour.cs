using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.Utility
{
    public class OnDestroyEventBehaviour : MonoBehaviour
    {
        public UnityEvent onDestroyEvent;

        private void OnDestroy()
        {
            onDestroyEvent.Invoke();
        }
    }
}
