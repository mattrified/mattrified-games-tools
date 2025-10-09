using UnityEngine;
using System.Collections;
using TrueSync;
using System.Collections.Generic;

public class UnevenEnvironmentManager : EnvironmentManager
{
    [Range(0,100)]
    public int rows;

    [Range(0, 100)]
    public int columns;
    public List<FP> heightCoordinates;

    [SerializeField()]
    FP minX, minZ;

    [SerializeField()]
    Gradient grad;

    public override TSVector CheckPos(FP charRadius, TSVector position)
    {
        TSVector pos = base.CheckPos(charRadius, position);

        pos = SetupPosition(pos, true);

        return pos;
    }

    // TODO:  do height and other things.  I think TFManager checks ground, which should be run through this.
    public TSVector SetupPosition(TSVector pos, bool yOnly)
    {
        int minRow = TSMath.Floor(pos.x - minX).AsInt();
        int maxRow = minRow + 1;
        FP fracX = (pos.x - minX) - minRow;

        if (minRow < 0)
        {
            minRow = 0;
            maxRow = 0;
            if (!yOnly)
                pos.x = minX;
            fracX = 0;
        }
        if (maxRow >= rows)
        {
            minRow = rows - 1;
            maxRow = rows - 1;
            if (!yOnly)
                pos.x = rows - 1 - minX;
            fracX = 0;
        }

        int minCol = TSMath.Floor(pos.z - minZ).AsInt();
        int maxCol = minCol + 1;
        FP fracZ = (pos.z - minZ) - minCol;

        if (minCol < 0)
        {
            minCol = 0;
            maxCol = 0;
            if (!yOnly)
                pos.z = minZ;
            fracZ = 0;
        }
        if (maxCol >= columns)
        {
            minCol = columns - 1;
            maxCol = columns - 1;
            if (!yOnly)
                pos.z = columns - 1 - minZ;
            fracZ = 0;
        }

        FP c1 = heightCoordinates[minRow * columns + minCol];
        FP c2 = heightCoordinates[minRow * columns + maxCol];
        FP c3 = heightCoordinates[maxRow * columns + minCol];
        FP c4 = heightCoordinates[maxRow * columns + maxCol];

        FP x1 = TSLerp(c1, c2, fracZ);
        FP x2 = TSLerp(c3, c4, fracZ);
        FP x3 = TSLerp(x1, x2, fracX);

        pos.y = x3;

        return pos;
    }

    private void OnValidate()
    {
        if (heightCoordinates.Count != rows * columns)
        {
            Debug.LogWarning("Not enough height coordingates for size:  " + (rows * columns));
        }
    }

    public static FP TSLerp(FP min, FP max, FP percent)
    {
        return percent * (max - min) + min;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        for (int x =0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Vector3 pnt = SetupPosition(new TSVector(x + minX, 0, z + minZ), true).ToVector();
                Gizmos.color = grad.Evaluate(pnt.y);
                Gizmos.DrawWireCube(pnt, Vector3.one * 0.1f);
            }
        }
    }

    [ContextMenu("Create Mesh")]
    public void CreateMesh()
    {
        MeshFilter mf = gameObject.GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();

        MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
        if (mr == null)
            mr = gameObject.AddComponent<MeshRenderer>();

        Mesh m = mf.sharedMesh;
        if (m == null)
            m = new Mesh() { name = this.name + " mesh" };

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 v = new Vector3(i + minX.AsFloat(), heightCoordinates[i * columns + j].AsFloat(), j + minZ.AsFloat());
                verts.Add(v);

                Vector2 uv = new Vector2();
                uv.x = i / (float)Mathf.Max(rows, columns);
                uv.y = j / (float)Mathf.Max(columns, rows);
                uvs.Add(uv);

                int a = i * columns + j;
                int b = i * columns + j + 1;
                int c = a + columns;
                int d = b + columns;

                if (j == columns - 1 || i == rows - 1)
                    continue;
                /*if (b >= rows * columns || c >= rows * columns || d >= rows * columns)
                    continue;*/

                tris.Add(a);
                tris.Add(b);
                tris.Add(c);

                tris.Add(d);
                tris.Add(c);
                tris.Add(b);
                //0,1,2,3,2,1;
                //0,1,2,0,2,3*/
            }


        }


        m.Clear();
        m.SetVertices(verts);
        m.SetTriangles(tris, 0);
        m.SetUVs(0, uvs);
        m.RecalculateNormals();

        mf.sharedMesh = m;
    }
}