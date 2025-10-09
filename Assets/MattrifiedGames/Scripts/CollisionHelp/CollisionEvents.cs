using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.CollisionHelp
{
    public class CollisionEvents : MonoBehaviour
    {
        [SerializeField()]
        UnityCollisionEvent onCollisionEnterEvent, onCollisionStayEvent, onCollisionExitEvent;

        private void OnCollisionEnter(Collision collision)
        {
            onCollisionEnterEvent.Invoke(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            onCollisionStayEvent.Invoke(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            onCollisionExitEvent.Invoke(collision);
        }
    }

    [System.Serializable()]
    public class UnityCollisionEvent : UnityEvent<Collision> { }
}