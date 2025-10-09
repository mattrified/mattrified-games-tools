using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class ExpressionHelper : MonoBehaviour
{
    [Header("Jaw Rotation")]
    public Transform jawRootT;
    public bool jawAssigned;
    public Vector3 defaultJawAngle;

    [Range(0, 1)]
    public float jawWeight;

    public Vector3 jawMinAngle;
    public Vector3 jawMaxAngle;

    [Range(-1f, 1f)]
    public float jawOpenClose = 0f;

    [Range(-1f, 1f)]
    public float jawLeftRight = 0f;

    [Range(-1f, 1f)]
    public float jawTwist = 0f;

    [Range(0f,1f)]
    public float eyeWeightTotal = 0f;

    [Header("Left Eye Region")]
    public Transform leftEyeT;
    public bool leftEyeAssigned;
    public Vector3 defaultLeftEyePos;

    public Vector3 leftEyeOffset;

    [Range(0f, 1f)]
    public float leftEyeOffsetWeight;

    public float leftEyeScale = 1f;

    [Range(0, 1)]
    public float leftEyeTotalWeight;

    [Header("Right Eye Region")]
    public Transform rightEyeT;
    public bool rightEyeAssigned;
    public Vector3 defaultRightEyePos;

    public Vector3 rightEyeOffset;

    [Range(0f, 1f)]
    public float rightEyeOffsetWeight;

    public float rightEyeScale = 1f;

    [Range(0, 1)]
    public float rightEyeTotalWeight;

    private void OnValidate()
    {
        if (!jawAssigned && jawRootT != null)
        {
            defaultJawAngle = jawRootT.localEulerAngles;
            jawAssigned = true;
        }

        if (!leftEyeAssigned && leftEyeT != null)
        {
            defaultLeftEyePos = leftEyeT.localPosition;
            leftEyeAssigned = true;
        }

        if (!rightEyeAssigned && rightEyeT != null)
        {
            defaultRightEyePos = rightEyeT.localPosition;
            rightEyeAssigned = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (jawAssigned)
        {
            Vector3 defaultEuler = defaultJawAngle;

            defaultEuler.x = Mathf.Lerp(jawMinAngle.x, defaultEuler.x, jawLeftRight + 1f);
            defaultEuler.x = Mathf.Lerp(defaultEuler.x, jawMaxAngle.x, jawLeftRight);

            defaultEuler.y = Mathf.Lerp(jawMinAngle.y, defaultEuler.y, jawTwist + 1f);
            defaultEuler.y = Mathf.Lerp(defaultEuler.y, jawMaxAngle.y, jawTwist);

            defaultEuler.z = Mathf.Lerp(jawMinAngle.z, defaultEuler.z, jawOpenClose + 1f);
            defaultEuler.z = Mathf.Lerp(defaultEuler.z, jawMaxAngle.z, jawOpenClose);


            jawRootT.localRotation = Quaternion.Slerp(jawRootT.localRotation, Quaternion.Euler(defaultEuler), jawWeight);
        }

        if (leftEyeAssigned)
        {
            leftEyeT.localPosition = Vector3.Lerp(defaultLeftEyePos, defaultLeftEyePos + leftEyeOffset, leftEyeTotalWeight * leftEyeOffsetWeight * eyeWeightTotal);
            leftEyeT.localScale = Vector3.one * Mathf.Lerp(1f, leftEyeScale, leftEyeTotalWeight * eyeWeightTotal);
        }

        if (rightEyeAssigned)
        {
            rightEyeT.localPosition = Vector3.Lerp(defaultRightEyePos, defaultRightEyePos + rightEyeOffset, rightEyeTotalWeight * rightEyeOffsetWeight * eyeWeightTotal);
            rightEyeT.localScale = Vector3.one * Mathf.Lerp(1f, rightEyeScale, rightEyeTotalWeight * eyeWeightTotal);
        }
    }
}
