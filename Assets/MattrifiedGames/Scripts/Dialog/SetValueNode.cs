#if USING_SGF

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG.Vignettitor;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.Graph.NodeViews;
using System;

/// <summary>
/// Jump nodes transition immediately to their only child node and are 
/// meant to serve an organizational purpose but not a functional purpose.
/// </summary>
[NodeMenu("Set Value", typeof(VignetteGraph))]
public class SetValueNode : VignetteNode
{
    public IntScriptableValue intScriptableValue = null;
    public int amount;
    public bool increment;

    public override OutputRule OutputRule
    {
        get
        {
            return OutputRule.Passthrough();
        }
    }

    internal void UpdateValue()
    {
        if ((object)intScriptableValue == null)
            Debug.LogWarning(this.name + " has no integer assigned.");
        else
        {
            if (increment)
                intScriptableValue.Value += amount;
            else
                intScriptableValue.Value = amount;
        }
    }
}

/// <summary>
/// Draws a vignettitor representation of a jump node, which includes a 
/// rendering of the node it will jump to.
/// </summary>
[NodeView(typeof(SetValueNode))]
public class SetValueNodeView : VignetteNodeView
{
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        var n = Node as SetValueNode;

        if (n.intScriptableValue != null)
            GUILayout.Label(n.intScriptableValue.name);

        if (n.increment)
            GUILayout.Label("Increment by " + n.amount);
        else
            GUILayout.Label("Set to " + n.amount);
    }
}

#endif