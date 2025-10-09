using System.Collections;
using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class TSWallCollider : MonoBehaviour
{
    public TSVector pointA;
    public TSVector pointB;
    public TSVector normal;

    public Color c;

    private void Awake()
    {
        normal = TSVector.Cross((pointB - pointA), TSVector.up).normalized;
    }

    private void OnValidate()
    {
        normal = TSVector.Cross((pointB - pointA), TSVector.up).normalized;
    }

    [ContextMenu("Flip")]
    public void Flip()
    {
        TSVector p = pointA;
        pointA = pointB;
        pointB = p;
    }

    public bool OnCorrectSide(TSVector point, FP radius)
    {
        TSVector diff = (point - normal * radius) - pointA;
        diff.Normalize();
        return TSVector.Dot(diff, normal) >= 0;
    }

    public FP DistanceFromPoint(TSVector point, FP radius)
    {
        point -= radius * normal;

        TSVector diff = pointB - pointA;


        return TSMath.Abs(diff.x * (pointA.z - point.z) - (pointA.x - point.x) * diff.z) /
            TSMath.Sqrt(diff.x * diff.x + diff.z * diff.z);

        /*public static double DistanceFromPointToLine(Point2D point, Line2D line)
        {
            // given a line based on two points, and a point away from the line,
            // find the perpendicular distance from the point to the line.
            // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
            // for explanation and defination.
            Point2D l1 = line.InitialPoint;
            Point2D l2 = line.TerminalPoint;

            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }*/
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = c;

        Gizmos.DrawLine(pointA.ToVector(), pointB.ToVector());
        Gizmos.DrawRay((pointA + pointB).ToVector() * 0.5f, normal.ToVector());
    }
}
