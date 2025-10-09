using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCurveFollow : MonoBehaviour
{
    public Transform transformToFollow;

    [SerializeField()]
    float deadPoint;

    [SerializeField()]
    float deadZoneWidth;

    public CurvedMattrifiedPhysicsBehaviour follower;
    public InGameCurveBase curve;

    Vector3 pos;
    public Vector3 offset;
    Vector3 euler;

    public float posRate, rotRate;

    public float deadPointClampMin, deadPointClampMax = 0;

#if USING_CINEMACHINE
    [SerializeField()]
    public Cinemachine.CinemachineVirtualCamera virtualCamera;
#endif

    private void Awake()
    {
        deadPoint = follower.PositionX;
    }

    public void LateUpdate()
    {
        if (follower.currentCurve != curve)
            return;

        var x = follower.PositionX;
        float deadZoneMin = deadPoint - deadZoneWidth;
        float deadZoneMax = deadPoint + deadZoneWidth;
        if (x < deadZoneMin)
        {
            deadPoint += x - deadZoneMin;
        }
        else if (x > deadZoneMax)
        {
            deadPoint += x - deadZoneMax;
        }

        if (!curve.Loop)
            deadPoint = Mathf.Clamp(deadPoint, deadPointClampMin, curve.distance - deadPointClampMax);

        Vector3 pp = Vector3.zero;
        curve.GetAllFloorCeilAngleY(deadPoint, ref pos, ref pp, ref euler.y);

        transformToFollow.eulerAngles = euler;
        transformToFollow.position = pos + transformToFollow.rotation * offset;
    }

    internal void Focus()
    {
#if USING_CINEMACHINE
        if (virtualCamera != null)
            virtualCamera.Priority = 10;
#endif
    }

    internal void UnFocus()
    {
#if USING_CINEMACHINE
        if (virtualCamera != null)
            virtualCamera.Priority = 0;
#endif
    }
}
