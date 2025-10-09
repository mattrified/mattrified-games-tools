using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class CubicBlocker : MonoBehaviour
{
    [SerializeField()]
    public TSVector center;

    [SerializeField()]
    public TSVector size;

    [ContextMenu("Snap From Pos")]
    public void SnapFromPos()
    {
        center = new TSVector(transform.position.x, transform.position.y, transform.position.z);
        size = new TSVector(transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center.ToVector(), size.ToVector());
    }

}


[System.Serializable()]
public struct TSBBox
{
    public TSVector min, max;

    public TSVector center
    {
        get
        {
            return (min + max) * FP.Half;
        }

        set
        {
            var diff = value - center;
            min += diff;
            max += diff;
        }
    }

    public TSVector size
    {
        get
        {
            return max - min;
        }
    }

    public TSBBox(TSVector min, TSVector max)
    {
        this.min = min;
        this.max = max;
    }
}