using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurvedMattrifiedPhysicsBehaviour : MattrifiedPhysicsBehaviour
{
    [Tooltip("The curve used in the game.  If one is not assigned, the behaviour will disable.")]
    public InGameCurveBase currentCurve;

    [SerializeField(), Tooltip("The list of curves kept by the game.  Faster search than finding every curve in the scene when we want to assign one.")]
    InGameCurveList curveList;

    [SerializeField(), Tooltip("The position of the object on the curve in world space.")]
    Vector3 curvePos;

    [SerializeField(), Tooltip("The floor position of the object on the curve in world space.")]
    Vector3 curveFloorPos;

    [SerializeField(), Tooltip("The ceiling position of the object on the curve in world space.")]
    Vector3 curveCeilingPos;

    /// <summary>
    /// The y euler angle of the object
    /// </summary>
    float eulerY;

    /// <summary>
    /// The vector3 euler whose y is updated and assigned to the transform.
    /// </summary>
    Vector3 euler;

    float eulerBase;

    /// <summary>
    /// Offset values for the euler positioning.
    /// </summary>
    public float eulerOffset;

    /// <summary>
    /// The goal the euleroffset is moving towards
    /// </summary>
    public float eulerOffsetGoal;

    /// <summary>
    /// The maximum delta the offset can move.
    /// </summary>
    public float eulerOffsetDeltaMax = 180f / 8f;

    /// <summary>
    /// The factor in which the transition percent is decreased.
    /// </summary>
    public float transitionTime;

    /// <summary>
    /// The starting position the character is in during a curve transition
    /// </summary>
    public Vector3 transitionPosition;

    /// <summary>
    /// The starting position euler y of a character during a curve transition
    /// </summary>
    public float transitionEulerY;

    [Range(0, 1), Tooltip("The rate at which a transition is occuring.  0 is the transition is not active, 1 is the transition is active.")]
    public float transitionPercent = 0f;

    /// <summary>
    /// Is a transition active?
    /// </summary>
    public bool transitioning;

    /// <summary>
    /// The radius of the character
    /// </summary>
    public float radius;

    /// <summary>
    /// Event triggered when a curve is left.
    /// </summary>
    public UnityInGameCurveEvent OnCurveLeave;
     
    /// <summary>
    /// Event triggered when a curve is entered.
    /// </summary>
    public UnityInGameCurveEvent OnCurveEnter;

    [SerializeField(), Header("Saving and Loading Path Positions")]
    bool saveLoadPosition;

    [SerializeField()]
    FloatScriptableValue startingValue = null;

    [SerializeField()]
    IntScriptableValue startingPathIndex = null;

    

    public float PositionX
    {
        get
        {
            return position.x;
        }
    }

    protected override void Awake()
    {
        m_Transform = transform;

        if (saveLoadPosition)
        {
            AttemptToLoadPath();
        }
        else if (currentCurve == null)
        {
            enabled = false;
            return;
        }

        currentCurve.GetAllFloorCeilAngleY(position.x, ref curveFloorPos, ref curveCeilingPos, ref eulerBase);

        curvePos = curveFloorPos;
        position.y = curvePos.y;

        eulerY = eulerBase + eulerOffset;

        transitionPosition = curveFloorPos;
        transitionEulerY = eulerY;

        OnCurveEnter.Invoke(currentCurve);
    }

    private void AttemptToLoadPath()
    {
        if (!curveList.FindCurveByID(startingPathIndex.Value, out currentCurve))
        {
            FindCurveSlow();
        }

        SnapToFloat(startingValue.Value);
    }

    private void OnDestroy()
    {
    }

    /// <summary>
    /// Attempts to save the curve and position information.
    /// </summary>
    public void SaveCurveAndPosition()
    {
        if (saveLoadPosition)
        {
            startingPathIndex.Value = currentCurve.curveID;
            startingValue.Value = position.x;
        }
    }

    public void SnapToCurve(InGameCurve curve)
    {
        OnCurveLeave.Invoke(currentCurve);

        currentCurve = curve;

        OnCurveEnter.Invoke(currentCurve);
    }

    public void SnapToCurve(int curveIndex)
    {
        OnCurveLeave.Invoke(currentCurve);

        if (!curveList.FindCurveByID(curveIndex, out currentCurve))
        {
            FindCurveSlow();
        }

        OnCurveEnter.Invoke(currentCurve);
    }

    public void SnapToFloat(float value)
    {
        position.x = Mathf.Clamp(value, radius, currentCurve.distance - radius);
        Update();
    }

    private void FindCurveSlow()
    {
        InGameCurve[] curves = FindObjectsOfType<InGameCurve>();
        for (int i = 0, iLen = curves.Length; i < iLen; i++)
        {
            if (curves[i].curveID == startingPathIndex.Value)
            {
                currentCurve = curves[i];
                break;
            }
        }

        if (currentCurve == null)
        {
            currentCurve = curves[0];
        }
    }

    public void FocusOnCurve(InGameCurveBase curve)
    {
        curve.FocusOnCurve();
    }

    public void UnfocusOnCurve(InGameCurveBase curve)
    {
        curve.UnfocusOnCurve();
    }

    protected override void Update()
    {
        m_Transform.position = Vector3.Lerp(curvePos, transitionPosition, transitionPercent);

        euler.y = Mathf.LerpAngle(eulerY, transitionEulerY, transitionPercent);
        m_Transform.eulerAngles = euler;
    }

    protected override void FixedUpdate()
    {
        if (transitioning)
        {
            transitionPercent -= 1f / 60f;
            if (transitionPercent <= 0)
            {
                transitioning = false;
                
            }
        }

        position += speed;

        speed.x = Mathf.MoveTowards(speed.x, xSpeedGoal, xFriction);
        speed.z = Mathf.MoveTowards(speed.z, zSpeedGoal, zFriction);

        speed.y += gravity;

        EvaluatePosition();
    }

    /// <summary>
    /// Transitions from the current curve to a new one.
    /// </summary>
    /// <param name="curve">The new curve</param>
    /// <param name="newPosition">The new position</param>
    /// <param name="time">The number of frames the transition should take</param>
    public void TransitionToCurve(InGameCurve curve, float newPosition, float time = 60f)
    {
        transitionPosition = transform.position;

        transitionEulerY = eulerY;

        transitionPercent = 1f;

        transitionTime = 1f / time;

        OnCurveLeave.Invoke(currentCurve);

        currentCurve = curve;

        SetSpeedX(0);
        position.x = newPosition;

        transitioning = true;

        if (saveLoadPosition)
        {
            startingPathIndex.Value = currentCurve.curveID;
            startingValue.Value = position.x;
        }

        OnCurveEnter.Invoke(currentCurve);
    }

    private void OnDrawGizmosSelected()
    {
        if (currentCurve != null)
        {
            Vector3 pos = position;
            currentCurve.GetFloorPoint(position.x, ref pos);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(pos, radius);
        }
    }

    protected override void EvaluatePosition()
    {
        if (!currentCurve.Loop)
        {
            position.x = Mathf.Clamp(position.x, radius, currentCurve.distance - radius);
        }

        currentCurve.GetAllFloorCeilAngleY(position.x, ref curveFloorPos, ref curveCeilingPos, ref eulerBase);

        curvePos.x = curveFloorPos.x;
        curvePos.z = curveFloorPos.z;

        if (position.y <= curveFloorPos.y)
        {
            if (speed.y < 0)
            {
                onLandEvent.Invoke(this);
                speed.y = 0;
                gravity = 0;
            }
            position.y = curveFloorPos.y;
        }
        else if (Mathf.Approximately(0f, speed.y))
        {
            position.y = curveFloorPos.y;
        }

        curvePos.y = position.y;

        eulerOffset = Mathf.MoveTowards(eulerOffset, eulerOffsetGoal, eulerOffsetDeltaMax);
        eulerY = eulerBase + eulerOffset;
    }
}