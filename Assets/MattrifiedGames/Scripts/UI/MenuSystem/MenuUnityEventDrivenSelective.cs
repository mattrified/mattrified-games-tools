using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuUnityEventDrivenSelective : MenuElementBase
{
    public enum MenuEventType
    {
        OnLeftHoldEvent,
        OnLeftPressedEvent,

        OnRightHoldEvent,
        OnRightPressedEvent,

        OnUpHoldEvent,
        OnUpPressedEvent,

        OnDownHoldEvent,
        OnDownPressedEvent,

        OnConfirmEvent,
        OnCancelEvent,

        OnButtonPressEvent,

        OnHighlightEvent,
        OnUnhighlightEvent,
    }

    [System.Serializable()]
    public class UnityEventPair
    {
        public MenuEventType eventType;
        public UnityIntEvent @event;
    }

    public List<UnityEventPair> events;

public MenuUnityEventDrivenSelective elementToCopy;

    protected virtual void OnValidate()
    {
        if (elementToCopy != null)
        {
            events = new List<UnityEventPair>();

            for (int i = 0; i < elementToCopy.events.Count; i++)
            {
                var s = JsonUtility.ToJson(elementToCopy.events[i]);
                events.Add(JsonUtility.FromJson<UnityEventPair>(s));
                s = null;
            }

            elementToCopy = null;
        }
    }

    public override void OnHighlight()
    {
        TriggerEvent(MenuEventType.OnHighlightEvent, 0);
        base.OnHighlight();
    }

    public override void OnUnhighlight()
    {
        TriggerEvent(MenuEventType.OnUnhighlightEvent, 0);
        base.OnUnhighlight();
    }

    public override void OnCancelPressed()
    {
        TriggerEvent(MenuEventType.OnCancelEvent, 0);
        base.OnCancelPressed();
    }

    public override void OnConfirmPressed()
    {
        TriggerEvent(MenuEventType.OnConfirmEvent, 0);
        base.OnConfirmPressed();
    }

    public override void OnInputPressed(int index)
    {
        TriggerEvent(MenuEventType.OnButtonPressEvent, index);
        base.OnInputPressed(index);
    }

    public override void OnLeftHold(bool holding)
    {
        TriggerEvent(MenuEventType.OnLeftHoldEvent, holding ? 1 : 0);
        base.OnLeftHold(holding);
    }

    public override void OnLeftPressed()
    {
        TriggerEvent(MenuEventType.OnLeftPressedEvent, 1);
        base.OnLeftPressed();
    }

    public override void OnRightHold(bool holding)
    {
        TriggerEvent(MenuEventType.OnRightHoldEvent, holding ? 1 : 0);
        base.OnRightHold(holding);
    }

    public override void OnRightPressed()
    {
        TriggerEvent(MenuEventType.OnRightPressedEvent, 1);
        base.OnRightPressed();
    }

    public override void OnDownHold(bool holding)
    {
        TriggerEvent(MenuEventType.OnDownHoldEvent, holding ? 1 : 0);
        base.OnDownHold(holding);
    }

    public override void OnDownPressed()
    {
        TriggerEvent(MenuEventType.OnDownPressedEvent, 1);
        base.OnDownPressed();
    }

    public override void OnUpHold(bool holding)
    {
        TriggerEvent(MenuEventType.OnUpHoldEvent, holding ? 1 : 0);
        base.OnUpHold(holding);
    }

    public override void OnUpPressed()
    {
        TriggerEvent(MenuEventType.OnUpPressedEvent, 1);
        base.OnUpPressed();
    }

    protected virtual void TriggerEvent(MenuEventType eType, int value)
    {
        var result = events.Find(x => x.eventType == eType);
        if (result != null)
            result.@event.Invoke(value);
    }

    private void OnDestroy()
    {
        for (int i = 0; i < events.Count; i++)
        {
            events[i].@event.RemoveAllListeners();
            events[i].@event = null;
        }
        events.Clear();
        events = null;
    }

    public void AddEvent(MenuEventType eventType, UnityAction<int> intEvent)
    {
        var result = events.Find(x => x.eventType == eventType);
        if (result == null)
        {
            result = new UnityEventPair() { eventType = eventType, @event = new UnityIntEvent() };
            events.Add(result);
        }
        result.@event.AddListener(intEvent);
    }
}
