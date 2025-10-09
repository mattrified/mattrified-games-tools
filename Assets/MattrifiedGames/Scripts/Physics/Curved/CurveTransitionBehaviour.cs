using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTransitionBehaviour : MonoBehaviour
{
    [SerializeField()]
    InGameCurve curve;

    [SerializeField()]
    float xPosition;

    [SerializeField()]
    float time;

    public void TransitionBehaviour(GameObjectScriptableValue gosv)
    {
        CurvedMattrifiedPhysicsBehaviour cmpb = gosv.Value.GetComponent<CurvedMattrifiedPhysicsBehaviour>();
        if (cmpb == null)
        {
            Debug.LogWarning("Curve Physics component missing.");
        }
        else
        {
            TransitionBehaviour(cmpb);
        }
    }

    public void TransitionBehaviour(CurvedMattrifiedPhysicsBehaviour behaviour)
    {
        behaviour.TransitionToCurve(curve, xPosition, time);
    }

    private void OnDrawGizmosSelected()
    {
        if (curve == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);

        Gizmos.color = Color.yellow;
        Vector3 pos = Vector3.zero;
        curve.GetFloorPoint(xPosition, ref pos);
        Gizmos.DrawLine(transform.position, pos);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos, Vector3.one);
    }
}
