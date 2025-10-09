using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TSTriangleCollider : MonoBehaviour
{
    public TSTriangle[] triangles = new TSTriangle[] { TSTriangle.Default };

    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

#if UNITY_EDITOR

    public Transform checkingTransform;

    private void OnDrawGizmos()
    {
        foreach (TSTriangle triangle in triangles)
        {
            Gizmos.color = triangle.color;
            Gizmos.DrawLine(triangle.a.ToVector(), triangle.c.ToVector());
            Gizmos.DrawLine(triangle.b.ToVector(), triangle.c.ToVector());
            Gizmos.DrawLine(triangle.a.ToVector(), triangle.b.ToVector());

            if (triangle.abWall)
            {
                Gizmos.DrawLine(triangle.ABMid.ToVector(), triangle.ABMid.ToVector() + Vector3.up);
                Gizmos.DrawRay(triangle.ABMid.ToVector(), Vector3.Cross((triangle.b - triangle.a).normalized.ToVector(),
                    Vector3.up));
            }

            if (triangle.acWall)
            {
                Gizmos.DrawLine(triangle.ACMid.ToVector(), triangle.ACMid.ToVector() + Vector3.up);
                Gizmos.DrawRay(triangle.ACMid.ToVector(), Vector3.Cross((triangle.c - triangle.a).normalized.ToVector(),
                    Vector3.up));
            }

            if (triangle.bcWall)
            {
                Gizmos.DrawLine(triangle.BCMid.ToVector(), triangle.BCMid.ToVector() + Vector3.up);
                Gizmos.DrawRay(triangle.BCMid.ToVector(), Vector3.Cross((triangle.c - triangle.b).normalized.ToVector(),
                    Vector3.up));
            }

            if (triangle.PointContainedXZ(checkingTransform.position.ToTSVector()))
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(checkingTransform.position, 0.25f);

                Gizmos.color = Color.magenta;
                TSVector pos = checkingTransform.position.ToTSVector();
                triangle.SetYPoint(ref pos);
                Gizmos.DrawLine(checkingTransform.position, pos.ToVector());
            }
        }
    }

#endif

}

[System.Serializable()]
public struct TSTriangle
{
    public static readonly TSTriangle Default = new TSTriangle()
    {
        a = new TSVector(-1, 0, 0),
        b = new TSVector(1, 0, 0),
        c = new TSVector(0, 0, 1),
#if UNITY_EDITOR
        color = Color.green,
#endif
    };

    public bool positiveNormal;
    public TSVector a, b, c;

    public bool abWall;
    public bool acWall;
    public bool bcWall;

    public TSVector ABMid
    {
        get
        {
            return FP.Half * (a + b);
        }
    }

    public TSVector ACMid
    {
        get
        {
            return FP.Half * (a + c);
        }
    }

    public TSVector BCMid
    {
        get
        {
            return FP.Half * (c + b);
        }
    }

    public TSVector Normal
    {
        get
        {
            TSVector normal = TSVector.Cross(b - a, c - a);
            if (positiveNormal && normal.y < 0)
                normal *= -1;



            return normal;
        }

    }

    public void SetYPoint(ref TSVector point)
    {
        FP det = (b.z - c.z) * (a.x - c.x) + (c.x - b.x) * (a.z - c.z);

        FP l1 = ((b.z - c.z) * (point.x - c.x) + (c.x - b.x) * (point.z - c.z)) / det;
        FP l2 = ((c.z - a.z) * (point.x - c.x) + (a.x - c.x) * (point.z - c.z)) / det;
        FP l3 = FP.One - l1 - l2;

        point.y = l1 * a.y + l2 * b.y + l3 * c.y;

        /*
         * https://www.gamedev.net/forums/topic/597393-getting-the-height-of-a-point-on-a-triangle/
         * 
         * loat calcY(vec3 p1, vec3 p2, vec3 p3, float x, float z) {
float det = (p2.z - p3.z) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.z - p3.z);

float l1 = ((p2.z - p3.z) * (x - p3.x) + (p3.x - p2.x) * (z - p3.z)) / det;
float l2 = ((p3.z - p1.z) * (x - p3.x) + (p1.x - p3.x) * (z - p3.z)) / det;
float l3 = 1.0f - l1 - l2;

return l1 * y1 + l2 * y2 + l3 * y3;*/
    }

    public TSVector Center
    {
        get
        {
            return (a + b + c) / 3;
        }
    }

    public bool PointContainedXZ(TSVector point)
    {
        TSVector v0 = c - a;
        TSVector v1 = b - a;
        TSVector v2 = point - a;

        v0.y = 0;
        v1.y = 0;
        v2.y = 0;

        FP dot00 = TSVector.Dot(v0, v0);
        FP dot01 = TSVector.Dot(v0, v1);
        FP dot02 = TSVector.Dot(v0, v2);
        FP dot11 = TSVector.Dot(v1, v1);
        FP dot12 = TSVector.Dot(v1, v2);

        FP invDenom = FP.One / (dot00 * dot11 - dot01 * dot01);
        FP u = (dot11 * dot02 - dot01 * dot12) * invDenom;
        FP v = (dot00 * dot12 - dot01 * dot02) * invDenom;

        // Check if point is in triangle
        return (u >= 0) && (v >= 0) && (u + v < 1);
    }

#if UNITY_EDITOR
    public Color color;
#endif
}