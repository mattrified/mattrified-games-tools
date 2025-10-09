using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class RadialBlocker : MonoBehaviour
{
    [SerializeField()]
    public TSVector center;

    [SerializeField()]
    public FP radius;

    [ContextMenu("Snap From Pos")]
    public void SnapFromPos()
    {
        center = new TSVector(transform.position.x, transform.position.y, transform.position.z);
        radius = transform.localScale.x;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center.ToVector(), radius.AsFloat());
    }

}
