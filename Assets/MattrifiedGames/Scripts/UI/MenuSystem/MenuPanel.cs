using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.MenuSystem
{
    [Tooltip("A single panel in the MenuSystem.")]
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField(), Tooltip("The menu element currently active.")]
        protected MenuElementBase activeElement;

        [Tooltip("Event invoked when the panel is opened.")]
        public UnityEvent OnOpenEvent;

        [Tooltip("Event invoked when the panel is closed.")]
        public UnityEvent OnCloseEvent;

        [Tooltip("Event invoked when confirmed is pressed with this panel open.  Can be used for simple panels.")]
        public UnityEvent OnPanelConfirmEvent;

        [Tooltip("Event invoked when cancel is pressed with this panel open.")]
        public UnityEvent OnPanelCancelEvent;

        [Tooltip("If true, holding a direction will be treated as a press.")]
        public bool treatHoldingAsPress;

        [Tooltip("If true, the panel will be considered locked and not respond to input.")]
        protected bool _locked;
        public bool Locked
        {
            get => _locked;
            set => _locked = value;
        }

        protected virtual void OnEnable()
        {
            activeElement.OnHighlight();
        }

        public MenuElementBase ActiveElement { get { return activeElement; } }

        public virtual bool UpdateFromBase(MenuSystemBase msBase)
        {
            bool result = activeElement.UpdateFromPanel(msBase, this);
            return result;
        }

        public virtual void OnLeftHold(bool holding)
        {
            if (!treatHoldingAsPress)
            {
                activeElement.OnLeftHold(holding);
            }
            else if (holding)
            {
                if (activeElement.leftSibling == null)
                {
                    activeElement.OnLeftPressed();
                }
                else
                {
                    SelectNewElement(activeElement.leftSibling);
                    if (activeElement.Skip)
                        OnLeftPressed();
                }
            }
        }

        public virtual void OnCancelPressed()
        {
            activeElement.OnCancelPressed();
            OnPanelCancelEvent.Invoke();
        }

        public virtual void OnDownHold(bool holding)
        {
            if (!treatHoldingAsPress)
            {
                activeElement.OnDownHold(holding);
            }
            else if (holding)
            {
                if (activeElement.downSibling == null)
                {
                    activeElement.OnDownPressed();
                }
                else
                {
                    SelectNewElement(activeElement.downSibling);
                    if (activeElement.Skip)
                        OnDownPressed();
                }
            }
        }

        public virtual void OnConfirmPressed()
        {
            activeElement.OnConfirmPressed();
            OnPanelConfirmEvent.Invoke();
        }

        public virtual void OnDownPressed()
        {
            if (treatHoldingAsPress)
                return;

            if (activeElement.downSibling == null)
            {
                activeElement.OnDownPressed();
            }
            else
            {
                SelectNewElement(activeElement.downSibling);

                if (activeElement.Skip)
                    OnDownPressed();
            }
        }

        public virtual void Open(MenuSystemBase menu)
        {
            OnOpenEvent.Invoke();
        }

        public virtual void Close(MenuSystemBase menu)
        {
            OnCloseEvent.Invoke();
        }

        public virtual void OnInputPressed(int index)
        {
            activeElement.OnInputPressed(index);
        }

        public virtual void OnLeftPressed()
        {
            if (treatHoldingAsPress)
                return;

            if (activeElement.leftSibling == null)
            {
                activeElement.OnLeftPressed();
            }
            else
            {
                SelectNewElement(activeElement.leftSibling);
                if (activeElement.Skip)
                    OnLeftPressed();
            }
        }

        public virtual void OnRightHold(bool holding)
        {
            if (!treatHoldingAsPress)
            {
                activeElement.OnRightHold(holding);
            }
            else if (holding)
            {
                if (activeElement.rightSibling == null)
                {
                    activeElement.OnRightPressed();
                }
                else
                {
                    SelectNewElement(activeElement.rightSibling);
                    if (activeElement.Skip)
                        OnRightPressed();
                }
            }
        }

        public virtual void OnRightPressed()
        {
            if (treatHoldingAsPress)
                return;

            if (activeElement.rightSibling == null)
            {
                activeElement.OnRightPressed();
            }
            else
            {
                SelectNewElement(activeElement.rightSibling);
                if (activeElement.Skip)
                    OnRightPressed();
            }
        }

        public virtual void OnUpHold(bool holding)
        {
            if (!treatHoldingAsPress)
            {
                activeElement.OnUpHold(holding);
            }
            else if (holding)
            {
                if (activeElement.upSibling == null)
                {
                    activeElement.OnUpPressed();
                }
                else
                {
                    SelectNewElement(activeElement.upSibling);
                    if (activeElement.Skip)
                        OnUpPressed();
                }
            }
        }

        public virtual void OnUpPressed()
        {
            if (treatHoldingAsPress)
                return;

            if (activeElement.upSibling == null)
            {
                activeElement.OnUpPressed();
            }
            else
            {
                SelectNewElement(activeElement.upSibling);
                if (activeElement.Skip)
                    OnUpPressed();
            }
        }

        public void SelectNewElement(MenuElementBase newElement)
        {
            activeElement.OnUnhighlight();
            activeElement = newElement;
            activeElement.OnHighlight();
        }

        // TODO:  Set layout elements from menu panel OR should this be a panel layout helper?

        [ContextMenu("Setup Vertical Elements")]
        protected void SetupVerticalElements()
        {
            MenuElementBase[] elements = GetComponentsInChildren<MenuElementBase>();
            if (elements.Length == 0)
                return;

            if (elements.Length == 1)
            {
                elements[0].upSibling = null;
                elements[0].downSibling = null;
            }

            for (int i = 0; i < elements.Length; i++)
            {
                int pre = i - 1;
                int next = i + 1;

                if (pre >= 0)
                    elements[i].upSibling = elements[pre];
                else
                    elements[i].upSibling = elements[elements.Length - 1];

                if (next < elements.Length)
                    elements[i].downSibling = elements[next];
                else
                    elements[i].downSibling = elements[0];
            }
        }
    }
}