using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CurvedPhysicsEnvironment : MonoBehaviour
{
    [SerializeField()]
    InGameCurve curve;

    [SerializeField()]
    EdgeCollider2D edgeCollider;

    [ContextMenu("Create Edge")]
    public void SetupEdgeCollider()
    {
        List<Vector2> edges = new List<Vector2>();

        float dist = curve.xCurve[curve.xCurve.length - 1].time;
        Vector3 pos = Vector3.zero;

        for (int i = 0; i <= 30; i++)
        {
            float percent = i / 30f;
            float d = dist * percent;
            curve.GetFloorPoint(d, ref pos);
            edges.Add(new Vector2(d, pos.y));
        }

        edgeCollider.points = edges.ToArray();
    }
}
