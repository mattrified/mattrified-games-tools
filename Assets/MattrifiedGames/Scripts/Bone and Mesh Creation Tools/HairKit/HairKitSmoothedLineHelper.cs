using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.HairKit
{
    [ExecuteInEditMode()]
    public class HairKitSmoothedLineHelper : MonoBehaviour
    {
        [SerializeField()]
        Transform[] transforms = null;

        [SerializeField()]
        public int smoothedPointCount;

        public List<Vector3> transformPoints = new List<Vector3>();

        public List<Vector3> points = new List<Vector3>();
        public List<Vector3> curvedPoints = new List<Vector3>();

        public List<Quaternion> orientations = new List<Quaternion>();

        public Gradient gradient;

        public Vector3 upVector = Vector3.up;
        public bool showOrientationRays;

        public bool autoSetTransforms;

        private void OnValidate()
        {
            upVector.Normalize();

            if (autoSetTransforms)
                transforms = GetComponentsInChildren<Transform>();

            smoothedPointCount = Mathf.Max(smoothedPointCount, transforms.Length);
            transformPoints = new List<Vector3>(transforms.Length);

            curvedPoints = new List<Vector3>(smoothedPointCount + 1);

            SetPositions();
        }

        private void SetPositions()
        {
            transformPoints.Clear();
            for (int i = 0; i < transforms.Length; i++)
                transformPoints.Add(transforms[i].position);

            MakeSmoothCurve(transformPoints);
        }

        public void LateUpdate()
        {
            SetPositions();

            orientations.Clear();

            

            Vector3 diff = curvedPoints[0] - curvedPoints[1];
            diff.Normalize();
            orientations.Add(Quaternion.LookRotation(diff, upVector));

            for (int i = 1; i < curvedPoints.Count; i++)
            {
                diff = curvedPoints[i - 1] - curvedPoints[i];
                diff.Normalize();
                orientations.Add(Quaternion.LookRotation(diff, upVector));
            }
        }

        private void OnDrawGizmos()
        {
            for (int i = 1; i < transforms.Length; i++)
            {
                Gizmos.DrawLine(transforms[i - 1].position, transforms[i].position);
            }

            for (int i = 1; i < curvedPoints.Count; i++)
            {
                Gizmos.color = gradient.Evaluate((i - 1f) / (curvedPoints.Count - 1f));
                Gizmos.DrawLine(curvedPoints[i - 1], curvedPoints[i]);
            }

            if (showOrientationRays)
            {
                for (int i = 0; i < orientations.Count; i++)
                {
                    Gizmos.DrawRay(curvedPoints[i], orientations[i] * Vector3.up * 0.1f);
                }
            }
        }

        public void MakeSmoothCurve(List<Vector3> arrayToCurve)
        {
            int pointsLength = 0;
            int curvedLength = 0;
            
            pointsLength = arrayToCurve.Count;
            curvedLength = smoothedPointCount;

            curvedPoints.Clear();

            float t = 0.0f;
            for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
            {
                t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

                points.Clear();
                points.AddRange(arrayToCurve);

                for (int j = pointsLength - 1; j > 0; j--)
                {
                    for (int i = 0; i < j; i++)
                    {
                        transformPoints[i] = (1 - t) * transformPoints[i] + t * transformPoints[i + 1];
                    }
                }

                curvedPoints.Add(points[0]);
            }
            curvedPoints.RemoveAt(0);
        }
    }
}
