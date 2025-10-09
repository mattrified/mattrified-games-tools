using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.HairKit.Edit
{
    [CustomEditor(typeof(HairKitLinePoint)), CanEditMultipleObjects()]
    public class HairKitLinePointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            HairKitLinePoint point = (HairKitLinePoint)target;

            if (GUILayout.Button("Add Child"))
            {
                var newPoint = Instantiate(point);
                newPoint.transform.SetParent(point.transform, false);
                Selection.activeGameObject = newPoint.gameObject;
            }

            if (GUILayout.Button("Add Sibling"))
            {
                var newPoint = Instantiate(point);
                int siblingIndex = point.transform.GetSiblingIndex();
                newPoint.transform.SetParent(point.transform.parent, false);
                newPoint.transform.SetSiblingIndex(siblingIndex + 1);
                Selection.activeGameObject = newPoint.gameObject;
            }
        }
    }
}