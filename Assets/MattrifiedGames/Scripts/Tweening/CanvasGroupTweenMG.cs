using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MattrifiedGames.MGTweening
{
    public class CanvasGroupTweenMG : MattrifiedTweenBaseFloat<CanvasGroup>
    {
        public override void UpdateTween()
        {
            target.alpha = TweenedValue;
        }
    }
}