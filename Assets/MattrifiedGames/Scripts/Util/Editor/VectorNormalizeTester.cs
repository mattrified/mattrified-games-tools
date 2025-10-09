using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VectorNormalizeTester : EditorWindow
{
    [MenuItem("Tools/Testing/Normalize Vector")]
    public static void GW()
    {
        GetWindow<VectorNormalizeTester>();
    }

    public float angle;
    Vector2 vAngle;

    public Vector2 v2;
    public Vector3 v3;

    private void OnGUI()
    {
        angle = EditorGUILayout.FloatField("Angle in Degrees", angle);
        vAngle.x = Mathf.Cos(Mathf.Deg2Rad * angle);
        vAngle.y = Mathf.Sin(Mathf.Deg2Rad * angle);
        EditorGUILayout.FloatField(vAngle.x);
        EditorGUILayout.FloatField(vAngle.y);

        EditorGUILayout.Space();

        v2 = EditorGUILayout.Vector2Field("Vector2", v2);
        Vector2 v2n = v2.normalized;
        EditorGUILayout.FloatField(v2n.x);
        EditorGUILayout.FloatField(v2n.y);

        EditorGUILayout.Space();

        v3 = EditorGUILayout.Vector3Field("Vector3", v3);
        Vector3 v3n = v3.normalized;
        EditorGUILayout.FloatField(v3n.x);
        EditorGUILayout.FloatField(v3n.y);
        EditorGUILayout.FloatField(v3n.z);
    }

}
