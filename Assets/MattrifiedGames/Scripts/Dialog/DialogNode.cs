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
[NodeMenu("Dialog", typeof(VignetteGraph))]
public class DialogNode : VignetteNode
{
    public string speakerName;
    public bool displayOnTop;
    public string dialog;

    public override OutputRule OutputRule
    {
        get
        {
            return OutputRule.Passthrough();
        }
    }
}

/// <summary>
/// Draws a vignettitor representation of a jump node, which includes a 
/// rendering of the node it will jump to.
/// </summary>
[NodeView(typeof(DialogNode))]
public class DialogNodeView : VignetteNodeView
{
    const string D_LABEL = "Display on Top";

    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        var node = Node as DialogNode;
        node.speakerName = GUILayout.TextField(node.speakerName);
        node.displayOnTop = GUILayout.Toggle(node.displayOnTop, D_LABEL);
        node.dialog = GUILayout.TextArea(node.dialog);
    }
}

#endif