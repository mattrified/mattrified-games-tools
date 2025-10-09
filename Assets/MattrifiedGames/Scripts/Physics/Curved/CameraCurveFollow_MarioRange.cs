using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCurveFollow_MarioRange : MonoBehaviour
{
    public Transform transformToFollow;

    float centerPoint;

    [SerializeField()]
    float deadPointWidthOuter;

    [SerializeField()]
    float deadPointWidthInner;

    public bool focusRight = true;

    public CurvedMattrifiedPhysicsBehaviour follower;

    Vector3 pos;
    public Vector3 offset;
    Vector3 euler;

    public float x;

    public float posRate, rotRate;

    public float deadPointClampMin, deadPointClampMax = 0;
    
    public float deadOffset;

    public float focus;

    public float moveTowardsRate;

    private void Awake()
    {
        deadOffset = (focusRight ? deadPointWidthInner : -deadPointWidthInner);
        focus = follower.PositionX + deadOffset;
    }

    public void LateUpdate()
    {
        x = follower.PositionX;

        if (focusRight)
        {
            deadOffset = Mathf.MoveTowards(deadOffset, deadPointWidthInner, moveTowardsRate * Time.deltaTime);
            if (x > focus - deadOffset)
            {
                focus += x - (focus - deadOffset);
            }
            else if (x < focus - deadPointWidthOuter)
            {
                deadOffset = deadPointWidthOuter;
                focusRight = false;
            }
        }
        else
        {
            deadOffset = Mathf.MoveTowards(deadOffset, -deadPointWidthInner, moveTowardsRate * Time.deltaTime);
            if (x < focus - deadOffset)
            {
                focus += x - (focus - deadOffset);
            }
            else if (x > focus + deadPointWidthOuter)
            {
                deadOffset = -deadPointWidthOuter;
                focusRight = true;
            }
        }

        float finalPos = Mathf.Clamp(focus, deadPointClampMin, deadPointClampMax);

        follower.currentCurve.GetFloorPoint(finalPos, ref pos);

        follower.currentCurve.GetEulerY(finalPos, ref euler.y);
        
        transformToFollow.eulerAngles = euler;
        transformToFollow.position = Vector3.Lerp(transformToFollow.position,
            pos + transformToFollow.rotation * offset, posRate * Time.deltaTime);
    }
}
