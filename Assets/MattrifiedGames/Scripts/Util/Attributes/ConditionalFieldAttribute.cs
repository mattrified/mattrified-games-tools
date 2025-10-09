using System;
using UnityEngine;
using UnityEngine.Events;

[AttributeUsage(AttributeTargets.Field)]
public class ConditionalFieldAttribute : PropertyAttribute
{
    public string PropertyToCheck;

    public object CompareValue;

    public ConditionalFieldAttribute(string propertyToCheck, object compareValue = null)
    {
        PropertyToCheck = propertyToCheck;
        CompareValue = compareValue;
    }
}

/// <summary>
/// Object that uses the conditional field built in for fast access.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class ConditionalObject<T>
{
    [ConditionalField("use", true)]
    public bool use;

    public T value;
}

/// <summary>
/// Uses event base for fast conditional access.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class EventPairBase<T> : ConditionalObject<T> where T : UnityEventBase
{
}