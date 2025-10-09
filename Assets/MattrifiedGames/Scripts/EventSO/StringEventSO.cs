using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.EventSO
{
    [CreateAssetMenu(menuName = "MattrifiedGames/String EventSO")]
    public class StringEventSO : BaseEventSO<string, UnityBaseStringEvent> { }

    [System.Serializable()]
    public class UnityBaseStringEvent : UnityEvent<string> { }
}