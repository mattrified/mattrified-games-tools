using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickTriangleEditor : MonoBehaviour
{
    public Mesh currentMesh;
    public Mesh lastCreatedMesh;

    public string nameMeshName;

    public Collider[] colliders;

    public bool excludeVertsInColliders = true;

    public List<int> subMeshesToRemove;

    /// <summary>
    /// If true, new triangles will instead be created as new submeshes.
    /// </summary>
    public bool newTrianglesAsSubmesh = false;
    
    public virtual void OnValidate()
    {
        colliders = GetComponentsInChildren<Collider>();

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf != null)
        {
            currentMesh = mf.sharedMesh;
            return;
        }

        SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
        if (smr != null)
        {
            currentMesh = smr.sharedMesh;
        }
    }
}

[System.Serializable()]
public class BlendShapeInfo
{
    public string name;
    public int index;

    public List<BlendShapeFrame> frames;

    public static List<BlendShapeInfo> CreateInfo(Mesh m)
    {
        List<BlendShapeInfo> bsiList = new List<BlendShapeInfo>();
        int count = m.blendShapeCount;
        for (int i = 0; i < count; i++)
        {
            BlendShapeInfo bsi = new BlendShapeInfo();
            bsi.Define(m, i);
            bsiList.Add(bsi);
        }
        return bsiList;
    }

    public void Define(Mesh m, int shapeIndex)
    {
        this.index = shapeIndex;
        this.name = m.GetBlendShapeName(shapeIndex);

        int framesCount = m.GetBlendShapeFrameCount(shapeIndex);
        frames = new List<BlendShapeFrame>(framesCount);
        for (int i = 0; i < framesCount; i++)
        {
            BlendShapeFrame bsf = new BlendShapeFrame();

            Vector3[] deltaNormals = new Vector3[m.vertexCount];
            Vector3[] deltaTangents = new Vector3[m.vertexCount];
            Vector3[] deltaVerts = new Vector3[m.vertexCount];

            bsf.weight = m.GetBlendShapeFrameWeight(shapeIndex, i);
            m.GetBlendShapeFrameVertices(shapeIndex, i, deltaVerts, deltaNormals, deltaTangents);

            bsf.deltaVerts = new List<Vector3>(deltaVerts);
            bsf.deltaTangents = new List<Vector3>(deltaTangents);
            bsf.deltaNormals = new List<Vector3>(deltaNormals);

            frames.Add(bsf);
        }
    }

    [System.Serializable()]
    public class BlendShapeFrame
    {
        public float weight;
        public List<Vector3> deltaVerts;
        public List<Vector3> deltaNormals;
        public List<Vector3> deltaTangents;
    }
}


#if UNITY_EDITOR

[CustomEditor(typeof(QuickTriangleEditor), true)]
public class QuickTriangleEditorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        

        UnityEditor.EditorGUILayout.Space();

        QuickTriangleEditor qte = target as QuickTriangleEditor;

        if (GUILayout.Button("Set Copied Mesh as Active."))
        {
            if (qte.lastCreatedMesh != null)
            {

                MeshFilter mf = qte.GetComponent<MeshFilter>();
                if (mf != null)
                    mf.sharedMesh = qte.lastCreatedMesh;

                SkinnedMeshRenderer smr = qte.GetComponent<SkinnedMeshRenderer>();
                if (smr != null)
                    smr.sharedMesh = qte.lastCreatedMesh;

                qte.currentMesh = qte.lastCreatedMesh;
                qte.lastCreatedMesh = null;

                // Get the number of the 
                qte.nameMeshName += " NEW";
            }
        }

        if (GUILayout.Button("Reverse Triangles"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;
            List<int>[] triangles = new List<int>[copiedMesh.subMeshCount];

            for (int i = 0; i < submeshes; i++)
            {
                triangles[i] = new List<int>();
                copiedMesh.GetTriangles(triangles[i], i);
                triangles[i].Reverse();
                copiedMesh.SetTriangles(triangles[i], i);
            }
        }

        if (GUILayout.Button("Create Double Sided Copy"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;
            List<int>[] triangles = new List<int>[copiedMesh.subMeshCount];

            if (qte.newTrianglesAsSubmesh)
                copiedMesh.subMeshCount = submeshes * 2;

            for (int i = 0; i < submeshes; i++)
            {
                triangles[i] = new List<int>();
                copiedMesh.GetTriangles(triangles[i], i);
                if (!qte.newTrianglesAsSubmesh)
                {
                    for (int j = triangles[i].Count - 1; j >= 0; j--)
                    {
                        triangles[i].Add(triangles[i][j]);
                    }
                    copiedMesh.SetTriangles(triangles[i], i);
                }
                else
                {
                    triangles[i].Reverse();
                    copiedMesh.SetTriangles(triangles[i], submeshes + i);
                }

            }
        }



        if (GUILayout.Button("Create Double Sided Copy Based On Colliders"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;
            List<int>[] originalTriangles = new List<int>[copiedMesh.subMeshCount];
            List<int>[] newTriangles = new List<int>[copiedMesh.subMeshCount];

            if (qte.newTrianglesAsSubmesh)
                copiedMesh.subMeshCount = submeshes * 2;

            for (int i = 0; i < submeshes; i++)
            {
                originalTriangles[i] = new List<int>();
                newTriangles[i] = new List<int>();
                copiedMesh.GetTriangles(originalTriangles[i], i);
                copiedMesh.GetTriangles(newTriangles[i], i);

                newTriangles[i].Reverse();
            }

            List<Vector3> verts = new List<Vector3>();
            copiedMesh.GetVertices(verts);
            List<int> excludedVerts = GetExludedVerts(qte, verts, qte.excludeVertsInColliders);

            for (int i = 0; i < submeshes; i++)
            {
                for (int j = newTriangles[i].Count - 1; j >= 0; j -= 3)
                {
                    if (excludedVerts.Contains(newTriangles[i][j]) ||
                        excludedVerts.Contains(newTriangles[i][j - 1]) ||
                        excludedVerts.Contains(newTriangles[i][j - 2]))
                    {
                        newTriangles[i].RemoveRange(j - 2, 3);
                    }
                }

                if (!qte.newTrianglesAsSubmesh)
                {
                    originalTriangles[i].AddRange(newTriangles[i]);
                    copiedMesh.SetTriangles(originalTriangles[i], i);
                }
                else
                {
                    copiedMesh.SetTriangles(newTriangles[i], i + submeshes);
                }
            }
        }

        if (GUILayout.Button("Remove Empty Submeshes"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;

            List<int> submeshesRemaining = new List<int>();

            List<int>[] subMeshTriangles = new List<int>[submeshes];

            for (int i = 0; i < submeshes; i++)
            {

                subMeshTriangles[i] = new List<int>();
                copiedMesh.GetTriangles(subMeshTriangles[i], i);

                if (subMeshTriangles[i].Count > 0)
                    submeshesRemaining.Add(i);
            }

            if (submeshesRemaining.Count == submeshes)
            {
                Debug.Log("No empty submeshes detected.");
                return;
            }

            List<int>[] newSubMeshTriangles = new List<int>[submeshesRemaining.Count];
            copiedMesh.subMeshCount = submeshesRemaining.Count;
            for (int i = 0; i < submeshesRemaining.Count; i++)
            {
                newSubMeshTriangles[i] = subMeshTriangles[submeshesRemaining[i]];
                copiedMesh.SetTriangles(newSubMeshTriangles[i], i, true);
            }

            copiedMesh.RecalculateBounds();
        }

        if (GUILayout.Button("Remove Verts In Collision"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;
            List<int>[] originalTriangles = new List<int>[copiedMesh.subMeshCount];
            List<int>[] newTriangles = new List<int>[copiedMesh.subMeshCount];
            for (int i = 0; i < submeshes; i++)
            {
                originalTriangles[i] = new List<int>();
                newTriangles[i] = new List<int>();

                copiedMesh.GetTriangles(originalTriangles[i], i);
            }

            List<Vector3> verts = new List<Vector3>();
            copiedMesh.GetVertices(verts);
            List<int> excludedVerts = GetExludedVerts(qte, verts, qte.excludeVertsInColliders);

            // Gets colors
            List<Color> vertColors = new List<Color>(copiedMesh.colors);

            // Get normals
            List<Vector3> vertNormals = new List<Vector3>(copiedMesh.normals);

            // Get vertex weights
            List<BoneWeight> boneWeights = new List<BoneWeight>(copiedMesh.boneWeights);

            List<Vector2> uv1 = new List<Vector2>(copiedMesh.uv);
            List<Vector2> uv2 = new List<Vector2>(copiedMesh.uv2);
            List<Vector2> uv3 = new List<Vector2>(copiedMesh.uv3);
            List<Vector2> uv4 = new List<Vector2>(copiedMesh.uv4);
            List<Vector4> vertTangets = new List<Vector4>(copiedMesh.tangents);

            Debug.Log(excludedVerts.Count + ", " + verts.Count);

            List<Vector3> newVertList = new List<Vector3>(verts);

            // Attempts blend mesh stuff
            List<BlendShapeInfo> bsiList = BlendShapeInfo.CreateInfo(copiedMesh);
            copiedMesh.ClearBlendShapes();
            for (int i = 0; i < bsiList.Count; i++)
            {
                for (int j = 0; j < bsiList[i].frames.Count; j++)
                {
                    copiedMesh.AddBlendShapeFrame(bsiList[i].name, bsiList[i].frames[j].weight,
                        bsiList[i].frames[j].deltaVerts.ToArray(), bsiList[i].frames[j].deltaNormals.ToArray(),
                        bsiList[i].frames[j].deltaTangents.ToArray());
                }
            }

            for (int i = excludedVerts.Count - 1; i >= 0; i--)
            {
                EditorUtility.DisplayProgressBar("Clear Data", "Vert " + i + " / " + verts.Count,
                ((float)i / (verts.Count - 1)));

                if (vertColors.Count > 0)
                    vertColors.RemoveAt(excludedVerts[i]);

                if (vertNormals.Count > 0)
                    vertNormals.RemoveAt(excludedVerts[i]);

                if (boneWeights.Count > 0)
                    boneWeights.RemoveAt(excludedVerts[i]);

                if (newVertList.Count > 0)
                    newVertList.RemoveAt(excludedVerts[i]);

                if (uv1.Count > 0)
                    uv1.RemoveAt(excludedVerts[i]);

                if (uv2.Count > 0)
                    uv2.RemoveAt(excludedVerts[i]);

                if (uv3.Count > 0)
                    uv3.RemoveAt(excludedVerts[i]);

                if (uv4.Count > 0)
                    uv4.RemoveAt(excludedVerts[i]);

                if (vertTangets.Count > 0)
                    vertTangets.RemoveAt(excludedVerts[i]);

                for (int j = 0; j < bsiList.Count; j++)
                {
                    for (int k = 0; k < bsiList[j].frames.Count; k++)
                    {
                        bsiList[j].frames[k].deltaNormals.RemoveAt(excludedVerts[i]);
                        bsiList[j].frames[k].deltaTangents.RemoveAt(excludedVerts[i]);
                        bsiList[j].frames[k].deltaVerts.RemoveAt(excludedVerts[i]);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            Debug.Log(newVertList.Count + " || " + verts.Count);

            List<int> updatedIndexValues = new List<int>(verts.Count);
            int reduce = 0;
            for (int i = 0; i < verts.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Updating Indices", "Vert " + i + " / " + verts.Count,
                ((float)i / (verts.Count - 1)));

                if (excludedVerts.Contains(i))
                {
                    updatedIndexValues.Add(-1);
                    reduce++;
                }
                else
                {
                    updatedIndexValues.Add(i - reduce);
                }
            }
            EditorUtility.ClearProgressBar();

            for (int i = 0; i < submeshes; i++)
            {
                for (int j = originalTriangles[i].Count - 1; j >= 0; j -= 3)
                {
                    // We don't add these triangles
                    if (updatedIndexValues[originalTriangles[i][j]] < 0 ||
                        updatedIndexValues[originalTriangles[i][j - 1]] < 0 ||
                        updatedIndexValues[originalTriangles[i][j - 2]] < 0)
                        continue;

                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j]]);
                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j - 1]]);
                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j - 2]]);
                }
                newTriangles[i].Reverse();

                copiedMesh.SetTriangles(newTriangles[i], i);
            }

            

            copiedMesh.SetVertices(newVertList);

            copiedMesh.ClearBlendShapes();
            for (int i = 0; i < bsiList.Count; i++)
            {
                for (int j =0; j < bsiList[i].frames.Count; j++)
                {
                    copiedMesh.AddBlendShapeFrame(bsiList[i].name, bsiList[i].frames[j].weight,
                        bsiList[i].frames[j].deltaVerts.ToArray(),
                        bsiList[i].frames[j].deltaNormals.ToArray(),
                        bsiList[i].frames[j].deltaTangents.ToArray());
                }
            }

            copiedMesh.SetColors(vertColors);
            copiedMesh.SetNormals(vertNormals);
            copiedMesh.SetTangents(vertTangets);

            copiedMesh.boneWeights = boneWeights.ToArray();
            copiedMesh.uv = uv1.ToArray();
            copiedMesh.uv2 = uv2.ToArray();
            copiedMesh.uv3 = uv3.ToArray();
            copiedMesh.uv4 = uv4.ToArray();

            copiedMesh.RecalculateBounds();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Sub Mesh Stuff");

        if (GUILayout.Button("Remove SubMesh"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            List<int[]> subMeshes = new List<int[]>();

            for (int i = 0; i < copiedMesh.subMeshCount; i++)
            {
                if (qte.subMeshesToRemove.Contains(i))
                    continue;

                subMeshes.Add(copiedMesh.GetTriangles(i));
            }

            copiedMesh.subMeshCount = subMeshes.Count;

            for (int i = 0; i < subMeshes.Count; i++)
            {
                copiedMesh.SetTriangles(subMeshes[i], i);
            }

            copiedMesh.RecalculateBounds();
        }

        if (GUILayout.Button("Removed Unused Verts"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            int submeshes = copiedMesh.subMeshCount;
            List<int>[] originalTriangles = new List<int>[copiedMesh.subMeshCount];
            List<int>[] newTriangles = new List<int>[copiedMesh.subMeshCount];
            for (int i = 0; i < submeshes; i++)
            {
                originalTriangles[i] = new List<int>();
                newTriangles[i] = new List<int>();

                copiedMesh.GetTriangles(originalTriangles[i], i);
            }

            List<Vector3> verts = new List<Vector3>();
            copiedMesh.GetVertices(verts);
            List<int> excludedVerts = GetUnsedVerts(qte, verts);

            // Gets colors
            List<Color> vertColors = new List<Color>(copiedMesh.colors);

            // Get normals
            List<Vector3> vertNormals = new List<Vector3>(copiedMesh.normals);

            // Get vertex weights
            List<BoneWeight> boneWeights = new List<BoneWeight>(copiedMesh.boneWeights);

            List<Vector2> uv1 = new List<Vector2>(copiedMesh.uv);
            List<Vector2> uv2 = new List<Vector2>(copiedMesh.uv2);
            List<Vector2> uv3 = new List<Vector2>(copiedMesh.uv3);
            List<Vector2> uv4 = new List<Vector2>(copiedMesh.uv4);
            List<Vector4> vertTangets = new List<Vector4>(copiedMesh.tangents);

            Debug.Log(excludedVerts.Count + ", " + verts.Count);

            List<Vector3> newVertList = new List<Vector3>(verts);

            // Attempts blend mesh stuff
            List<BlendShapeInfo> bsiList = BlendShapeInfo.CreateInfo(copiedMesh);
            copiedMesh.ClearBlendShapes();
            for (int i = 0; i < bsiList.Count; i++)
            {
                for (int j = 0; j < bsiList[i].frames.Count; j++)
                {
                    copiedMesh.AddBlendShapeFrame(bsiList[i].name, bsiList[i].frames[j].weight,
                        bsiList[i].frames[j].deltaVerts.ToArray(), bsiList[i].frames[j].deltaNormals.ToArray(),
                        bsiList[i].frames[j].deltaTangents.ToArray());
                }
            }

            for (int i = excludedVerts.Count - 1; i >= 0; i--)
            {
                EditorUtility.DisplayProgressBar("Clear Data", "Vert " + i + " / " + verts.Count,
                ((float)i / (verts.Count - 1)));

                if (vertColors.Count > 0)
                    vertColors.RemoveAt(excludedVerts[i]);

                if (vertNormals.Count > 0)
                    vertNormals.RemoveAt(excludedVerts[i]);

                if (boneWeights.Count > 0)
                    boneWeights.RemoveAt(excludedVerts[i]);

                if (newVertList.Count > 0)
                    newVertList.RemoveAt(excludedVerts[i]);

                if (uv1.Count > 0)
                    uv1.RemoveAt(excludedVerts[i]);

                if (uv2.Count > 0)
                    uv2.RemoveAt(excludedVerts[i]);

                if (uv3.Count > 0)
                    uv3.RemoveAt(excludedVerts[i]);

                if (uv4.Count > 0)
                    uv4.RemoveAt(excludedVerts[i]);

                if (vertTangets.Count > 0)
                    vertTangets.RemoveAt(excludedVerts[i]);

                for (int j = 0; j < bsiList.Count; j++)
                {
                    for (int k = 0; k < bsiList[j].frames.Count; k++)
                    {
                        bsiList[j].frames[k].deltaNormals.RemoveAt(excludedVerts[i]);
                        bsiList[j].frames[k].deltaTangents.RemoveAt(excludedVerts[i]);
                        bsiList[j].frames[k].deltaVerts.RemoveAt(excludedVerts[i]);
                    }
                }
            }
            EditorUtility.ClearProgressBar();

            Debug.Log(newVertList.Count + " || " + verts.Count);

            List<int> updatedIndexValues = new List<int>(verts.Count);
            int reduce = 0;
            for (int i = 0; i < verts.Count; i++)
            {
                EditorUtility.DisplayProgressBar("Updating Indices", "Vert " + i + " / " + verts.Count,
                ((float)i / (verts.Count - 1)));

                if (excludedVerts.Contains(i))
                {
                    updatedIndexValues.Add(-1);
                    reduce++;
                }
                else
                {
                    updatedIndexValues.Add(i - reduce);
                }
            }
            EditorUtility.ClearProgressBar();

            for (int i = 0; i < submeshes; i++)
            {
                for (int j = originalTriangles[i].Count - 1; j >= 0; j -= 3)
                {
                    // We don't add these triangles
                    if (updatedIndexValues[originalTriangles[i][j]] < 0 ||
                        updatedIndexValues[originalTriangles[i][j - 1]] < 0 ||
                        updatedIndexValues[originalTriangles[i][j - 2]] < 0)
                        continue;

                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j]]);
                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j - 1]]);
                    newTriangles[i].Add(updatedIndexValues[originalTriangles[i][j - 2]]);
                }
                newTriangles[i].Reverse();

                copiedMesh.SetTriangles(newTriangles[i], i);
            }



            copiedMesh.SetVertices(newVertList);

            copiedMesh.ClearBlendShapes();
            for (int i = 0; i < bsiList.Count; i++)
            {
                for (int j = 0; j < bsiList[i].frames.Count; j++)
                {
                    copiedMesh.AddBlendShapeFrame(bsiList[i].name, bsiList[i].frames[j].weight,
                        bsiList[i].frames[j].deltaVerts.ToArray(),
                        bsiList[i].frames[j].deltaNormals.ToArray(),
                        bsiList[i].frames[j].deltaTangents.ToArray());
                }
            }

            copiedMesh.SetColors(vertColors);
            copiedMesh.SetNormals(vertNormals);
            copiedMesh.SetTangents(vertTangets);

            copiedMesh.boneWeights = boneWeights.ToArray();
            copiedMesh.uv = uv1.ToArray();
            copiedMesh.uv2 = uv2.ToArray();
            copiedMesh.uv3 = uv3.ToArray();
            copiedMesh.uv4 = uv4.ToArray();

            copiedMesh.RecalculateBounds();
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Blend Shape Tools");

        if (GUILayout.Button("Remove Blend Shapes"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            copiedMesh.ClearBlendShapes();
        }

        if (GUILayout.Button("Create Test BlendShape"))
        {
            Mesh copiedMesh = CreateMeshCopy(qte);

            BlendShapeInfo bsi = new BlendShapeInfo();
            bsi.name = "TEST";
            BlendShapeInfo.BlendShapeFrame frame = new BlendShapeInfo.BlendShapeFrame();
            frame.weight = 1f;

            frame.deltaVerts = new List<Vector3>();
            frame.deltaNormals = new List<Vector3>();
            frame.deltaTangents = new List<Vector3>();
            for (int i = 0; i < copiedMesh.vertexCount; i++)
            {
                frame.deltaVerts.Add(Vector3.left);
                frame.deltaNormals.Add(Vector3.zero);
                frame.deltaTangents.Add(Vector3.zero);
            }

            copiedMesh.AddBlendShapeFrame(bsi.name, 1f,
                frame.deltaVerts.ToArray(),
                frame.deltaNormals.ToArray(),
                frame.deltaTangents.ToArray());
        }

        if (GUILayout.Button("Save Created Copy"))
        {
            qte.OnValidate();

            try
            {
                string path = AssetDatabase.GetAssetPath(qte.currentMesh);
                path = System.IO.Path.GetDirectoryName(path);
                AssetDatabase.CreateAsset(qte.lastCreatedMesh, path + "/" + qte.lastCreatedMesh.name + ".asset");
                Selection.activeObject = qte.lastCreatedMesh;
            }
            catch
            {
                string path = "Assets/";
                AssetDatabase.CreateAsset(qte.lastCreatedMesh, path + qte.lastCreatedMesh.name + ".asset");
                Selection.activeObject = qte.lastCreatedMesh;
            }
        }
    }

    private List<int> GetUnsedVerts(QuickTriangleEditor qte, List<Vector3> verts)
    {
        List<int> vertIndiciesToRemove = new List<int>();

        int vCount = qte.currentMesh.vertexCount;
        for (int i = 0; i < vCount; i++)
        {
            vertIndiciesToRemove.Add(i);
        }

        int subMeshes = qte.currentMesh.subMeshCount;
        for (int i = 0; i < subMeshes; i++)
        {
            var tris = qte.currentMesh.GetTriangles(i);
            for (int j = 0; j < tris.Length; j++)
            {
                vertIndiciesToRemove.Remove(tris[j]);
            }
        }

        return vertIndiciesToRemove;
    }

    private static Mesh CreateMeshCopy(QuickTriangleEditor qte)
    {
        qte.OnValidate();

        Mesh copiedMesh = Instantiate(qte.currentMesh);
        copiedMesh.name = qte.nameMeshName;
        qte.lastCreatedMesh = copiedMesh;
        return copiedMesh;
    }

    private static List<int> GetExludedVerts(QuickTriangleEditor qte, List<Vector3> verts, bool excludeVerts)
    {
        List<int> excludedVerts = new List<int>();
        for (int i = 0; i < verts.Count; i++)
        {
            EditorUtility.DisplayProgressBar("Exluding Verts", "Vert " + i + " / " + verts.Count,
                ((float)i / (verts.Count - 1)));

            Vector3 worldPos = qte.transform.TransformPoint(verts[i]);
            for (int j = 0; j < qte.colliders.Length; j++)
            {
                BoxCollider bc = qte.colliders[j] as BoxCollider;
                if (bc != null)
                {
                    if (bc.bounds.Contains(worldPos) == excludeVerts)
                    {
                        excludedVerts.Add(i);
                    }
                    continue;
                }

                SphereCollider sc = qte.colliders[j] as SphereCollider;
                if (sc != null)
                {
                    if ((Vector3.Distance(sc.bounds.center, worldPos) < sc.radius) == excludeVerts)
                    {
                        excludedVerts.Add(i);
                    }
                    continue;
                }
            }
        }
        EditorUtility.ClearProgressBar();


        if (!excludeVerts)
        {
            List<int> exExclusions = new List<int>();

            List<int>[] triangles = new List<int>[qte.currentMesh.subMeshCount];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new List<int>();
                qte.currentMesh.GetTriangles(triangles[i], i);

                for (int j = 0; j < triangles[i].Count; j += 3)
                {
                    EditorUtility.DisplayProgressBar("Exluding Tris SubMesh:  " + i,
                        "Triangle " + j + " / " + triangles[i].Count, (float)j / triangles[i].Count);

                    if (!excludedVerts.Contains(triangles[i][j]) ||
                        !excludedVerts.Contains(triangles[i][j + 1]) ||
                        !excludedVerts.Contains(triangles[i][j + 2]))
                    {
                        exExclusions.Add(triangles[i][j]);
                        exExclusions.Add(triangles[i][j + 1]);
                        exExclusions.Add(triangles[i][j + 2]);
                    }
                }
            }

            for (int i = 0; i < exExclusions.Count; i++)
                excludedVerts.Remove(exExclusions[i]);
        }
        EditorUtility.ClearProgressBar();

        return excludedVerts;
    }
}

#endif
