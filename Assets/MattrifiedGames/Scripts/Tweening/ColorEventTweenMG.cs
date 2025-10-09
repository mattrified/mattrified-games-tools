using MattrifiedGames.MGTweening;
using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColorEventTweenMG : MattrifiedTweenBaseColor<UnityColorEvent>
{
    public override void UpdateTween()
    {
        target.Invoke(TweenedValue);
    }
}
