using UnityEngine;
using System.Collections;

public class IgnoreAttribute : PropertyAttribute
{
    
}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(IgnoreAttribute))]
public class IgnoreAttributeDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        Color c = GUI.color;
        GUI.color = new Color(0.5f, 0.9f, 1f, 1f);
        var obj = fieldInfo.GetValue(property.serializedObject.targetObject);
        string s;
        if (obj == null)
            s = "Null";
        else
            s = obj.ToString();
        UnityEditor.EditorGUI.SelectableLabel(position, label.text + ":  " + s);
        GUI.color = c;
    }
}

#endif