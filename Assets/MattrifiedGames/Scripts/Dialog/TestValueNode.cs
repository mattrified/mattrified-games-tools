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
[NodeMenu("Test Value", typeof(VignetteGraph))]
public class TestValueNode : VignetteNode
{
    public IntScriptableValue intScriptableValue = null;

    public List<TestConditions> testConditions = new List<TestConditions>() { new TestConditions() };

    [System.Serializable()]
    public class TestConditions
    {
        public int minValue = 0;
        public int maxValue = 0;
        public bool include = true;
    }

    public override OutputRule OutputRule
    {
        get
        {
            return OutputRule.Static(testConditions.Count + 1);
        }
    }

    internal int TestValue()
    {
        if (intScriptableValue == null)
        {
            Debug.LogWarning("No int script is plugged in.  Returning first result for " + this.name);
            return 0;
        }

        int value = intScriptableValue.Value;

        for (int i = 0, len = testConditions.Count; i < len; i++)
        {
            var tc = testConditions[i];
            bool result = (value >= tc.minValue && value <= tc.maxValue) && tc.include;
            if (result)
            {
                return i;
            }
        }

        // Returns the last result.
        return testConditions.Count;
    }
}

/// <summary>
/// Draws a vignettitor representation of a jump node, which includes a 
/// rendering of the node it will jump to.
/// </summary>
[NodeView(typeof(TestValueNode))]
public class TestValueNodeView : VignetteNodeView
{
    public override void Draw(Rect rect)
    {
        base.Draw(rect);

        var node = Node as TestValueNode;

        if (node.intScriptableValue != null)
            GUILayout.Label(node.intScriptableValue.name);

        for (int i = 0; i < node.testConditions.Count; i++)
        {
            GUILayout.BeginHorizontal();

            int.TryParse(GUILayout.TextField(node.testConditions[i].minValue.ToString()),
                    out node.testConditions[i].minValue);

            int.TryParse(GUILayout.TextField(node.testConditions[i].maxValue.ToString()),
                    out node.testConditions[i].maxValue);

            if (node.testConditions[i].include)
            {
                if (GUILayout.Button("=="))
                {
                    node.testConditions[i].include = !node.testConditions[i].include;
                }
            }
            else
            {
                if (GUILayout.Button("!="))
                {
                    node.testConditions[i].include = !node.testConditions[i].include;
                }
            }
            

            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-"))
            node.testConditions.RemoveAt(node.testConditions.Count - 1);
        if (GUILayout.Button("+"))
            node.testConditions.Add(new TestValueNode.TestConditions());
        GUILayout.EndHorizontal();
    }
}

#endif