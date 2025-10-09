using UnityEngine;
using System.Collections.Generic;

namespace MattrifiedGames.MeshCreation
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class CreateSkinnedStrip : MonoBehaviour
    {
        [Range(2, 100)]
        public int segments = 10;
        public float baseWidth;
        public AnimationCurve widthCurve = AnimationCurve.Linear(0, 1, 1, 1);

        public float length = 1;

        public Vector2 uvMin = Vector2.zero, uvMax = Vector2.one;

        Transform[] bones = new Transform[0];

        [ContextMenu("Build")]
        void CreateMesh()
        {
            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            if (smr == null || smr.rootBone == null)
                return;

            Mesh m = smr.sharedMesh;
            if (m == null)
            {
                m = new Mesh();
                m.name = this.name + " mesh";
                smr.sharedMesh = m;
            }
            m.Clear();

            Vector3[] verts;
            List<int> tris = new List<int>();
            BoneWeight[] boneweights;
            Matrix4x4[] bindPoses;
            Vector2[] uvs;

            verts = new Vector3[segments * 2];
            boneweights = new BoneWeight[verts.Length];
            bindPoses = new Matrix4x4[segments];
            uvs = new Vector2[verts.Length];

            Transform[] newBones = new Transform[segments];
            bones = newBones;

            Transform parent = smr.rootBone;

            for (int i = 0; i < segments; i++)
            {
                if (bones[i] == null)
                    bones[i] = new GameObject(this.name + " bone " + i).transform;
                bones[i].SetParent(parent, false);
                float segmentY = (float)length / segments;
                float percent = Mathf.InverseLerp(0, segments - 1, i);
                bones[i].transform.localPosition = new Vector3(0, i == 0 ? 0 : segmentY, 0);
                parent = bones[i];

                Vector3 point = bones[i].position + new Vector3(baseWidth * widthCurve.Evaluate(percent), 0, 0);
                verts[2 * i] = smr.rootBone.InverseTransformPoint(point);

                point = bones[i].position + new Vector3(-baseWidth * widthCurve.Evaluate(percent), 0, 0);
                verts[2 * i + 1] = smr.rootBone.InverseTransformPoint(point);

                uvs[2 * i] = new Vector2(uvMin.x, Mathf.Lerp(uvMin.y, uvMax.y, percent));
                uvs[2 * i + 1] = new Vector2(uvMax.x, Mathf.Lerp(uvMin.y, uvMax.y, percent));

                boneweights[2 * i] = new BoneWeight() { boneIndex0 = i, weight0 = 1f };
                boneweights[2 * i + 1] = new BoneWeight() { boneIndex0 = i, weight0 = 1f };

                if (i < segments - 1)
                {
                    tris.Add(2 * i);
                    tris.Add(2 * i + 1);
                    tris.Add(2 * i + 2);

                    tris.Add(2 * i + 2 + 1);
                    tris.Add(2 * i + 2);
                    tris.Add(2 * i + 1);

                    tris.Add(2 * i + 2);
                    tris.Add(2 * i + 1);
                    tris.Add(2 * i);

                    tris.Add(2 * i + 1);
                    tris.Add(2 * i + 2);
                    tris.Add(2 * i + 2 + 1);
                }

                bindPoses[i] = bones[i].worldToLocalMatrix * smr.rootBone.localToWorldMatrix;
            }

            smr.bones = bones;
            m.vertices = verts;
            m.triangles = tris.ToArray();
            m.uv = uvs;
            m.bindposes = bindPoses;
            m.boneWeights = boneweights;

        }
    }
}