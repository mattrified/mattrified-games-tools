using UnityEngine;
using System.Collections;

/// <summary>
/// abstract class for custom physics.
/// </summary>
public abstract class MattrifiedPhysicsBehaviourBase : MonoBehaviour
{
    public const float DELTA_TIME = 1f / 60f;

    /// <summary>
    /// The transform controlled by the physics.
    /// </summary>
    protected Transform m_Transform;

    [SerializeField(), Tooltip("The current position of the object in the physics simulation")]
    protected Vector3 position;

    protected virtual void Awake()
    {
        m_Transform = transform;
    }

    protected virtual void FixedUpdate()
    {
        EvaluatePosition();
    }

    protected abstract void EvaluatePosition();

    protected virtual void Update()
    {
        m_Transform.position = position;
    }
}