using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MattrifiedGames.MGTweening
{
    public class GraphicGradientColorTweenMG : MattrifiedTweenBaseFloat<Graphic>
    {
        public Gradient gradient;

        public override void UpdateTween()
        {
            target.color = gradient.Evaluate(TweenedValue);
        }
    }
}

