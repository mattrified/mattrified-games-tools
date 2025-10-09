using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.Utility
{
    public class KeycodeEventBehaviour : MonoBehaviour
    {
        [System.Serializable()]
        public class KeycodeEvent
        {
            public KeyCode key;
            public UnityEvent @event;
        }

        public KeycodeEvent[] onPress;

        public KeycodeEvent[] isPressed;
        public KeycodeEvent[] notPressed;
        public KeycodeEvent[] onRelease;

        private void Update()
        {
            for (int i = 0; i < onPress.Length; i++)
            {
                if (Input.GetKeyDown(onPress[i].key))
                    onPress[i].@event.Invoke();
            }

            for (int i = 0; i < isPressed.Length; i++)
            {
                if (Input.GetKey(isPressed[i].key))
                    isPressed[i].@event.Invoke();
            }

            for (int i = 0; i < notPressed.Length; i++)
            {
                if (!Input.GetKey(notPressed[i].key))
                    notPressed[i].@event.Invoke();
            }

            for (int i = 0; i < onRelease.Length; i++)
            {
                if (Input.GetKeyUp(onRelease[i].key))
                    onRelease[i].@event.Invoke();
            }
        }
    }
}
