using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public struct RandomizedRange
{
    public bool randomize;

    public float min;

    public float max;

    public RandomizedRange(float min)
    {
        this.min = min;
        this.max = min;
        randomize = false;
    }

    public RandomizedRange(float min, float max)
    {
        this.min = Mathf.Min(min, max);
        this.max = Mathf.Max(min, max);
        this.randomize = true;
    }

    public float Value => randomize ? Random.Range(min, max) : min;
}

public class ClampRandomizedRangeAttribute : PropertyAttribute
{
    public bool useClamp;
    public float clampMin;
    public float clampMax;
}
    

