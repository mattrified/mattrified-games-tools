using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.CollisionHelp
{
    public class TriggerEvents : MonoBehaviour
    {
        [SerializeField()]
        UnityColliderEvent onTriggerEnterEvent, onTriggerStayEvent, onTriggerExitEvent;

        [SerializeField()]
        bool entered;

        protected virtual bool Validate(Collider other)
        {
            return enabled;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!Validate(other))
                return;

            onTriggerEnterEvent.Invoke(other);

            entered = true;
        }

        protected virtual void OnDisable()
        {
            if (entered)
            {
                onTriggerExitEvent.Invoke(null);
            }
            entered = false;
        }

        protected virtual void OnEnable()
        {
            entered = false;
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (!Validate(other))
            {
                // If we fail validation during stay but entered, we treat it as a valid exit.
                if (entered)
                {
                    onTriggerExitEvent.Invoke(other);
                    entered = false;
                }
                return;
            }

            if (!entered)
            {
                onTriggerEnterEvent.Invoke(other);
                entered = true;
                return;
            }

            onTriggerStayEvent.Invoke(other);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (entered)
            {
                entered = false;
                if (!Validate(other))
                    return;
                onTriggerExitEvent.Invoke(other);
            }
        }
    }

    [System.Serializable()]
    public class UnityColliderEvent : UnityEvent<Collider> { }
}