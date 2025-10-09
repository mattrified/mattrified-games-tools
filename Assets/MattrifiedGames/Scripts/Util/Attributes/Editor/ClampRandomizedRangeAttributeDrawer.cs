using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ClampRandomizedRangeAttribute))]
public class ClampRandomizedRangeAttributeDrawer : PropertyDrawer
{
    const string RANDOMIZE = "randomize";

    const string MIN = "min";

    const string MAX = "max";

    const string VALUE = "Value";

    const string MISTAKE = "Can only be used on RandomizedRange.";


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ClampRandomizedRangeAttribute att = (ClampRandomizedRangeAttribute)attribute;

        float oldWidth = EditorGUIUtility.labelWidth;

        var minProperty = property.FindPropertyRelative(MIN);
        var maxProperty = property.FindPropertyRelative(MAX);
        var randomProperty = property.FindPropertyRelative(RANDOMIZE);

        if (property.hasMultipleDifferentValues || minProperty == null || maxProperty == null || randomProperty == null)
        {
            EditorGUI.LabelField(position, property.displayName, MISTAKE);
        }
        else
        {
            EditorGUIUtility.labelWidth = 48f;

            Rect smallRect = position;
            smallRect.width *= 1f / 4f;

            EditorGUI.LabelField(smallRect, label);

            smallRect.x += smallRect.width;
            randomProperty.boolValue =
                EditorGUI.ToggleLeft(smallRect, randomProperty.displayName, randomProperty.boolValue);

            if (randomProperty.boolValue)
            {
                smallRect.x += smallRect.width;
                minProperty.floatValue = !att.useClamp ?
                    EditorGUI.FloatField(smallRect, minProperty.displayName, minProperty.floatValue) :
                    EditorGUI.Slider(smallRect, minProperty.displayName, minProperty.floatValue, att.clampMin, att.clampMax);

                smallRect.x += smallRect.width;
                maxProperty.floatValue = !att.useClamp ?
                    EditorGUI.FloatField(smallRect, maxProperty.displayName, maxProperty.floatValue) :
                    EditorGUI.Slider(smallRect, maxProperty.displayName, maxProperty.floatValue, att.clampMin, att.clampMax);

                float min = Mathf.Min(minProperty.floatValue, maxProperty.floatValue);
                float max = Mathf.Max(minProperty.floatValue, maxProperty.floatValue);
                minProperty.floatValue = min;
                maxProperty.floatValue = max;
            }
            else
            {
                smallRect.x += smallRect.width;
                smallRect.width *= 2f;
                minProperty.floatValue = !att.useClamp ?
                    EditorGUI.FloatField(smallRect, VALUE, minProperty.floatValue) :
                    EditorGUI.Slider(smallRect, VALUE, minProperty.floatValue, att.clampMin, att.clampMax);
                maxProperty.floatValue = minProperty.floatValue;


            }
        }

        EditorGUIUtility.labelWidth = oldWidth;
    }
}
