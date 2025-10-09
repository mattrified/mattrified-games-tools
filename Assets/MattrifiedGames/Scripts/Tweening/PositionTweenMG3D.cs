using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MattrifiedGames.MGTweening
{
    public class PositionTweenMG3D : MattrifiedTweenBaseVector3<Transform>
    {
        public override void UpdateTween()
        {
            target.position = TweenedValue;
        }
    }
}
