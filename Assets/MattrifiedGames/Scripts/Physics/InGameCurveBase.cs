using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls 2D Curves
/// TODO:  Add 2d physics simulation and 3D rendering environment stuff.
/// Unsure if players should also work on the 2D simulation layer yet.
/// This may be a MerFight only thing as the fighting game can't do this.
/// </summary>
public abstract class InGameCurveBase : MonoBehaviour
{
    public int curveID;

    public Gradient color;

    [SerializeField()]
    protected InGameCurveList curveList;

    [SerializeField()]
    public CameraCurveFollow associatedCamera;

    public float distance;

    public abstract bool Loop
    {
        get;
    }

    protected virtual void Awake()
    {
        DefineDistance();

        if (curveList != null)
            curveList.Add(this);
    }

    public abstract float Percent(float xPos);

    [ContextMenu("Define Distance")]
    public abstract void DefineDistance();

    protected virtual void OnDestroy()
    {
        if (curveList != null)
            curveList.Remove(this);
    }

    internal abstract void GetFloorPoint(float distance, ref Vector3 vector3);

    internal abstract void GetCeilingPoint(float distance, ref Vector3 vector3);

    internal abstract void GetEulerY(float distance, ref float euler);

    internal abstract void GetAllFloorCeilAngleY(float distance, ref Vector3 floor, ref Vector3 ceiling, ref float angle);

    public void FocusOnCurve()
    {
        if (associatedCamera != null)
            associatedCamera.Focus();
    }

    public void UnfocusOnCurve()
    {
        if (associatedCamera != null)
            associatedCamera.UnFocus();
    }
}