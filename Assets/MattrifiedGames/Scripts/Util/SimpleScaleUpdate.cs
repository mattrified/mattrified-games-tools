using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleScaleUpdate : MonoBehaviour
{
    public Transform parentTransform;
    private Transform tr;

    private void Awake()
    {
        tr = transform;
    }

    private void Update()
    {
        tr.localScale = parentTransform.localScale;
    }
}
