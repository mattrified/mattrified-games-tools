using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformChildOrganizer : MonoBehaviour
{
    public bool organizeX;
    public bool organizeY;
    public bool organizeZ;
    public Vector3 localPosStartValue;
    public Vector3 localPosSpace;

    public bool organizeScale;
    public Vector3 startingLocalScale;
    public Vector3 localScaleSpacing;

    private void OnValidate()
    {
        var t = transform.childCount;
        for (int i = 0; i < t; i++)
        {
            var c = transform.GetChild(i);
            Organize(i, c);
        }
    }

    void Organize(int index, Transform child)
    {
        var lP = child.localPosition;
        bool[] organize = new bool[] { organizeX, organizeY, organizeZ };
        for (int i = 0; i < 3; i++)
        {
            if (organize[i])
            {
                lP[i] = localPosStartValue[i] + localPosSpace[i] * index;
            }
        }

        var lS = child.localScale;
        if (organizeScale)
        {
            lS = startingLocalScale + localScaleSpacing * index;
        }

        child.localPosition = lP;
        child.localScale = lS;
    }
}
