using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TSWallColliderGroup : EnvironmentManager
{

    public TSCircle[] circle;

    public TSWallCollider[] walls;

    public override TSVector CheckPos(FP charRadius, TSVector position)
    {
        position = CheckBlockers(charRadius, position);

        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].OnCorrectSide(position, charRadius))
            {
                position += walls[i].DistanceFromPoint(position, charRadius) * walls[i].normal;
            }
        }

        for (int j = 0; j < circle.Length; j++)
        {
            if (!circle[j].wall.OnCorrectSide(position, charRadius))
            {
                TSVector diff = position - circle[j].center;
                diff.y = 0;
                FP mag = diff.magnitude;
                if (mag > circle[j].radius - charRadius)
                {
                    position -= diff.normalized * (diff.magnitude - (circle[j].radius - charRadius));
                }
                
            }
        }

        return position;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        for (int i = 0; i < walls.Length; i++)
        {
            if (!walls[i].OnCorrectSide(transform.position.ToTSVector(), FP.Half * FP.Half))
            {
                Gizmos.DrawRay(transform.position,
                    walls[i].DistanceFromPoint(transform.position.ToTSVector(), FP.Half * FP.Half).AsFloat() * walls[i].normal.ToVector());
            }
        }

        for (int i = 0; i < circle.Length; i++)
        {
            Gizmos.DrawWireSphere(circle[i].center.ToVector(), circle[i].radius.AsFloat());
        }

    }

    [System.Serializable()]
    public class TSCircle
    {
        public TSVector center;
        public FP radius;
        public TSWallCollider wall;
    }
}