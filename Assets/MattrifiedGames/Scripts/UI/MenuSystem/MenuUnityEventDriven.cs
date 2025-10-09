using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuUnityEventDriven : MenuElementBase
{
    public ConditionalUnityBoolEvent OnLeftHoldEvent;
    public ConditionalUnityEvent OnLeftPressedEvent;

    public ConditionalUnityBoolEvent OnRightHoldEvent;
    public ConditionalUnityEvent OnRightPressedEvent;

    public ConditionalUnityBoolEvent OnUpHoldEvent;
    public ConditionalUnityEvent OnUpPressedEvent;

    public ConditionalUnityBoolEvent OnDownHoldEvent;
    public ConditionalUnityEvent OnDownPressedEvent;

    public ConditionalUnityEvent OnConfirmEvent;
    public ConditionalUnityEvent OnCancelEvent;

    public ConditionalUnityIntEvent OnButtonPressEvent;

    public ConditionalUnityEvent OnHighlightEvent;
    public ConditionalUnityEvent OnUnhighlightEvent;

    public override void OnHighlight()
    {
        OnHighlightEvent.value.Invoke();
        base.OnHighlight();
    }

    public override void OnUnhighlight()
    {
        OnUnhighlightEvent.value.Invoke();
        base.OnUnhighlight();
    }

    public override void OnCancelPressed()
    {
        OnCancelEvent.value.Invoke();
        base.OnCancelPressed();
    }

    public override void OnConfirmPressed()
    {
        OnConfirmEvent.value.Invoke();
        base.OnConfirmPressed();
    }

    public override void OnInputPressed(int index)
    {
        OnButtonPressEvent.value.Invoke(index);
        base.OnInputPressed(index);
    }

    public override void OnLeftHold(bool holding)
    {
        OnLeftHoldEvent.value.Invoke(holding);
        base.OnLeftHold(holding);
    }

    public override void OnLeftPressed()
    {
        OnLeftPressedEvent.value.Invoke();
        base.OnLeftPressed();
    }

    public override void OnRightHold(bool holding)
    {
        OnRightHoldEvent.value.Invoke(holding);
        base.OnRightHold(holding);
    }

    public override void OnRightPressed()
    {
        OnRightPressedEvent.value.Invoke();
        base.OnRightPressed();
    }

    public override void OnDownHold(bool holding)
    {
        OnDownHoldEvent.value.Invoke(holding);
        base.OnDownHold(holding);
    }

    public override void OnDownPressed()
    {
        OnDownPressedEvent.value.Invoke();
        base.OnDownPressed();
    }

    public override void OnUpHold(bool holding)
    {
        OnUpHoldEvent.value.Invoke(holding);
        base.OnUpHold(holding);
    }

    public override void OnUpPressed()
    {
        OnUpPressedEvent.value.Invoke();
        base.OnUpPressed();
    }
}

[System.Serializable()]
public class ConditionalUnityBoolEvent : ConditionalObject<UnityBoolEvent>{}

[System.Serializable()]
public class ConditionalUnityEvent : ConditionalObject<UnityEvent> { }

[System.Serializable()]
public class ConditionalUnityIntEvent : ConditionalObject<UnityIntEvent> { }
