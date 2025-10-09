using UnityEngine;
using System.Collections;

public class CurvedPhysicsDriver : MonoBehaviour
{
    public Transform itemToDrive;
    public Rigidbody2D rigid;

    Vector3 localPos;
    Vector3 curvePos;
    public InGameCurve curve;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        localPos = rigid.transform.localPosition;
        
        curvePos.x = curve.xCurve.Evaluate(localPos.x);
        curvePos.y = localPos.y;
        curvePos.z = curve.zCurve.Evaluate(localPos.x);

        itemToDrive.position = curvePos;
    }
}
