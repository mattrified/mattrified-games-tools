using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(OptionalAttribute))]
public class OptionalAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        OptionalAttribute att = attribute as OptionalAttribute;
        var boolProperty = property.serializedObject.FindProperty(att.useVariable);
        if (boolProperty == null || boolProperty.propertyType != SerializedPropertyType.Boolean)
        {
            EditorGUI.LabelField(position, "Optional Bool Broken");
        }
        else
        {
            EditorGUIUtility.labelWidth = position.width * 0.125f;

            Rect half = position;
            half.width = position.width * 0.25f;

            boolProperty.boolValue = EditorGUI.ToggleLeft(half, boolProperty.displayName, boolProperty.boolValue);

            if (boolProperty.boolValue)
            {
                half.x += position.width * 0.25f;
                half.width = position.width * 0.75f;

                label.text = string.Empty;
                EditorGUI.PropertyField(half, property, label);
            }
            else
            {
                

                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    property.objectReferenceValue = null;
                }
            }

            EditorGUIUtility.labelWidth = -1;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label);
    }
}
