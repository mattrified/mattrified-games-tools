#if USING_INCONTROL
using UnityEngine;
using System.Collections;
using MattrifiedGames.SVData;
using UnityEngine.Events;

public class MenuSystemIncontrol : MenuSystemBase
{
    [SerializeField()]
    protected InControlDefinedPlayerActionSetScriptableValue playerInputSV;

    public BoolScriptableValue P1DisconnectActiveBSV;
    public BoolScriptableValue P2DisconnectActiveBSV;

    public int movementIndex = 0;

    public int confirmIndex;
    public int cancelIndex;

    Vector2 lastMovement;
    Vector2 movement;

    [SerializeField()]
    float tolerance = 0.25f;

    float negTolerance;

    public UnityEvent OnAnyPress;

    private void Awake()
    {
        negTolerance = -tolerance;

        if (playerInputSV.Value == null)
        {
            Debug.LogWarning("No input has been assigned.  This menu will not be active.");
            this.Locked = true;
            //gameObject.SetActive(false);
        }

        // We hide the cursor if not in windowed mode.
        Cursor.visible = Screen.fullScreenMode == FullScreenMode.Windowed;
    }

    public override bool TestAllLocks
    {
        get
        {
            return P1DisconnectActiveBSV.Value || P2DisconnectActiveBSV.Value || base.TestAllLocks;
        }
    }

    public override void Update()
    {
        if (P1DisconnectActiveBSV.Value || P2DisconnectActiveBSV.Value)
        {
            lockTicks = 10;
            return;
        }

        base.Update();
    }

    public void AssignInput(InControlDefinedPlayerActionSetScriptableValue input)
    {
        playerInputSV = input;
    }

    public bool TestInput(int index)
    {
        if (playerInputSV.Value != null)
        {
            return playerInputSV.Value.GetAction(index).WasPressed;
        }
        return false;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        currentPanel = null;
        panelQueue.Clear();
    }

    public override void CheckInput()
    {
        var input = playerInputSV.Value;
        if (input == null || Locked || lockTicks > 0)
            return;

        movement = input.GetTwoAxisActionValue(movementIndex);
        lastMovement = input.GetTwoAxisActionLastValue(movementIndex);

        if (movement.x < negTolerance)
        {
            currentPanel.OnLeftHold(leftHoldTest.Test(true));
            currentPanel.OnRightHold(rightHoldTest.Test(false));
            if (lastMovement.x > negTolerance)
            {
                currentPanel.OnLeftPressed();
            }
            OnAnyPress.Invoke();
        }
        else if (movement.x > tolerance)
        {
            currentPanel.OnRightHold(rightHoldTest.Test(true));
            currentPanel.OnLeftHold(leftHoldTest.Test(false));
            if (lastMovement.x < tolerance)
            {
                currentPanel.OnRightPressed();
            }
            OnAnyPress.Invoke();
        }
        else
        {
            currentPanel.OnLeftHold(leftHoldTest.Test(false));
            currentPanel.OnRightHold(rightHoldTest.Test(false));
        }

        if (movement.y < negTolerance)
        {
            currentPanel.OnDownHold(downHoldTest.Test(true));
            currentPanel.OnUpHold(upHoldTest.Test(false));
            if (lastMovement.y > negTolerance)
            {
                currentPanel.OnDownPressed();
            }
            OnAnyPress.Invoke();
        }
        else if (movement.y > tolerance)
        {
            currentPanel.OnUpHold(upHoldTest.Test(true));
            currentPanel.OnDownHold(downHoldTest.Test(false));
            if (lastMovement.y < tolerance)
            {
                currentPanel.OnUpPressed();
            }
            OnAnyPress.Invoke();
        }
        else
        {
            currentPanel.OnDownHold(downHoldTest.Test(false));
            currentPanel.OnUpHold(upHoldTest.Test(false));
        }

        for (int i = 0, len = input.ActionCount; i < len;  i++)
        {
            var action = input.GetAction(i);
            if (action.WasPressed)
            {
                currentPanel.OnInputPressed(i);

                if (i == confirmIndex)
                    currentPanel.OnConfirmPressed();

                if (i == cancelIndex)
                    currentPanel.OnCancelPressed();

                OnAnyPress.Invoke();
            }
        }
    }
}
#endif