using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.EventSO
{
    [CreateAssetMenu(menuName = "MattrifiedGames/ObjArrayEventSO")]
    public class ObjArrayEventSO : BaseEventSO<object[], UnityObjArrayEvent> { }

    [System.Serializable()]
    public class UnityObjArrayEvent : UnityEvent<object[]> { }
}