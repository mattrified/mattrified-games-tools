using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HoldAndRepeatBehaviour : MonoBehaviour
{
    [SerializeField()]
    HoldAndRepeatTest holdAndRepeatTest;

    [SerializeField()]
    UnityEvent OnHoldTestSuccessEvent;

    public bool Holding
    {
        get;
        set;
    }

    private void OnDisable()
    {
        Holding = false;
        holdAndRepeatTest.Test(false);
    }

    private void Update()
    {
        if (holdAndRepeatTest.Test(Holding))
            OnHoldTestSuccessEvent.Invoke();
    }
}