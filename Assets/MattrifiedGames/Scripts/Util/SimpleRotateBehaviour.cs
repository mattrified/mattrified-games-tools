using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotateBehaviour : MonoBehaviour
{
    public Vector3 rotationRate;
    public Space space = Space.Self;

    private void Update()
    {
        transform.Rotate(rotationRate * Time.deltaTime, space);
    }
}
