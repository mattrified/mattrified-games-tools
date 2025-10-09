using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.HairKit.Edit
{
    [CustomEditor(typeof(HairKitShape))]
    public class HairKitShapeEditor : Editor
    {
        float radius = 1f;
        int count = 10;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();

            radius = EditorGUILayout.FloatField("Radius", radius);
            count = Mathf.Max(3, EditorGUILayout.IntField("Count", count));

            HairKitShape shape = (HairKitShape)target;

            if (GUILayout.Button("Renamed Children"))
            {
                int cc = shape.transform.childCount;
                for (int i = 0; i < cc; i++)
                {
                    shape.transform.GetChild(i).name = target.name + " Point " + i;
                }
            }

            if (GUILayout.Button("Create Radius"))
            {
                int cc = shape.transform.childCount;
                for (int i = 0; i < cc; i++)
                {
                    DestroyImmediate(shape.transform.GetChild(0).gameObject);
                }

                for (int i = 0; i < count; i++)
                {
                    GameObject go = new GameObject(target.name + " " + i);
                    go.transform.SetParent(shape.transform, false);
                    go.transform.localPosition = Vector3.down * radius;
                    go.transform.RotateAround(shape.transform.position, shape.transform.forward, i * 360f / (count - 1));
                }
            }
        }
    }
}