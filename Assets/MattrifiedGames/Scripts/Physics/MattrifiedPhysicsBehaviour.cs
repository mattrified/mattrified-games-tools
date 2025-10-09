using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MattrifiedPhysicsBehaviour : MattrifiedPhysicsBehaviourBase
{
    [SerializeField()]
    protected Vector3 speed;

    [SerializeField()]
    protected float gravity;

    [SerializeField()]
    protected float xSpeedGoal, zSpeedGoal;

    [SerializeField()]
    protected float xFriction, zFriction;

    public UnityMattrifiedPhysicsBehaviourEvent onLandEvent;

    public virtual void Jump(float jumpHeight, float jumpTime)
    {
        gravity = (-2f * jumpHeight) / (jumpTime * jumpTime * 0.25f);

        speed.y = Mathf.Sqrt(2 * -gravity * jumpHeight) + gravity * 0.5f;
    }

    public void PushOnZ(float distance, float time)
    {
        speed.z = 2f * distance / time;
        zFriction = Mathf.Abs(-speed.z / (time - 1f));
    }

    public void PushOnX(float distance, float time)
    {
        speed.x = 2f * distance / time;
        xFriction = Mathf.Abs(-speed.x / (time - 1f));
    }

    public void SetSpeedXBasedOnDistance(float distance, float time = 60f, float friction = 10)
    {
        xSpeedGoal = distance / time;
        xFriction = friction;
    }

    public void SetSpeedZBasedOnDistance(float distance, float time = 60f, float friction = 10)
    {
        zSpeedGoal = distance / time;
        zFriction = friction;
    }

    public void SetSpeedZ(float value, float friction = 10)
    {
        zSpeedGoal = value;
        zFriction = friction;
    }

    public void SetSpeedX(float value, float friction = 10)
    {
        xSpeedGoal = value;
        xFriction = friction;
    }

    public void SetSpeed(float speed)
    {
        SetSpeedX(speed);
    }

    protected override void FixedUpdate()
    {
        position += m_Transform.rotation * speed;

        speed.x = Mathf.MoveTowards(speed.x, xSpeedGoal, xFriction);
        speed.z = Mathf.MoveTowards(speed.z, zSpeedGoal, zFriction);

        speed.y += gravity;

        EvaluatePosition();
    }

    protected override void EvaluatePosition()
    {
        if (speed.y < 0 && position.y <= 0)
        {
            onLandEvent.Invoke(this);
            speed.y = 0;
            gravity = 0;
            position.y = 0;
        }
    }
}

[System.Serializable()]
public class UnityMattrifiedPhysicsBehaviourEvent : UnityEvent<MattrifiedPhysicsBehaviour> { }