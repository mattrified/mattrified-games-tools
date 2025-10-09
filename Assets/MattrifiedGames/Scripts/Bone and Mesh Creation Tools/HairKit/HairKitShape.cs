using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.HairKit
{
    [ExecuteInEditMode()]
    public class HairKitShape : MonoBehaviour
    {
        public Gradient gradient = new Gradient()
        {
            colorKeys = new GradientColorKey[]
            {
            new GradientColorKey(Color.red, 0f),
            new GradientColorKey(Color.yellow, 0.5f),
            new GradientColorKey(Color.green, 1f)
            },
            alphaKeys = new GradientAlphaKey[]
            {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(1f, 1f),
            }
        };

        public List<float> uvPercentages = new List<float>();

        [ContextMenu("Clear")]
        public void Clear()
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var t = transform.GetChild(0);
                if (Application.isEditor && !Application.isPlaying)
                {
                    DestroyImmediate(t.gameObject);
                }
                else
                {
                    Destroy(t.gameObject);
                }
            }
        }

        public void Update()
        {
            uvPercentages.Clear();
            float dist = 0f;
            uvPercentages.Add(0f);
            for (int i = 1; i < transform.childCount; i++)
            {
                dist += Vector3.Distance(transform.GetChild(i - 1).position, transform.GetChild(i).position);
                uvPercentages.Add(dist);
            }

            float find = uvPercentages[uvPercentages.Count - 1];
            for (int i = 0; i < uvPercentages.Count; i++)
            {
                uvPercentages[i] = uvPercentages[i] / find;
            }
        }

        private void OnDrawGizmos()
        {
            if (transform.childCount < 2)
                return;

            for (int i = 1; i < transform.childCount; i++)
            {
                Gizmos.color = gradient.Evaluate(Mathf.InverseLerp(0, transform.childCount, i - 1));
                Transform a = transform.GetChild(i - 1);
                Transform b = transform.GetChild(i);
                Gizmos.DrawLine(a.position, b.position);
            }
        }
    }
}