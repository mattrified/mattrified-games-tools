using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MattrifiedGames.HairKit.Edit
{
    [CustomEditor(typeof(HairKitLine)), CanEditMultipleObjects()]
    public class HairKitLineEditor : Editor
    {
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            HairKitLine line = (HairKitLine)target;


            if (GUILayout.Button("Add Child"))
            {
                GameObject go = new GameObject(this.name + " Point " + line.children.Count, typeof(HairKitLinePoint));
                go.transform.SetParent(line.transform, false);
            }

            if (line.addTransform != null)
            {
                if (GUILayout.Button("Add Child at Transform"))
                {
                    GameObject go = new GameObject(this.name + " Point " + line.children.Count, typeof(HairKitLinePoint));
                    go.transform.SetParent(line.transform, false);

                    go.transform.position = line.addTransform.position;
                    go.transform.rotation = line.addTransform.rotation;
                }
            }

            if (GUILayout.Button("Rename Children"))
            {
                for (int i = 0; i < line.children.Count; i++)
                {
                    line.children[i].name = line.name + " Point " + i;
                }
            }

            if (GUILayout.Button("Smooth Children:  WARNING, only to unstacked children"))
            {
                AnimationCurve[] curves = new AnimationCurve[7];
                for (int i = 0; i < 7; i++)
                    curves[i] = new AnimationCurve();

                int len = line.children.Count;
                for (int i = 0; i < len; i++)
                {
                    var a = line.children[i];

                    curves[0].AddKey(i, a.transform.position.x);
                    curves[1].AddKey(i, a.transform.position.y);
                    curves[2].AddKey(i, a.transform.position.z);

                    curves[3].AddKey(i, a.transform.rotation.x);
                    curves[4].AddKey(i, a.transform.rotation.y);
                    curves[5].AddKey(i, a.transform.rotation.z);
                    curves[6].AddKey(i, a.transform.rotation.w);

                    GameObject go = new GameObject(a.name + "_" + line.children.Count, typeof(HairKitLinePoint));
                    go.transform.SetParent(a.transform.parent);

                    line.children.Add(go.GetComponent<HairKitLinePoint>());
                }

                for (int i = 0; i < line.children.Count; i++)
                {
                    Vector3 pos = new Vector3();
                    pos.x = curves[0].Evaluate(i / 2f);
                    pos.y = curves[1].Evaluate(i / 2f);
                    pos.z = curves[2].Evaluate(i / 2f);

                    Quaternion quat = Quaternion.identity;
                    quat.x = curves[3].Evaluate(i / 2f);
                    quat.y = curves[4].Evaluate(i / 2f);
                    quat.z = curves[5].Evaluate(i / 2f);
                    quat.w = curves[6].Evaluate(i / 2f);

                    line.children[i].transform.position = pos;
                    line.children[i].transform.rotation = quat;
                }
            }

            if (GUILayout.Button("Stack Children"))
            {
                for (int i = 1; i < line.children.Count; i++)
                {
                    line.children[i].transform.SetParent(line.children[i - 1].transform, true);
                }
            }

            if (GUILayout.Button("Unstack Children"))
            {
                for (int i = 1; i < line.children.Count; i++)
                {
                    line.children[i].transform.SetParent(line.transform, true);
                }
            }

            if (GUILayout.Button("Reset Rotation"))
            {
                List<Vector3> pos = new List<Vector3>();
                List<Quaternion> rot = new List<Quaternion>();

                for (int i = 0; i < line.children.Count; i++)
                {
                    pos.Add(line.children[i].transform.position);
                    rot.Add(line.children[i].transform.rotation);
                }

                line.transform.rotation = Quaternion.identity;

                for (int i = 0; i < line.children.Count; i++)
                {
                    line.children[i].transform.SetPositionAndRotation(pos[i], rot[i]);
                }
            }

            if (GUILayout.Button("Reset Scale"))
            {
                List<Vector3> pos = new List<Vector3>();
                List<Quaternion> rot = new List<Quaternion>();

                for (int i = 0; i < line.children.Count; i++)
                {
                    pos.Add(line.children[i].transform.position);
                    rot.Add(line.children[i].transform.rotation);
                }

                line.transform.localScale = Vector3.one;

                for (int i = 0; i < line.children.Count; i++)
                {
                    line.children[i].transform.SetPositionAndRotation(pos[i], rot[i]);
                }
            }
        }
    }
}