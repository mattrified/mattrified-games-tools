using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable()]
public struct IntRange
{
    [SerializeField()]
    int min;

    [SerializeField()]
    int max;

    public IntRange(int min, int max)
    {
        this.min = Mathf.Min(min, max);
        this.max = Mathf.Max(min, max);
    }

    public int Min
    {
        get
        {
            return min;
        }

        set
        {
            if (value <= max)
                min = value;
            else
            {
                min = max;
                max = value;
            }
        }
    }

    public int Max
    {
        get
        {
            return max;
        }

        set
        {
            if (value >= min)
                max = value;
            else
            {
                max = min;
                min = value;
            }
        }
    }

    public int Random
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    public int Length
    {
        get
        {
            return max - min;
        }
    }

    public void SetMinMax(int min, int max)
    {
        min = Mathf.Min(min, max);
        max = Mathf.Max(min, max);
    }
}

[System.Serializable()]
public struct FloatRange
{
    [SerializeField()]
    float min;

    [SerializeField()]
    float max;

    public FloatRange(float min, float max)
    {
        this.min = Mathf.Min(min, max);
        this.max = Mathf.Max(min, max);
    }

    public float Min
    {
        get
        {
            return min;
        }

        set
        {
            if (value <= max)
                min = value;
            else
            {
                min = max;
                max = value;
            }
        }
    }

    public float Max
    {
        get
        {
            return max;
        }

        set
        {
            if (value >= min)
                max = value;
            else
            {
                max = min;
                min = value;
            }
        }
    }

    public float Random
    {
        get
        {
            return UnityEngine.Random.Range(min, max);
        }
    }

    public float Length
    {
        get
        {
            return max - min;
        }
    }

    public void SetMinMax(int min, int max)
    {
        min = Mathf.Min(min, max);
        max = Mathf.Max(min, max);
    }

    public bool InRange(float value)
    {
        return value >= min && value <= max;
    }
}

[System.Serializable()]
public struct ClampedInt
{
    [SerializeField()]
    IntRange range;

    [SerializeField()]
    int value;

    public int Min { get { return range.Min; } set { range.Min = value; } }
    public int Max { get { return range.Max; } set { range.Max = value; } }
    
    public int ClampedValue { get { return value; } set { this.value = Mathf.Clamp(value, range.Min, range.Max); } }

    
    public int Random { get { return range.Random; } }

    public int Length { get { return range.Length; } }

    public float Percent
    {
        get
        {
            return Mathf.InverseLerp(Min, Max, value);
        }

        set
        {
            this.value = Mathf.RoundToInt(Mathf.Lerp(Min, Max, value));
        }
    }

    public void SetMinMax(int min, int max)
    {
        range.SetMinMax(min, max);
        value = Mathf.Clamp(value, range.Min, range.Max);
    }
}

[System.Serializable()]
public struct ClampedFloat
{
    [SerializeField()]
    FloatRange range;

    [SerializeField()]
    float value;

    public float Min { get { return range.Min; } set { range.Min = value; } }
    public float Max { get { return range.Max; } set { range.Max = value; } }

    public float Value { get { return value; } set { this.value = Mathf.Clamp(value, range.Min, range.Max); } }


    public float Random { get { return range.Random; } }

    public float Length { get { return range.Length; } }

    public float Percent
    {
        get
        {
            return Mathf.InverseLerp(Min, Max, value);
        }

        set
        {
            this.value = Mathf.Lerp(Min, Max, value);
        }
    }

    public void SetMinMax(int min, int max)
    {
        range.SetMinMax(min, max);
        value = Mathf.Clamp(value, range.Min, range.Max);
    }
}