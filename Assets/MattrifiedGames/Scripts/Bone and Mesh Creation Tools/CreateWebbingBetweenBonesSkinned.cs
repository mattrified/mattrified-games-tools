using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MattrifiedGames.MeshCreation
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class CreateWebbingBetweenBonesSkinned : MonoBehaviour
    {
        public FinSegment[] finSegments;
        List<BoneTPair> internalPairs;

        [System.Serializable()]
        public class FinSegment
        {
            public BoneTPair startingPair;
            public int length = 3;

            public BoneTPair[] bonePair;
            public BoneTPair[] BonePairs
            {
                get
                {
                    if (bonePair == null || bonePair.Length != length)
                    {
                        bonePair = new BoneTPair[length];
                        bonePair[0] = startingPair;
                        for (int i = 1; i < length; i++)
                        {
                            bonePair[i] = new BoneTPair(bonePair[i - 1].boneA.GetChild(0), bonePair[i - 1].boneB.GetChild(0));
                        }
                    }

                    return bonePair;
                }
            }
        }

        [System.Serializable()]
        public class BoneTPair
        {
            public BoneTPair(Transform bA, Transform bB)
            {
                boneA = bA;
                boneB = bB;
            }

            public Transform boneA;
            public Transform boneB;
        }

        public Vector2 uvMin = new Vector2(0, 0), uvMax = new Vector2(1, 1);

        Vector3[] vList;
        BoneWeight[] boneWeightList;
        int[] tris;

        public bool upDown;

        void Start()
        {
            //MakeMesh();
        }

        [ContextMenu("Make Mesh")]
        void MakeMesh()
        {
            internalPairs = new List<BoneTPair>();

            List<Transform> bones = new List<Transform>();
            Matrix4x4[] bindPoses;
            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < finSegments.Length; i++)
            {
                BoneTPair[] array = finSegments[i].BonePairs;
                for (int j = 0; j < array.Length; j++)
                {
                    if (!bones.Contains(array[j].boneA))
                        bones.Add(array[j].boneA);
                    if (!bones.Contains(array[j].boneA.GetChild(0)))
                        bones.Add(array[j].boneA.GetChild(0));
                    if (!bones.Contains(array[j].boneB))
                        bones.Add(array[j].boneB);
                    if (!bones.Contains(array[j].boneB.GetChild(0)))
                        bones.Add(array[j].boneB.GetChild(0));

                    // Add another pair
                    internalPairs.Add(array[j]);

                    if (upDown)
                    {
                        uvs.Add(new Vector2(uvMin.x, Mathf.Lerp(uvMin.y, uvMax.y, (float)j / array.Length)));
                        uvs.Add(new Vector2(uvMax.x, Mathf.Lerp(uvMin.y, uvMax.y, (float)j / array.Length)));
                        uvs.Add(new Vector2(uvMin.x, Mathf.Lerp(uvMin.y, uvMax.y, (float)(1 + j) / array.Length)));
                        uvs.Add(new Vector2(uvMax.x, Mathf.Lerp(uvMin.y, uvMax.y, (float)(1 + j) / array.Length)));
                    }
                    else
                    {
                        uvs.Add(new Vector2(Mathf.Lerp(uvMin.x, uvMax.x, (float)j / array.Length), uvMin.y));
                        uvs.Add(new Vector2(Mathf.Lerp(uvMin.x, uvMax.x, (float)j / array.Length), uvMax.y));
                        uvs.Add(new Vector2(Mathf.Lerp(uvMin.x, uvMax.x, (float)(1 + j) / array.Length), uvMin.y));
                        uvs.Add(new Vector2(Mathf.Lerp(uvMin.x, uvMax.x, (float)(1 + j) / array.Length), uvMax.y));
                    }
                }
            }



            int pTotal = internalPairs.Count;
            // Sets the number of vList and tris; more on start.
            vList = new Vector3[pTotal * 4];
            tris = new int[pTotal * 12];
            boneWeightList = new BoneWeight[pTotal * 4];

            SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
            Mesh m = smr.sharedMesh;

            bindPoses = new Matrix4x4[bones.Count];
            for (int i = 0; i < bindPoses.Length; i++)
                bindPoses[i] = bones[i].worldToLocalMatrix * smr.rootBone.localToWorldMatrix;

            if (m == null)
                m = new Mesh();
            m.name = "Fin Mesh";

            for (int i = 0; i < pTotal; i++)
            {
                int vS = i * 4;
                vList[vS] = smr.rootBone.InverseTransformPoint(internalPairs[i].boneA.position);
                vList[vS + 1] = smr.rootBone.InverseTransformPoint(internalPairs[i].boneB.position);

                vList[vS + 2] = smr.rootBone.InverseTransformPoint(internalPairs[i].boneA.GetChild(0).position);
                vList[vS + 3] = smr.rootBone.InverseTransformPoint(internalPairs[i].boneB.GetChild(0).position);

                boneWeightList[vS] = new BoneWeight() { boneIndex0 = bones.IndexOf(internalPairs[i].boneA), weight0 = 1f };
                boneWeightList[vS + 1] = new BoneWeight() { boneIndex0 = bones.IndexOf(internalPairs[i].boneB), weight0 = 1f };
                boneWeightList[vS + 2] = new BoneWeight() { boneIndex0 = bones.IndexOf(internalPairs[i].boneA.GetChild(0)), weight0 = 1f };
                boneWeightList[vS + 3] = new BoneWeight() { boneIndex0 = bones.IndexOf(internalPairs[i].boneB.GetChild(0)), weight0 = 1f };

                int tS = i * 12;
                tris[tS] = vS;
                tris[tS + 1] = vS + 1;
                tris[tS + 2] = vS + 2;
                tris[tS + 3] = vS + 3;
                tris[tS + 4] = vS + 2;
                tris[tS + 5] = vS + 1;
                tris[tS + 6] = vS + 2;
                tris[tS + 7] = vS + 1;
                tris[tS + 8] = vS;
                tris[tS + 9] = vS + 1;
                tris[tS + 10] = vS + 2;
                tris[tS + 11] = vS + 3;
            }

            m.vertices = vList;
            m.triangles = tris;
            m.SetUVs(0, uvs);

            m.boneWeights = boneWeightList;
            m.bindposes = bindPoses;

            m.RecalculateBounds();
            //smr.rootBone = bones[0];
            smr.bones = bones.ToArray();
            smr.sharedMesh = m;
        }
    }
}