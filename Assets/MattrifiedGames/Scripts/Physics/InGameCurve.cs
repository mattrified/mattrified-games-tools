using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls 2D Curves
/// TODO:  Add 2d physics simulation and 3D rendering environment stuff.
/// Unsure if players should also work on the 2D simulation layer yet.
/// This may be a MerFight only thing as the fighting game can't do this.
/// </summary>
public class InGameCurve : InGameCurveBase
{
    [Range(10, 1000)]
    public int smoothness = 10;

    public AnimationCurve angleCurve;

    public AnimationCurve yMinCurve = AnimationCurve.Linear(0, 0, 1, 0);
    public AnimationCurve yMaxCurve = AnimationCurve.Linear(0, 1, 1, 1);

    public AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve zCurve = AnimationCurve.Linear(0, 0, 1, 0);

    [SerializeField()]
    protected WrapMode wrapMode;

    public bool useChildrenToPreviewCurve;

    public override bool Loop
    {
        get
        {
            return wrapMode == WrapMode.Loop;
        }
    }

    protected override void Awake()
    {
        DefineDistance();

        angleCurve.preWrapMode = wrapMode;
        angleCurve.postWrapMode = wrapMode;

        xCurve.preWrapMode = wrapMode;
        xCurve.postWrapMode = wrapMode;

        zCurve.preWrapMode = wrapMode;
        zCurve.postWrapMode = wrapMode;

        yMinCurve.preWrapMode = wrapMode;
        yMinCurve.postWrapMode = wrapMode;

        yMaxCurve.preWrapMode = wrapMode;
        yMaxCurve.postWrapMode = wrapMode;

        if (curveList != null)
            curveList.Add(this);
    }

    public override float Percent(float xPos)
    {
        return xPos / distance;
    }

    [ContextMenu("Define Distance")]
    public override void DefineDistance()
    {
        distance = angleCurve[angleCurve.length - 1].time;
    }

    internal override void GetFloorPoint(float distance, ref Vector3 vector3)
    {
        vector3.x = xCurve.Evaluate(distance);
        vector3.y = transform.position.y + yMinCurve.Evaluate(distance);
        vector3.z = zCurve.Evaluate(distance);
    }

    internal override void GetCeilingPoint(float distance, ref Vector3 vector3)
    {
        vector3.x = xCurve.Evaluate(distance);
        vector3.y = transform.position.y + yMaxCurve.Evaluate(distance);
        vector3.z = zCurve.Evaluate(distance);
    }

    [ContextMenu("Define Curve")]
    internal void Define()
    {
        Vector3 startPos = transform.position;
        Vector3 startEuler = transform.eulerAngles;
        
        GameObject go = gameObject;
        go.transform.eulerAngles = new Vector3(0, angleCurve.Evaluate(0), 0);

        xCurve = new AnimationCurve();
        zCurve = new AnimationCurve();

        float distance = angleCurve[angleCurve.length - 1].time;
        //Debug.Log(distance);

        for (int i = 0; i <= smoothness; i++)
        {
            float percent = (float)i / (smoothness);
            float pos = percent * distance;
            xCurve.AddKey(new Keyframe(pos, go.transform.position.x));
            zCurve.AddKey(new Keyframe(pos, go.transform.position.z));

            float f = angleCurve.Evaluate(pos);
            //Debug.Log(f);
            go.transform.eulerAngles = new Vector3(0, f, 0);
            go.transform.position += go.transform.forward * distance / (smoothness + 1);
        }

        for (int i = 0; i < xCurve.length; i++)
        {
            xCurve.SmoothTangents(i, 0);
            zCurve.SmoothTangents(i, 0);
        }

        transform.position = startPos;
        transform.eulerAngles = startEuler;

        DefineDistance();
    }

    [ContextMenu("Build Mesh From Curve")]
    public void BuildMesh()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
            mf = gameObject.AddComponent<MeshFilter>();

        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr == null)
            mr = gameObject.AddComponent<MeshRenderer>();

        Mesh m = mf.sharedMesh;
        if (m == null)
        {
            m = new Mesh();
            mf.sharedMesh = m;
        }
        m.name = this.name + " Mesh";

        m.Clear();

        List<Vector3> points = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        float distance = angleCurve[angleCurve.length - 1].time;
        for (int i = 0; i <= smoothness; i++)
        {
            float percent = (float)i / smoothness;
            float dist = percent * distance;

            Vector3 pLeft, pRight;

            Quaternion quat = Quaternion.Euler(0, angleCurve.Evaluate(dist), 0);
            

            Vector3 pos = Vector3.zero;
            GetFloorPoint(dist, ref pos);

            pLeft = pos + quat * Vector3.left;
            pRight = pos + quat * Vector3.right;

            points.Add(transform.InverseTransformPoint(pLeft));
            points.Add(transform.InverseTransformPoint(pRight));

            uvs.Add(new Vector2(0, percent));
            uvs.Add(new Vector2(1, percent));
        }

        for (int i = 0; i < points.Count - 2; i += 2)
        {
            tris.Add(i);
            tris.Add(i + 2);
            tris.Add(i + 1);

            tris.Add(i + 1);
            tris.Add(i + 2);
            tris.Add(i + 3);
        }

        m.SetVertices(points);
        m.SetUVs(0, uvs);
        m.SetTriangles(tris, 0);

        m.RecalculateBounds();
        m.RecalculateNormals();
        m.RecalculateTangents();
    }

    internal override void GetAllFloorCeilAngleY(float distance, ref Vector3 floor, ref Vector3 ceiling, ref float angle)
    {
        GetFloorPoint(distance, ref floor);
        GetCeilingPoint(distance, ref ceiling);
        GetEulerY(distance, ref angle);
    }

    internal override void GetEulerY(float distance, ref float angle)
    {
        angle = angleCurve.Evaluate(distance);
    }

#if UNITY_EDITOR
    [ContextMenu("Create CUrves from Children")]
    void CreateCurvesFromChildren()
    {
        useChildrenToPreviewCurve = false;

        float totalDistance = 0;
        Vector3 lastPos = transform.position;
        float eulerY = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;

        angleCurve = new AnimationCurve();
        angleCurve.AddKey(new Keyframe(0f, eulerY));

        yMinCurve = new AnimationCurve();
        yMinCurve.AddKey(new Keyframe(0f, 0f));

        yMaxCurve = new AnimationCurve();
        yMaxCurve.AddKey(new Keyframe(0f, 10f));

        foreach (Transform t in transform)
        {
            //Gizmos.DrawLine(lastPos, t.position);
            float dist = Vector3.Distance(lastPos, t.position);
            totalDistance += dist;
            lastPos = t.position;
            angleCurve.AddKey(totalDistance, UnityEditor.TransformUtils.GetInspectorRotation(t).y + eulerY);

            yMinCurve.AddKey(new Keyframe(totalDistance, t.localPosition.y));
            yMaxCurve.AddKey(new Keyframe(totalDistance, t.localPosition.y + 10f));
        }

        for (int i = 0; i < angleCurve.length; i++)
        {
            angleCurve.SmoothTangents(i, 0);
            yMaxCurve.SmoothTangents(i, 0);
            yMinCurve.SmoothTangents(i, 0);
        }


        Define();
    }

    private void OnDrawGizmos()
    {
        if (!useChildrenToPreviewCurve)
        {
            float distance = angleCurve[angleCurve.length - 1].time;

            Vector3 a = Vector3.zero;
            Vector3 b = Vector3.zero;
            for (float f = 0f; f < distance; f += distance / 100f)
            {
                Gizmos.color = color.Evaluate(f / distance);

                GetFloorPoint(f, ref a);
                GetFloorPoint(f + distance / 100f, ref b);
                Gizmos.DrawLine(a, b);
            }
        }
        else
        {
            float totalDistance = 0;
            Vector3 lastPos = transform.position;
            AnimationCurve angleCurve = new AnimationCurve();

            float eulerY = UnityEditor.TransformUtils.GetInspectorRotation(transform).y;
            angleCurve.AddKey(new Keyframe(0f, eulerY));

            AnimationCurve yCurve = new AnimationCurve();
            yCurve.AddKey(new Keyframe(0, 0));

            foreach (Transform t in transform)
            {
                //Gizmos.DrawLine(lastPos, t.position);
                Vector3 diff = t.position - lastPos;
                diff.y = 0f;
                float dist = diff.magnitude;
                
                totalDistance += dist;
                lastPos = t.position;
                angleCurve.AddKey(totalDistance, UnityEditor.TransformUtils.GetInspectorRotation(t).y + eulerY);
                yCurve.AddKey(totalDistance, t.localPosition.y);
            }

            for (int i = 0; i < angleCurve.length; i++)
            {
                angleCurve.SmoothTangents(i, 0);
                yCurve.SmoothTangents(i, 0);
            }

            lastPos = transform.position;
            for (int i = 0; i <= smoothness; i++)
            {
                float percent = (float)i / (smoothness);
                float pos = percent * totalDistance;

                Vector3 forward = Quaternion.Euler(0, angleCurve.Evaluate(pos), 0) * Vector3.forward;
                Vector3 newPos = lastPos + forward * (totalDistance / (smoothness + 1));

                newPos.y = transform.position.y + yCurve.Evaluate(pos);

                Gizmos.DrawLine(lastPos, newPos);

                lastPos = newPos;
            }
        }
    }


#endif
}