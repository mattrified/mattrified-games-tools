using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.CollisionHelp
{
    public class PathedTriggerEvents : TriggerEvents
    {
        public InGameCurveBase curve;

        protected override bool Validate(Collider other)
        {
            if (!base.Validate(other))
                return false;

            var cmb = other.GetComponent<CurvedMattrifiedPhysicsBehaviour>();
            return (cmb != null && cmb.currentCurve == curve);
        }
    }
}
