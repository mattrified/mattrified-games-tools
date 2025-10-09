#if USING_SGF

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SG.Vignettitor;
using SG.Vignettitor.VignetteData;
using SG.Vignettitor.NodeViews;
using SG.Vignettitor.Graph.NodeViews;

/// <summary>
/// Jump nodes transition immediately to their only child node and are 
/// meant to serve an organizational purpose but not a functional purpose.
/// </summary>
[NodeMenu("Dialog Choice", typeof(VignetteGraph))]
public class DialogChoiceNode : VignetteNode
{
    public List<string> choices = new List<string> { "" };

    public override OutputRule OutputRule
    {
        get
        {
            return OutputRule.Static(choices.Count);
        }
    }
}

/// <summary>
/// Draws a vignettitor representation of a jump node, which includes a 
/// rendering of the node it will jump to.
/// </summary>
[NodeView(typeof(DialogChoiceNode))]
public class DialogChoiceNodeView : VignetteNodeView
{
    const string D_LABEL = "Display on Top";

    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        var node = Node as DialogChoiceNode;

        for (int i = 0, count = node.choices.Count; i < count; i++)
        {
            node.choices[i] = GUILayout.TextField(node.choices[i]);
        }

        GUILayout.Space(8f);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-"))
            node.choices.RemoveAt(node.choices.Count - 1);
        if (GUILayout.Button("+"))
            node.choices.Add("NEW");
        GUILayout.EndHorizontal();
    }

    public override string GetConnectionLabel(int c)
    {
        var node = Node as DialogChoiceNode;

        if (c >= node.choices.Count)
            return base.GetConnectionLabel(c);
        else
            return node.choices[c];
    }
}

#endif