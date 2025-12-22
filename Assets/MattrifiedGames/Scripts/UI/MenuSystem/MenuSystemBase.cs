using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace MattrifiedGames.MenuSystem
{
    /// <summary>
    /// Abstract class to setup the menu system.  Abstract since the input type might change.
    /// </summary>
    public abstract class MenuSystemBase : MonoBehaviour
    {
        [SerializeField(), Tooltip("The initial panel of the menu system.")]
        protected MenuPanel startingPanel;

        [SerializeField, Tooltip("The panel currently being focused on.")]

        protected MenuPanel currentPanel;

        /// <summary>
        /// The stack of panels for going back and forth between pages.
        /// </summary>
        protected Stack<MenuPanel> _panelStack = new Stack<MenuPanel>();

        [SerializeField(), Tooltip("If true, the panel stack will be cleared and the menu system will return to the starting panel.")]
        protected bool resetOnEnable;

        #region Hold And Repeat Test
        [SerializeField(), Tooltip("How holding left will react in the menu system.")]
        protected HoldAndRepeatTest leftHoldTest;

        [SerializeField(), Tooltip("How holding right will react in the menu system.")]
        protected HoldAndRepeatTest rightHoldTest;

        [SerializeField(), Tooltip("How holding up will react in the menu system.")]
        protected HoldAndRepeatTest upHoldTest;

        [SerializeField(), Tooltip("How holding down will react in the menu system.")]
        protected HoldAndRepeatTest downHoldTest;
        #endregion

        [Tooltip("Events that are invoked when the menu system is enabled.")]
        public UnityEvent OnEnableEvent;

        [Tooltip("How many in-game ticks will the menu system be locked for.  Used to prevent menus events from being automatically triggered between different panel transitions.")]
        public int lockTicks = 10;

        [SerializeField(), Tooltip("Is the current menu system locked.")]
        private bool _locked;
        public bool Locked { get { return _locked; } set { lockTicks = 10; _locked = value; } }

        public int LockTicks { get { return lockTicks; } set { lockTicks = value; } }

        public virtual bool LockedAndLockTicks => Locked || lockTicks > 0;

        /// <summary>
        /// Gets the reference to the current system.
        /// </summary>
        public MenuPanel CurrentPanel
        {
            get
            {
                return currentPanel;
            }
        }

        public int CurrentPanelInstanceID
        {
            get
            {
                return currentPanel.GetInstanceID();
            }
        }

        public virtual void Update()
        {
            if (Locked || lockTicks > 0)
                return;

            if (!currentPanel.Locked)
            {
                if (currentPanel.UpdateFromBase(this))
                    return;
                CheckInput();
            }
        }

        private void FixedUpdate()
        {
            lockTicks--;
        }

        public abstract void CheckInput();

        protected virtual void OnEnable()
        {
            // On enable we always set the lock ticks to one to prevent accidental confirmation
            lockTicks = 21;

            if (resetOnEnable)
            {
                _panelStack.Clear();
                currentPanel = startingPanel;
            }
            else if (currentPanel == null)
            {
                currentPanel = startingPanel;
                currentPanel.Open(this);
            }

            OnEnableEvent.Invoke();
        }

        public void GoToNewPanel(MenuPanel nextPanel)
        {
            _panelStack.Push(currentPanel);
            currentPanel.Close(this);

            currentPanel = nextPanel;
            currentPanel.Open(this);
        }

        public void GoToNewPanelClear(MenuPanel nextPanel)
        {
            _panelStack.Clear();
            currentPanel.Close(this);

            currentPanel = nextPanel;
            currentPanel.Open(this);
        }

        public void ReturnToStartingPanel()
        {
            GoToNewPanelClear(startingPanel);
        }

        public void TraverseBackToPanel(MenuPanel panel)
        {
            if (!_panelStack.Contains(panel))
            {
                Debug.LogWarning("This panel has to be contained to traverse back to.  Will remain on current panel");
                return;
            }

            while (currentPanel != panel && _panelStack.Count > 0)
            {
                GoToPreviousPanel();
            }
        }

        public void GoToPreviousPanel()
        {
            if (_panelStack.Count == 0)
                return;

            currentPanel.Close(this);

            currentPanel = _panelStack.Pop();
            lockTicks = 10;

            currentPanel.Open(this);
        }
    }
}