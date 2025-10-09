using System;
using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance
    {
        get;
        private set;
    }

    public bool useBox;

    protected TSBBox box;

    [SerializeField()]
    protected TSVector boxMin;

    [SerializeField()]
    protected TSVector boxMax;

    public FP radius;

    public RadialBlocker[] blockers;
    public CubicBlocker[] cBlockers;

    public bool useRingOut;
    public FP ringOutHeight;

    private void Awake()
    {
        Instance = this;
        box = new TSBBox(boxMin, boxMax);
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public virtual TSVector CheckPos(FP charRadius, TSVector position)
    {
        position = CheckBlockers(charRadius, position);

        if (!useBox)
        {
            if (position.magnitude > radius - charRadius)
            {
                if (!useRingOut)
                    position -= position.normalized * (position.magnitude - (radius - charRadius));
                else
                    position.y = ringOutHeight;
            }
        }
        else
        {
            if (!useRingOut)
            {
                if (position.x < box.min.x)
                    position.x = box.min.x;
                else if (position.x > box.max.x)
                    position.x = box.max.x;

                if (position.z < box.min.z)
                    position.z = box.min.z;
                else if (position.z > box.max.z)
                    position.z = box.max.z;
            }
            else
            {
                if (position.x < box.min.x ||
                    position.x > box.max.x ||
                    position.z < box.min.z ||
                    position.z > box.max.z)
                {
                    position.y = ringOutHeight;
                }
            }
        }

        return position;
    }

    protected TSVector CheckBlockers(FP charRadius, TSVector position)
    {
        for (int i = 0; i < blockers.Length; i++)
        {
            TSVector diff = blockers[i].center - position;
            diff.y = 0;
            if (diff.magnitude < charRadius + blockers[i].radius)
            {
                TSVector direction = diff.normalized;
                position -= direction * (charRadius + blockers[i].radius - diff.magnitude);
            }
        }

        for (int i = 0; i < cBlockers.Length; i++)
        {
            // clamp(value, min, max) - limits value to the range min..max

            // Find the closest point to the circle within the rectangle
            FP closestX = TSMath.Clamp(position.x, cBlockers[i].center.x - cBlockers[i].size.x * FP.Half,
                cBlockers[i].center.x + cBlockers[i].size.x * FP.Half);
            FP closestZ = TSMath.Clamp(position.z, cBlockers[i].center.z - cBlockers[i].size.z * FP.Half,
                cBlockers[i].center.z + cBlockers[i].size.z * FP.Half);

            // Calculate the distance between the circle's center and this closest point
            FP distanceX = position.x - closestX;
            FP distanceZ = position.z - closestZ;

            // If the distance is less than the circle's radius, an intersection occurs
            FP distanceSquared = (distanceX * distanceX) + (distanceZ * distanceZ);
            if (distanceSquared < (charRadius * charRadius))
            {
                position.x += charRadius * TSMath.Sign(distanceX) - distanceX;
                position.z += charRadius * TSMath.Sign(distanceZ) - distanceZ;
            }
        }

        return position;
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        if (!useBox)
            Gizmos.DrawWireSphere(Vector3.zero, radius.AsFloat());
        else
        {
            TSBBox b = new TSBBox(boxMin, boxMax);
            Gizmos.DrawWireCube(b.center.ToVector(), b.size.ToVector());
        }
    }
}
