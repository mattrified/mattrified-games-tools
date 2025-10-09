using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MattrifiedGames.Utility;

[CustomEditor(typeof(SkinRendererWeightTransfer))]
public class SkinRendererWeightTransferEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SkinRendererWeightTransfer smrwt = (SkinRendererWeightTransfer)target;

        SkinnedMeshRenderer skinnedMeshRenderer = smrwt.GetComponent<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderer.bones.Length; i++)
        {
            EditorGUILayout.ObjectField(skinnedMeshRenderer.bones[i], typeof(Transform), true);
        }
    }
}
