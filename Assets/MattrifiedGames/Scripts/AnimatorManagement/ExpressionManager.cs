using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.ManagedAnimation
{
    public class ExpressionManager : MonoBehaviour
    {
        public Animator animRef;
        public SkinnedMeshRenderer[] skinnedRenderers;
        public Transform[] otherTransforms;

        public List<Expression> expressions;

        public float totalWeight;

        public Expression defaultExpression;

        [System.Serializable()]
        public class Expression
        {
            public bool useAnimParameter;
            public StringHash animParameter;

            public string expressionName;
            
            [Range(0f,1f)]
            public float weight = 0;

            public ExpressionBlendShapeInfo[] blendShapeInfo;

            public Vector3[] localPositions;
            public Vector3[] localRotations;
            public Vector3[] localScale;

            public void CopyTo(Expression otherExpression, string name)
            {
                otherExpression.expressionName = name;
                otherExpression.weight = weight;
                otherExpression.blendShapeInfo = new ExpressionBlendShapeInfo[blendShapeInfo.Length];
                for (int i = 0; i < blendShapeInfo.Length; i++)
                {
                    otherExpression.blendShapeInfo[i] = new ExpressionBlendShapeInfo() { blendShapeWeights = (float[])blendShapeInfo[i].blendShapeWeights.Clone() };
                }
                otherExpression.localPositions = (Vector3[])localPositions.Clone();
                otherExpression.localRotations = (Vector3[])localRotations.Clone();
                otherExpression.localScale = (Vector3[])localScale.Clone();
            }
        }

        [System.Serializable()]
        public class ExpressionBlendShapeInfo
        {
            [Range(0f,100f)]
            public float[] blendShapeWeights;
        }

        private void OnValidate()
        {
            if (expressions.Count > 0)
            {
                expressions[0].CopyTo(defaultExpression, "Default");
            }

            DefineTotalWeight();
            UpdateExpression(true);
        }

        private void Awake()
        {
            
        }

        void DefineTotalWeight()
        {
            totalWeight = 0f;
            for (int i = 0; i < expressions.Count; i++)
            {
                totalWeight += expressions[i].weight;
            }
            totalWeight = Mathf.Max(1f, totalWeight);
        }

        private void LateUpdate()
        {
            for (int i =0; i < expressions.Count; i++)
            {
                if (expressions[i].useAnimParameter)
                {
                    totalWeight -= expressions[i].weight;
                    expressions[i].weight = animRef.GetFloat(expressions[i].animParameter.Hash);
                    totalWeight += expressions[i].weight;
                }
            }

            UpdateExpression(false);
        }

        public void UpdateExpression(bool assignDefault)
        {
            if (Mathf.Approximately(0f, totalWeight))
                return;

            for (int i = 0; i < defaultExpression.blendShapeInfo.Length; i++)
            {
                for (int j = 0; j < defaultExpression.blendShapeInfo[i].blendShapeWeights.Length; j++)
                {
                    float value = assignDefault ? defaultExpression.blendShapeInfo[i].blendShapeWeights[j] :
                        skinnedRenderers[i].GetBlendShapeWeight(j);

                    for (int k = 0; k < expressions.Count; k++)
                    {
                        if (Mathf.Approximately(0f, expressions[k].weight))
                            continue;

                        float lWeight = expressions[k].weight / totalWeight;

                        value = Mathf.Lerp(value, expressions[k].blendShapeInfo[i].blendShapeWeights[j], lWeight);
                    }
                    skinnedRenderers[i].SetBlendShapeWeight(j, value);
                }
            }

            for (int i = 0; i < otherTransforms.Length; i++)
            {
                Vector3 pos = defaultExpression.localPositions[i];
                Vector3 scale = defaultExpression.localScale[i];
                Quaternion rot = Quaternion.Euler(defaultExpression.localRotations[i]);
                if (!assignDefault)
                {
                    pos = otherTransforms[i].localPosition;
                    scale = otherTransforms[i].localScale;
                    rot = Quaternion.Euler(otherTransforms[i].localEulerAngles);
                }

                for (int j = 0; j < expressions.Count; j++)
                {
                    if (Mathf.Approximately(0f, expressions[j].weight))
                        continue;

                    float lWeight = expressions[j].weight / totalWeight;

                    pos = Vector3.Lerp(pos, expressions[j].localPositions[i], lWeight);
                    rot = Quaternion.Slerp(rot, Quaternion.Euler(expressions[j].localRotations[i]), lWeight);
                    scale = Vector3.Lerp(scale, expressions[j].localScale[i], lWeight);
                }

                otherTransforms[i].localRotation = rot;
                otherTransforms[i].localPosition = pos;
                otherTransforms[i].localScale = scale;
            }
        }

        private Expression DefineNewExpression()
        {
            var newExpression = new Expression();
            newExpression.blendShapeInfo = new ExpressionBlendShapeInfo[skinnedRenderers.Length];
            for (int i = 0; i < skinnedRenderers.Length; i++)
            {
                newExpression.blendShapeInfo[i] = new ExpressionBlendShapeInfo();
                newExpression.blendShapeInfo[i].blendShapeWeights = new float[skinnedRenderers[i].sharedMesh.blendShapeCount];
                for (int j = 0; j < newExpression.blendShapeInfo[i].blendShapeWeights.Length; j++)
                {
                    newExpression.blendShapeInfo[i].blendShapeWeights[j] =
                        Mathf.Clamp(skinnedRenderers[i].GetBlendShapeWeight(j), 0f, 100f);
                }
            }

            newExpression.localPositions = new Vector3[otherTransforms.Length];
            newExpression.localRotations = new Vector3[otherTransforms.Length];
            newExpression.localScale = new Vector3[otherTransforms.Length];

            for (int i = 0; i < otherTransforms.Length; i++)
            {
                newExpression.localPositions[i] = otherTransforms[i].localPosition;
                newExpression.localRotations[i] = otherTransforms[i].localEulerAngles;
                newExpression.localScale[i] = otherTransforms[i].localScale;
            }

            return newExpression;
        }

        [ContextMenu("Reset To Initial")]
        public void ResetToInitial()
        {
            SnapToExpression(0);
        }

        private void SnapToExpression(int index)
        {
            for (int i = 0; i < expressions.Count; i++)
            {
                expressions[i].weight = i == index ? 1f : 0f;
            }
            totalWeight = 1f;

            UpdateExpression(true);
        }

        [ContextMenu("Setup")]
        public void CreateNewExpression()
        {
            Expression exp = DefineNewExpression();
            exp.expressionName = "New Expression";
            expressions.Add(exp);
        }
    }
}