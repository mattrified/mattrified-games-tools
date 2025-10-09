using UnityEngine;
using System.Collections.Generic;

#if USING_CURVY
using FluffyUnderware.Curvy.Generator.Modules;
using FluffyUnderware.Curvy;
#endif

namespace MattrifiedGames.HairKit
{
    [ExecuteInEditMode()]
    public class HairKitLine : MonoBehaviour
    {
        public HairKitShape shape;

        public float uniformScale = 1f;
        public AnimationCurve uniformScaleCurve = AnimationCurve.Linear(0, 1f, 1f, 1f);
        public AnimationCurve xScale = AnimationCurve.Linear(0, 1f, 1f, 1f);
        public AnimationCurve yScale = AnimationCurve.Linear(0, 1f, 1f, 1f);

        public float uniformTwist;
        public float twist;

        public List<Vector3> vList = new List<Vector3>(10000);

        public Rect uvRect = new Rect(0f, 0f, 1f, 1f);

        public int segments;

        public bool flipNormal;

        public bool evaluateGoalTransform;
        public Transform goalTransform;
        public List<HairKitLinePoint> children = new List<HairKitLinePoint>();

        public bool usePosCurve;
        public Vector3 offset = Vector3.forward;
        public AnimationCurve localOffsetCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public bool useEulerCurve;
        public Vector3 localEuler;
        public AnimationCurve eulerCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public bool useSmoothLineHelper;
        public HairKitSmoothedLineHelper smoothLineHelper;

        public bool autoAngle;
        public Vector3 autoAngleUp = Vector3.up;

        public List<float> uvPercentages = new List<float>();

        public Transform addTransform;

#if USING_CURVY
    public bool useCurveyModule;
    public CurvySpline curveyModule;
#endif

        private void Awake()
        {
            //if (Application.isPlaying)
            //    enabled = false;
        }

        void UpdateUVPercentages()
        {
            uvPercentages.Clear();
            float dist = 0f;
            uvPercentages.Add(0f);
            for (int i = 1; i < children.Count; i++)
            {
                dist += Vector3.Distance(children[i - 1].transform.position, children[i].transform.position);
                uvPercentages.Add(dist);
            }

            float find = uvPercentages[uvPercentages.Count - 1];
            for (int i = 0; i < uvPercentages.Count; i++)
            {
                uvPercentages[i] = uvPercentages[i] / find;
            }
        }

        private void Update()
        {
            vList.Clear();

            children.Clear();
            children.AddRange(GetComponentsInChildren<HairKitLinePoint>());
            segments = children.Count;

            if (segments < 2)
                return;

            if (evaluateGoalTransform && goalTransform != null)
            {
                float dist = Vector3.Distance(children[0].transform.position, goalTransform.position);
                for (int i = 1; i < children.Count; i++)
                {
                    children[i].transform.localPosition = new Vector3(0, 0, dist / segments);
                }
            }

#if USING_CURVY
        if (useCurveyModule && curveyModule != null)
        {
            var curveyT = curveyModule.transform;
            float seg = curveyModule.Length / (segments - 1);
            for (int i = 0; i < segments; i++)
            {
                children[i].transform.position = curveyModule.transform.position + curveyModule.transform.rotation * curveyModule.InterpolateByDistance(i * seg);
            }
        }
#endif

            if (!useSmoothLineHelper || smoothLineHelper == null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    if (useEulerCurve)
                        children[i].transform.localEulerAngles = localEuler * eulerCurve.Evaluate((i) / (float)children.Count);

                    if (usePosCurve && i > 0)
                    {
                        children[i].transform.localPosition = offset * localOffsetCurve.Evaluate((i) / (float)children.Count);
                    }
                }
            }
            else
            {
                smoothLineHelper.smoothedPointCount = children.Count;
                smoothLineHelper.LateUpdate();

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].transform.position = smoothLineHelper.curvedPoints[i];
                    children[i].transform.rotation = smoothLineHelper.orientations[i];
                }
            }

            if (autoAngle)
            {
                var angleUpNormalized = autoAngleUp.normalized;

                List<Quaternion> o = new List<Quaternion>();
                List<Vector3> p = new List<Vector3>();

                Vector3 diff = children[0].lockPosition - children[1].lockPosition;
                diff.Normalize();
                o.Add(Quaternion.LookRotation(diff, angleUpNormalized));
                p.Add(children[0].transform.position);

                for (int i = 1; i < children.Count; i++)
                {
                    diff = children[i - 1].lockPosition - children[i].lockPosition;
                    diff.Normalize();
                    o.Add(Quaternion.LookRotation(diff, angleUpNormalized));
                    p.Add(children[i].transform.position);
                }

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].transform.rotation = o[i];
                    children[i].transform.position = p[i];
                }
            }

            float tw = 0;

            for (int i = 0; i < children.Count; i++)
            {
                Quaternion oldRot = children[i].transform.rotation;

                children[i].transform.Rotate(0, 0, uniformTwist + tw, Space.Self);

                for (int j = 0; j < shape.transform.childCount; j++)
                {
                    Vector3 scaledPos = shape.transform.GetChild(j).localPosition * uniformScale;
                    scaledPos *= uniformScaleCurve.Evaluate(1f - Mathf.InverseLerp(0, segments - 1, i));
                    scaledPos.x *= xScale.Evaluate(1f - Mathf.InverseLerp(0, segments - 1, i));
                    scaledPos.y *= yScale.Evaluate(1f - Mathf.InverseLerp(0, segments - 1, i));

                    Vector3 poinst = children[i].transform.position + children[i].transform.rotation * scaledPos;
                    vList.Add(poinst);
                }

                tw += twist;

                children[i].transform.rotation = oldRot;
            }

            UpdateUVPercentages();
        }

        public bool hideGizmo;
        public bool drawBoxes;

        private void OnDrawGizmos()
        {
            if (shape == null || hideGizmo)
                return;

            Gizmos.color = new Color(1, 1, 1, 0.5f);
#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == this.gameObject)
                Gizmos.color = Color.white;
#endif


            for (int i = 0; i < vList.Count; i += shape.transform.childCount)
            {
                for (int j = 0; j < shape.transform.childCount; j++)
                {
                    if (drawBoxes)
                        Gizmos.DrawWireCube(vList[i + j], Vector3.one * 0.1f);

                    int index = i + j;
                    int next = i + j + 1;

                    if (next % shape.transform.childCount != 0)
                    {
                        Gizmos.DrawLine(vList[index], vList[next]);
                    }

                    int nextRow = i + j + shape.transform.childCount;
                    if (nextRow < vList.Count)
                    {
                        Gizmos.DrawLine(vList[index], vList[nextRow]);
                    }
                }
            }
        }
    }
}