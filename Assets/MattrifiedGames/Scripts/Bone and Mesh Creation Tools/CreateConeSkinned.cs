using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MattrifiedGames.Utility;

namespace MattrifiedGames.MeshCreation
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class CreateConeSkinned : MonoBehaviour
    {
        [Range(3, 64)]
        public int sides = 8;

        public float baseRadius = 1f;
        // The curve of the bone (used for unique shapes);
        public AnimationCurve radiusCurve;

        public float height = 2f;

        // The number of bones in the cone.
        [Range(2, 20)]
        public int boneNumber = 2;

        public Transform[] bones;

        [ContextMenu("Build")]
        public void BuildCone()
        {
            List<Vector3> verts = new List<Vector3>();
            // TODO:  Assign UVS.
            List<Vector2> uvs = new List<Vector2>();
            List<int> tris = new List<int>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            List<Matrix4x4> bindPoses = new List<Matrix4x4>();

            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            if (smr == null)
            {
                Debug.LogError("No SkinnedMeshRenderer found.");
                return;
            }

            Mesh m = smr.sharedMesh;
            if (m == null)
            {
                m = new Mesh();
                m.name = this.name + " mesh";
                smr.sharedMesh = m;
            }
            m.Clear(false);

            // Builds the bones themselves.
            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i] != null)
                {
                    if (Application.isEditor)
                        DestroyImmediate(bones[i].gameObject);
                    else
                        Destroy(bones[i].gameObject);
                }
            }
            bones = new Transform[boneNumber];
            Transform parent = smr.rootBone;
            for (int i = 0; i < boneNumber; i++)
            {
                float percent = Mathf.InverseLerp(0, boneNumber - 1, i);
                GameObject go = new GameObject(this.name + " bone " + i);
                if (i == 0)
                    go.AddComponent<BoneVisualizer>();
                go.transform.SetParent(parent, true);
                go.transform.position = Vector3.Lerp(smr.rootBone.position, smr.rootBone.position + Vector3.up * height, percent);
                parent = go.transform;
                bones[i] = parent;
                bindPoses.Add(bones[i].worldToLocalMatrix * smr.rootBone.localToWorldMatrix);

                for (int j = 0; j < sides; j++)
                {
                    Vector3 pos = Vector3.zero;
                    Vector3 vert = pos + new Vector3(baseRadius * radiusCurve.Evaluate(percent) * Mathf.Cos(Mathf.Deg2Rad * j * 360f / sides),
                        percent * height, baseRadius * radiusCurve.Evaluate(percent) * Mathf.Sin(Mathf.Deg2Rad * j * 360f / sides));
                    verts.Add(vert);
                    boneWeights.Add(new BoneWeight() { boneIndex0 = i, weight0 = 1f });
                }
            }

            for (int i = 0; i < boneNumber - 1; i++)
            {
                for (int j = 0; j < sides; j++)
                {
                    tris.Add(i * sides + j);
                    tris.Add(i * sides + j + 1);
                    tris.Add(i * sides + j + sides);

                    if (j < sides - 1)
                    {
                        tris.Add(i * sides + j + 1 + sides);
                        tris.Add(i * sides + j + sides);
                        tris.Add(i * sides + j + 1);
                    }
                    else
                    {
                        tris.Add(i * sides + j + 1 + sides - sides);
                        tris.Add(i * sides + j + sides - sides);
                        tris.Add(i * sides + j + 1 - sides);
                    }
                }
            }

            tris.Reverse();

            m.vertices = verts.ToArray();
            m.triangles = tris.ToArray();
            m.uv = uvs.ToArray();
            m.boneWeights = boneWeights.ToArray();
            m.bindposes = bindPoses.ToArray();
            smr.bones = bones;
        }
    }
}