using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

namespace TSFighter.Collision
{

    public static class TSCollisionHelper
    {
        public static bool CheckSphereCollision(TSVector center1, FP rad1, TSVector center2, FP rad2, out TSVector position)
        {
            position = TSVector.zero;

            TSVector diff = center1 - center2;
            if (diff.magnitude < rad1 + rad2)
            {
                position = FP.Half * (center1 + center2);
                return true;
            }

            return false;
        }

        public static bool CheckSphereCollision(TSSphere sphere1, TSSphere sphere2, out TSVector position)
        {
            position = TSVector.zero;

            TSVector diff = sphere1.center - sphere2.center;
            if (diff.magnitude < sphere1.radius + sphere2.radius)
            {
                position = FP.Half * (sphere1.center + sphere2.center);
                return true;
            }

            return false;
        }

        public static bool KeepCylinderOutOfCylinder(ref TSCylinder cylinderToMove, ref TSCylinder cylinderToStay)
        {
            if (!RangeIntersect(cylinderToMove.Bottom, cylinderToMove.Top, cylinderToStay.Bottom, cylinderToStay.Top))
                return false;

            TSVector diff = cylinderToMove.center - cylinderToStay.center;
            diff.y = 0;

            FP mag = diff.magnitude;
            FP radSum = cylinderToMove.radius + cylinderToStay.radius;
            if (mag < radSum)
            {
                cylinderToMove.center += diff.normalized * (radSum - mag);
                return true;
            }

            return false;
        }

        public static bool PushCylinders(ref TSCylinder cylinderToMove, ref TSCylinder cylinderToStay)
        {
            if (!RangeIntersect(cylinderToMove.Bottom, cylinderToMove.Top, cylinderToStay.Bottom, cylinderToStay.Top))
                return false;

            TSVector diff = cylinderToMove.center - cylinderToStay.center;
            diff.y = 0;

            FP mag = diff.magnitude;
            FP radSum = cylinderToMove.radius + cylinderToStay.radius;
            if (diff.magnitude < radSum)
            {
                cylinderToMove.center += diff.normalized * (radSum - mag);
                cylinderToStay.center -= diff.normalized * (radSum - mag);
                return true;
            }

            return false;
        }

        public static bool PushCylinders(TSVector bottomA, FP heightA, FP radA,
            TSVector bottomB, FP heightB, FP radB, out TSVector newPosA, out TSVector newPosB, TSVector forA, TSVector forB)
        {
            newPosA = bottomA;
            newPosB = bottomB;

            if (!RangeIntersect(bottomA.y, bottomA.y + heightA, bottomB.y, bottomB.y + heightB))
                return false;

            TSVector diff = bottomA - bottomB;
            diff.y = 0;

            FP mag = diff.magnitude;
            FP radSum = radA + radB;

            if (mag == 0)
            {
                if (heightA >= heightB)
                {
                    newPosA += forA * radSum * FP.Half;
                    newPosB -= forA * radSum * FP.Half;
                }
                else
                {
                    newPosA += forB * radSum * FP.Half;
                    newPosB -= forB * radSum * FP.Half;
                }
                return true;
            }

            if (mag < radSum)
            {
                newPosA += diff.normalized * (radSum - mag) * FP.Half;
                newPosB -= diff.normalized * (radSum - mag) * FP.Half;
                return true;
            }

            return false;
        }


        public static bool KeepCircleOutOfCylinder(ref TSSphere sphere, ref TSCylinder cylinder)
        {
            if (!RangeIntersect(sphere.Bottom, sphere.Top, cylinder.Bottom, cylinder.Top))
                return false;

            TSVector diff = sphere.center - cylinder.center;
            diff.y = 0;

            FP mag = diff.magnitude;
            FP radSum = sphere.radius + cylinder.radius;
            if (diff.magnitude < radSum)
            {
                sphere.center += diff.normalized * (radSum - mag);
                return true;
            }

            return false;
        }

        public static FP FlatDistance(TSVector v1, TSVector v2)
        {
            v1.y = 0;
            v2.y = 0;

            return TSVector.Distance(v1, v2);
        }

        public static bool CircleCollision(FP xA, FP yA, FP radA, FP xB, FP yB, FP radB)
        {
            FP dX = xA - xB;
            dX *= dX;

            FP dY = yA - yB;
            dY *= dY;

            FP radSumSqr = radA + radB;
            radSumSqr *= radSumSqr;

            return dX + dY < radSumSqr;

        }

        public static bool RangeIntersect(FP minA, FP maxA, FP minB, FP maxB)
        {
            return ((minA <= minB && minB <= maxA) || (minB <= minA && minA <= maxB));
        }
    }

    [System.Serializable()]
    public struct TSCylinder
    {
        public TSVector center;

        public FP radius;

        public FP height;

        public FP Top
        {
            get
            {
                return center.y + height * FP.Half;
            }
        }

        public FP Bottom
        {
            get
            {
                return center.y - height * FP.Half;
            }
        }
    }

    [System.Serializable()]
    public struct TSSphere
    {
        public TSVector center;
        public FP radius;

        public FP Top
        {
            get
            {
                return center.y + radius;
            }
        }

        public FP Bottom
        {
            get
            {
                return center.y - radius;
            }
        }
    }

    [System.Serializable()]
    public struct TSPlane
    {
        public TSVector position;
        public TSVector normal;
    }

    [System.Serializable()]
    public struct TSLine2
    {
        public TSVector2 start;
        public TSVector2 end;

        public static void Intersection(TSLine2 S1, TSLine2 S2, out TSVector2 vA, out TSVector2 vB, out FP sqrDistance)
        {
            // Done to prevent issues with points
            TSVector2 u = S1.end - S1.start;
            if (u.LengthSquared() == FP.Zero)
                u = TSVector2.up * FP.Epsilon;

            TSVector2 v = S2.end - S2.start;
            if (v.LengthSquared() == FP.Zero)
                v = TSVector2.up * FP.Epsilon;

            TSVector2 w = S1.start - S2.start;
            FP a = TSVector2.Dot(u, u);         // always >= 0
            FP b = TSVector2.Dot(u, v);
            FP c = TSVector2.Dot(v, v);         // always >= 0
            FP d = TSVector2.Dot(u, w);
            FP e = TSVector2.Dot(v, w);
            FP D = a * c - b * b;        // always >= 0
            FP sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
            FP tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < FP.Epsilon)
            { // the lines are almost parallel
                sN = FP.Zero;         // force using point P0 on segment S1
                sD = FP.One;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {                 // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < FP.Zero)
                {        // sc < 0 => the s=0 edge is visible
                    sN = FP.Zero;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < FP.Zero)
            {            // tc < 0 => the t=0 edge is visible
                tN = FP.Zero;
                // recompute sc for this edge
                if (-d < FP.Zero)
                    sN = FP.Zero;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {      // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < FP.Zero)
                    sN = FP.Zero;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            //sc = (TSMath.Abs(sN) < FP.Zero ? FP.Zero : sN / sD);
            //tc = (TSMath.Abs(tN) < FP.Zero ? FP.Zero : tN / tD);

            sc = sN / sD;
            tc = tN / tD;

            // get the difference of the two closest points
            //TSVector dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

            vA = S1.start + sc * u;
            vB = S2.start + tc * v;

            sqrDistance = (vB - vA).LengthSquared();
            //return dP;// norm(dP);   // return the closest distance
        }
    }

    [System.Serializable()]
    public struct TSLine
    {
        public TSVector start;
        public TSVector end;

        public static void IntersectionTest2(TSLine L1, TSLine L2, out TSVector vA, out TSVector vB, out FP dist)
        {
            TSVector u = L1.end - L1.start;
            TSVector v = L2.end - L2.start;
            TSVector w = L1.start - L2.start;
            FP a = TSVector.Dot(u, u);         // always >= 0
            FP b = TSVector.Dot(u, v);
            FP  c = TSVector.Dot(v, v);         // always >= 0
            FP d = TSVector.Dot(u, w);
            FP e = TSVector.Dot(v, w);
            FP D = a * c - b * b;        // always >= 0
            FP sc, tc;

            // compute the line parameters of the two closest points
            if (D < FP.Zero)
            {          // the lines are almost parallel
                sc = 0.0;
                tc = (b > c ? d / b : e / c);    // use the largest denominator
            }
            else
            {
                sc = (b * e - c * d) / D;
                tc = (a * e - b * d) / D;
            }

            // get the difference of the two closest points
            TSVector dP = w + (sc * u) - (tc * v);  // =  L1(sc) - L2(tc)

            vA = L1.start + sc * u;
            vB = L2.start + tc * v;


            dist = 0;
            //return norm(dP);   // return the closest distance
        }

        public static void Intersection(TSLine S1, TSVector point, out TSVector vA, out TSVector vB, out FP sqrDistance)
        {
            Intersection(S1, new TSLine() { start = point, end = point + TSVector.up * FP.EN3 }, out vA, out vB, out sqrDistance);
        }

        public static void Intersection(TSLine S1, TSLine S2, out TSVector vA, out TSVector vB, out FP sqrDistance)
        {
            // Done to prevent issues with points
            TSVector u = S1.end - S1.start;
            if (u.sqrMagnitude == FP.Zero)
                u = TSVector.up * FP.Epsilon;

            TSVector v = S2.end - S2.start;
            if (v.sqrMagnitude == FP.Zero)
                v = TSVector.up * FP.Epsilon;

            TSVector w = S1.start - S2.start;
            FP a = TSVector.Dot(u, u);         // always >= 0
            FP b = TSVector.Dot(u, v);
            FP c = TSVector.Dot(v, v);         // always >= 0
            FP d = TSVector.Dot(u, w);
            FP e = TSVector.Dot(v, w);
            FP D = a * c - b * b;        // always >= 0
            FP sc, sN, sD = D;       // sc = sN / sD, default sD = D >= 0
            FP tc, tN, tD = D;       // tc = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < FP.Epsilon) { // the lines are almost parallel
                sN = FP.Zero;         // force using point P0 on segment S1
                sD = FP.One;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {                 // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < FP.Zero)
                {        // sc < 0 => the s=0 edge is visible
                    sN = FP.Zero;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {  // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < FP.Zero)
            {            // tc < 0 => the t=0 edge is visible
                tN = FP.Zero;
                // recompute sc for this edge
                if (-d < FP.Zero)
                    sN = FP.Zero;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {      // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < FP.Zero)
                    sN = FP.Zero;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }
            // finally do the division to get sc and tc
            //sc = (TSMath.Abs(sN) < FP.Zero ? FP.Zero : sN / sD);
            //tc = (TSMath.Abs(tN) < FP.Zero ? FP.Zero : tN / tD);

            sc = sN / sD;
            tc = tN / tD;

            // get the difference of the two closest points
            //TSVector dP = w + (sc * u) - (tc * v);  // =  S1(sc) - S2(tc)

            vA = S1.start + sc * u;
            vB = S2.start + tc * v;

            sqrDistance = (vB - vA).sqrMagnitude;
            //return dP;// norm(dP);   // return the closest distance
        }


        
    }
    

    [System.Serializable()]
    public struct TSRay
    {
        public TSVector origin;
        public TSVector direction;
    }

    [System.Serializable()]
    public struct TSCapsule
    {
        public TSLine line;
        public FP radius;
    }
}