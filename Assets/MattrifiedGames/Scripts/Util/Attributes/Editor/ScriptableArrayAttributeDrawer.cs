using MattrifiedGames.SVData.Arrays;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ScriptableArrayAttribute))]
public class ScriptableArrayAttributeDrawer : PropertyDrawer
{
    ScriptableArrayBaseUntyped v;
    GUIContent [] array;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (v == null)
        {
            var list = MattrifiedGames.Utility.AssetFinder.LoadAllAssetOfType<ScriptableArrayBaseUntyped>();
            v = list.Find(x => x.name == ((ScriptableArrayAttribute)attribute).n);
            if (v != null)
            {
                var a = v.CreateStringArray();
                array = new GUIContent[a.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    array[i] = new GUIContent(a[i]);
                }
            }
        }

        if (v == null || property.serializedObject.isEditingMultipleObjects)
        {
            base.OnGUI(position, property, label);
        }
        else
        {
            property.intValue = EditorGUI.Popup(position, label, property.intValue, array);
        }
    }
}
