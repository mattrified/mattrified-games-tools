using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosOrientationConstraints : MonoBehaviour
{
    public Transform target;
    public bool pos, orientation;

    private void LateUpdate()
    {
        if (pos) transform.position = target.position;
        if (orientation) transform.rotation = target.rotation;
    }
}
