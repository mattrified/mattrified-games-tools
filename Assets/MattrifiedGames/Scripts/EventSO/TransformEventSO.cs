using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.EventSO
{
    [CreateAssetMenu(menuName = "MattrifiedGames/Transform EventSO")]
    public class TransformEventSO : BaseEventSO<Transform, UnityTransformEvent> { }
}