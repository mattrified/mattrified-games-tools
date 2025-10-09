using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.HairKit
{
    [ExecuteInEditMode()]
    public class HairKitMain : MonoBehaviour
    {
        public HairKitLine[] hairLines = new HairKitLine[0];

        List<Vector3> points = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Mesh m;

        MeshFilter mf;
        MeshRenderer mr;

        private void Update()
        {
            if (m == null)
            {
                m = new Mesh();
                m.name = name + " mesh";
            }
            m.Clear();

            mf = GetComponent<MeshFilter>();
            if (mf == null)
                mf = gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = m;

            mr = GetComponent<MeshRenderer>();
            if (mr == null)
                mr = gameObject.AddComponent<MeshRenderer>();


            points.Clear();
            tris.Clear();
            uvs.Clear();

            int offset = 0;
            for (int h = 0; h < hairLines.Length; h++)
            {
                var hairLine = hairLines[h];

                points.AddRange(hairLine.vList);

                int shapeChildCount = hairLine.shape.transform.childCount;
                int lines = hairLine.segments;

                for (int i = 0; i < hairLine.vList.Count; i += 1)
                {
                    points[i + offset] = transform.InverseTransformPoint(points[i + offset]);

                    int a = i;
                    int b = i + shapeChildCount;
                    int c = i + shapeChildCount + 1;
                    int d = i + 1;

                    Vector2 uv = Vector2.zero;
                    uv.x = Mathf.Lerp(hairLine.uvRect.xMin, hairLine.uvRect.xMax, hairLine.shape.uvPercentages[i % shapeChildCount]);

                    uv.y = Mathf.Lerp(hairLine.uvRect.yMin, hairLine.uvRect.yMax, hairLine.uvPercentages[Mathf.FloorToInt((float)i / shapeChildCount) % lines]);

                    uvs.Add(uv);

                    if (a >= hairLine.vList.Count || b >= hairLine.vList.Count || c >= hairLine.vList.Count || d % shapeChildCount == 0)
                        continue;

                    if (!hairLine.flipNormal)
                    {
                        tris.Add(a + offset);
                        tris.Add(b + offset);
                        tris.Add(c + offset);

                        tris.Add(a + offset);
                        tris.Add(c + offset);
                        tris.Add(d + offset);
                    }
                    else
                    {
                        tris.Add(d + offset);
                        tris.Add(c + offset);
                        tris.Add(a + offset);

                        tris.Add(c + offset);
                        tris.Add(b + offset);
                        tris.Add(a + offset);
                    }
                }

                offset += hairLine.vList.Count;
            }

            m.SetVertices(points);
            m.SetTriangles(tris, 0, true);
            m.SetUVs(0, uvs);

            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();
        }

        [ContextMenu("Clone and Save")]
        public void Clone()
        {
            GameObject copy = Instantiate(this.gameObject);

            // Disables the kit and the meshrenderer
            HairKitMain kits = copy.GetComponent<HairKitMain>();
            kits.enabled = false;

            HairKitLine[] line = copy.GetComponentsInChildren<HairKitLine>();

            MeshFilter mf = copy.GetComponent<MeshFilter>();
            Mesh m = Instantiate(mf.sharedMesh);
            mf.sharedMesh = m;

            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();

            DestroyImmediate(kits);

            foreach (var l in line)
                DestroyImmediate(l.gameObject);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(m, "Assets/" + this.name + "_mesh.asset");
#endif
        }

        [ContextMenu("Skin")]
        public void Skin()
        {
            GameObject copy = Instantiate(this.gameObject);

            // Disables the kit and the meshrenderer
            HairKitMain kits = copy.GetComponent<HairKitMain>();
            kits.enabled = false;

            HairKitLine[] line = copy.GetComponentsInChildren<HairKitLine>();

            MeshRenderer mr = copy.GetComponent<MeshRenderer>();
            mr.enabled = false;

            MeshFilter mf = copy.GetComponent<MeshFilter>();
            Mesh m = Instantiate(mf.sharedMesh);
            DestroyImmediate(mf);

            SkinnedMeshRenderer smr = copy.AddComponent<SkinnedMeshRenderer>();
            smr.rootBone = smr.transform;

            smr.sharedMesh = m;

            int offset = 0;
            int segmentOffset = 0;

            List<BoneWeight> boneWeight = new List<BoneWeight>();
            List<Matrix4x4> bindposes = new List<Matrix4x4>();
            List<Transform> bones = new List<Transform>();

            for (int h = 0; h < kits.hairLines.Length; h++)
            {
                var hairline = kits.hairLines[h];
                int shapeChildCount = hairline.shape.transform.childCount;

                for (int i = 0; i < hairline.vList.Count; i++)
                {
                    BoneWeight bw = new BoneWeight();
                    int row = (Mathf.FloorToInt(i / (float)(shapeChildCount)) % hairline.segments) + segmentOffset;

                    bw.boneIndex0 = row;
                    bw.weight0 = 1f;

                    boneWeight.Add(bw);
                }

                var c1 = hairline.transform.GetChild(0);
                bindposes.Add(c1.worldToLocalMatrix * smr.rootBone.localToWorldMatrix);
                bones.Add(c1);
                while (c1.childCount == 1)
                {
                    c1 = c1.GetChild(0);
                    bones.Add(c1);
                    bindposes.Add(c1.worldToLocalMatrix * smr.rootBone.localToWorldMatrix);
                }

                segmentOffset += hairline.segments;
                offset += hairline.vList.Count;
            }

            m.bindposes = bindposes.ToArray();
            m.boneWeights = boneWeight.ToArray();
            smr.bones = bones.ToArray();

            m.RecalculateBounds();
            m.RecalculateNormals();
            m.RecalculateTangents();

            DestroyImmediate(kits);

            foreach (var l in line)
                DestroyImmediate(l);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(m, "Assets/" + this.name + "_skinned_mesh.asset");
#endif
        }
    }
}