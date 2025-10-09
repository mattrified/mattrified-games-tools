using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class LimbTwistScript : MonoBehaviour
{
    public bool readyToUse;
    public Transform drivingTransform;
    public Transform drivenTransform;
    public float percentage = 1f;
    public Vector2 limit;

    private void OnValidate()
    {
        readyToUse = drivingTransform != null && drivenTransform != null;
    }

    private void Update()
    {
        if (!readyToUse)
            return;

        Vector3 v = drivingTransform.up;

        Quaternion q = Quaternion.Slerp(Quaternion.identity, drivingTransform.localRotation, percentage);

        float vv = q.eulerAngles.y;
        //Debug.Log(vv);
        /*if (vv >= 270f || Mathf.Approximately(vv, 270f))
        {
            vv -= 180f;
        }*/

        q = Quaternion.Euler(0, vv, 0);

        drivenTransform.localRotation = q;
    }
}
