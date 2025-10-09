using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MattrifiedGames.MeshCreation
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class CreateWebbingBetweenBones : MonoBehaviour
    {
        public FinSegment[] finSegments;
        List<BoneTPair> internalPairs;

        [System.Serializable()]
        public class FinSegment
        {
            public BoneTPair startingPair;
            public int length = 3;

            BoneTPair[] bonePair;
            public BoneTPair[] BonePairs
            {
                get
                {
                    if (bonePair == null)
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

        public Vector3[] vList;
        int[] tris;

        public bool upDown;

        void Start()
        {
            MakeMesh();
        }

        [ContextMenu("Make Mesh")]
        void MakeMesh()
        {
            internalPairs = new List<BoneTPair>();

            List<Vector2> uvs = new List<Vector2>();
            for (int i = 0; i < finSegments.Length; i++)
            {
                BoneTPair[] array = finSegments[i].BonePairs;
                for (int j = 0; j < finSegments[i].BonePairs.Length; j++)
                {
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

            Mesh m = GetComponent<MeshFilter>().sharedMesh;
            if (m == null)
                m = new Mesh();
            m.name = "Fin Mesh";

            for (int i = 0; i < pTotal; i++)
            {
                int vS = i * 4;
                vList[vS] = internalPairs[i].boneA.position;
                vList[vS + 1] = internalPairs[i].boneB.position;

                vList[vS + 2] = internalPairs[i].boneA.GetChild(0).position;
                vList[vS + 3] = internalPairs[i].boneB.GetChild(0).position;

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

            GetComponent<MeshFilter>().sharedMesh = m;
        }

        void LateUpdate()
        {
            Mesh m = GetComponent<MeshFilter>().sharedMesh;

            for (int i = 0; i < internalPairs.Count; i++)
            {
                int vS = i * 4;
                vList[vS] = internalPairs[i].boneA.position;
                vList[vS + 1] = internalPairs[i].boneB.position;

                vList[vS + 2] = internalPairs[i].boneA.GetChild(0).position;
                vList[vS + 3] = internalPairs[i].boneB.GetChild(0).position;
            }

            m.vertices = vList;
        }
    }
}