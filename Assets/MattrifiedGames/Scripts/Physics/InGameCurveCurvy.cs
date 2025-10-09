#if USING_CURVY
using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Generator;

public class InGameCurveCurvy : InGameCurveBase
{
    [SerializeField()]
    CurvySpline curveySpline;

    [SerializeField()]
    YSplineModuleBase floorCurve;

    [SerializeField()]
    YSplineModuleBase ceilingCurve;

    protected override void Awake()
    {
        base.Awake();
    }

    public override bool Loop
    {
        get
        {
            return curveySpline.Closed;
        }
    }

    public override float Percent(float xPos)
    {
        return xPos / distance;
    }

    public float TFPercent(float xPos)
    {
        return curveySpline.DistanceToTF(xPos, Loop ? CurvyClamping.Loop : CurvyClamping.Clamp);
    }

    public override void DefineDistance()
    {
        distance = curveySpline.Length;//.TFToDistance(1f, CurvyClamping.Clamp);
    }

    internal override void GetFloorPoint(float distance, ref Vector3 vector3)
    {
        float percent = Percent(distance);
        
        vector3 = curveySpline.Interpolate(percent);
        vector3.y = floorCurve.yCurve.Evaluate(percent) + floorCurve.Offset;
    }

    internal override void GetCeilingPoint(float distance, ref Vector3 vector3)
    {
        float percent = Percent(distance);
        vector3 = curveySpline.Interpolate(percent);
        vector3.y = ceilingCurve.yCurve.Evaluate(percent) + floorCurve.Offset;
    }

    internal override void GetEulerY(float distance, ref float euler)
    {
        float percent = Percent(distance);
        if (curveySpline.IsInitialized)
        {
            euler = curveySpline.GetOrientationFast(percent).eulerAngles.y;
        }
    }

    internal override void GetAllFloorCeilAngleY(float distance, ref Vector3 floor, ref Vector3 ceiling, ref float euler)
    {
        DefineDistance();

        if (!curveySpline.IsInitialized)
        {
            return;
        }

        float percent = Percent(distance);
        float tfPercent = TFPercent(distance);

        floor = curveySpline.Interpolate(tfPercent);
        ceiling = floor;

        floor.y = floorCurve.yCurve.Evaluate(percent) + floorCurve.Offset;
        ceiling.y = ceilingCurve.yCurve.Evaluate(percent) + ceilingCurve.Offset;

        euler = curveySpline.GetOrientationFast(tfPercent).eulerAngles.y;
    }
}
#endif