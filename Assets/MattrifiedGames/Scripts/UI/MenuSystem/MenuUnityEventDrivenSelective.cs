using MattrifiedGames.SVData;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    [Tooltip("Sets menu element actions by defining a list of different element types")]
    public class MenuUnityEventDrivenSelective : MenuElementBase
    {
        [System.Serializable()]
        public class MenuEventType
        {
            public enum EventType
            {
                NA = 0,

                Highlight = 1,
                Unhighlight = 2,
                Cancel = 3,
                Confirm = 4,
                Input = 5,

                LeftHold = 6,
                LeftPress = 7,

                RightHold = 8,
                RightPress = 9,

                UpHold = 10,
                UpPress = 11,

                DownHold = 12,
                DownPress = 13,
            }

            public EventType eventType;
            public UnityIntEvent intEvent;
        }

        [Tooltip("The list of event types in this Menu Element.")]
        public List<MenuEventType> events;


        private void OnDestroy()
        {
            for (int i = 0; i < events.Count; i++)
            {
                events[i].intEvent.RemoveAllListeners();
                events[i].intEvent = null;
                events[i] = null;
            }

            events.Clear();
            events = null;
        }

        public void FindEvent(MenuEventType.EventType type, int value)
        {
            var result = events.Find(x => x.eventType == type);
            if (result == null)
                return;

            result.intEvent.Invoke(value);
        }

        public void AddEventMethod(MenuEventType.EventType type, UnityAction<int> intAction)
        {
            var result = events.Find(x => x.eventType == type);
            if (result == null)
            {
                MenuEventType newEvent = new MenuEventType();
                newEvent.eventType = type;
                newEvent.intEvent = new UnityIntEvent();
                newEvent.intEvent.AddListener(intAction);
                events.Add(newEvent);
            }
            else
            {
                result.intEvent.AddListener(intAction);
            }
        }

        public override void OnHighlight()
        {
            FindEvent(MenuEventType.EventType.Highlight, 1);
            base.OnHighlight();
        }

        public override void OnUnhighlight()
        {
            FindEvent(MenuEventType.EventType.Unhighlight, 1);
            base.OnUnhighlight();
        }

        public override void OnCancelPressed()
        {
            FindEvent(MenuEventType.EventType.Cancel, 1);
            base.OnCancelPressed();
        }

        public override void OnConfirmPressed()
        {
            FindEvent(MenuEventType.EventType.Confirm, 1);
            base.OnConfirmPressed();
        }

        public override void OnInputPressed(int index)
        {
            FindEvent(MenuEventType.EventType.Input, index);
            base.OnInputPressed(index);
        }

        public override void OnLeftHold(bool holding)
        {
            if (!holding)
                return;

            FindEvent(MenuEventType.EventType.LeftHold, holding ? 1 : 0);
            base.OnLeftHold(holding);
        }

        public override void OnLeftPressed()
        {
            FindEvent(MenuEventType.EventType.LeftPress, 1);
            base.OnLeftPressed();
        }

        public override void OnRightHold(bool holding)
        {
            if (!holding)
                return;

            FindEvent(MenuEventType.EventType.RightHold, holding ? 1 : 0);
            base.OnRightHold(holding);
        }

        public override void OnRightPressed()
        {
            FindEvent(MenuEventType.EventType.RightPress, 1);
            base.OnRightPressed();
        }

        public override void OnDownHold(bool holding)
        {
            if (!holding)
                return;

            FindEvent(MenuEventType.EventType.DownHold, holding ? 1 : 0);
            base.OnDownHold(holding);
        }

        public override void OnDownPressed()
        {
            FindEvent(MenuEventType.EventType.DownPress, 1);
            base.OnDownPressed();
        }

        public override void OnUpHold(bool holding)
        {
            if (!holding)
                return;

            FindEvent(MenuEventType.EventType.UpHold, holding ? 1 : 0);
            base.OnUpHold(holding);
        }

        public override void OnUpPressed()
        {
            FindEvent(MenuEventType.EventType.UpPress, 1);
            base.OnUpPressed();
        }
    }
}