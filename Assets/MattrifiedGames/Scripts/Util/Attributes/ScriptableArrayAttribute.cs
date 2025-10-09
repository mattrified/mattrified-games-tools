using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AttributeUsage(AttributeTargets.Field)]
public class ScriptableArrayAttribute : PropertyAttribute
{
    public string n;
    public ScriptableArrayAttribute(string name)
    {
        this.n = name;
    }
}
