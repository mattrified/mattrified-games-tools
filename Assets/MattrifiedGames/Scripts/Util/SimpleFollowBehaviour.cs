using UnityEngine;
using System.Collections;

public class SimpleFollowBehaviour : MonoBehaviour
{
    public Transform other;

    public float posRate, rotRate;

    private void Awake()
    {
        if ((object)other == null)
            return;

        transform.position = other.position;
        transform.rotation = other.rotation;
    }
    private void LateUpdate()
    {
        if ((object)other == null)
            return;

        transform.position = Vector3.Lerp(transform.position, other.position, Time.deltaTime * posRate);
        transform.rotation = Quaternion.Slerp(transform.rotation, other.rotation, Time.deltaTime * rotRate);
    }
}
