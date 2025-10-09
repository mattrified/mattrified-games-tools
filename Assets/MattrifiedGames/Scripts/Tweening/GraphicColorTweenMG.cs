using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace MattrifiedGames.MGTweening
{
    public class GraphicColorTweenMG : MattrifiedTweenBaseColor<Graphic>
    {
        public override void UpdateTween()
        {
            target.color = TweenedValue;
        }
    }
}

