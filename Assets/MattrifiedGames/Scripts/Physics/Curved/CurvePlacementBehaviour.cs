using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class CurvePlacementBehaviour : MonoBehaviour
{
    public InGameCurve curve;
    public Vector3 pos;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            Update();
            enabled = false;
        }
    }

    void Update ()
    {
        if (curve == null)
            return;

        Vector3 v = transform.position;

        transform.localEulerAngles = new Vector3(0, curve.angleCurve.Evaluate(pos.x), 0);

        curve.GetFloorPoint(pos.x, ref v);

        v.y += pos.y;
        v += transform.right * pos.z;

        transform.position = v;
	}
}