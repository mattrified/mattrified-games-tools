using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.EventSO
{
    [CreateAssetMenu(menuName = "MattrifiedGames/Object EventSO")]
    public class ObjectEventSO : BaseEventSO<object, UnityBaseObjectEvent> { }

    [System.Serializable()]
    public class UnityBaseObjectEvent : UnityEvent<object> { }
}