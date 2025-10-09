using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCurveMovementBehaviour : MonoBehaviour
{
    public AnimationCurve curve;
    public Vector3 startPos;
    public Vector3 endPos;
    public float timeOffset;

    [ContextMenu("Set Start From Pos")]
    public void SetStartFromPos()
    {
        startPos = transform.localPosition;
    }

    [ContextMenu("Set End From Pos")]
    public void SetEndFromPos()
    {
        endPos = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, curve.Evaluate(Time.timeSinceLevelLoad + timeOffset));
    }
}
