using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class MenuSystemBase : MonoBehaviour
{
    [SerializeField()]
    protected MenuPanel startingPanel;

    [SerializeField]
    protected MenuPanel currentPanel;
    protected Stack<MenuPanel> panelQueue = new Stack<MenuPanel>();

    [SerializeField()]
    protected bool resetOnEnable;

    [SerializeField()]
    protected HoldAndRepeatTest leftHoldTest, rightHoldTest, upHoldTest, downHoldTest;

    public UnityEvent OnEnableEvent;

    public MenuPanel CurrentPanel
    {
        get
        {
            return currentPanel;
        }
    }

    public int lockTicks = 10;

    [SerializeField()]
    private bool _locked;
    public bool Locked { get { return _locked; } set { lockTicks = 10; _locked = value; }  }

    public int LockTicks { get { return lockTicks; } set { lockTicks = value; } }

    public virtual bool TestAllLocks
    {
        get
        {
            return Locked || lockTicks > 0;
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
            panelQueue.Clear();
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
        panelQueue.Push(currentPanel);
        currentPanel.Close(this);

        currentPanel = nextPanel;
        currentPanel.Open(this);
    }

    public void GoToNewPanelClear(MenuPanel nextPanel)
    {
        panelQueue.Clear();
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
        if (!panelQueue.Contains(panel))
        {
            Debug.LogError("This panel has to be contained to traverse");
            return;
        }

        while (currentPanel != panel && panelQueue.Count > 0)
        {
            GoToPreviousPanel();
        }
    }

    public void GoToPreviousPanel()
    {
        if (panelQueue.Count == 0)
            return;

        currentPanel.Close(this);

        currentPanel = panelQueue.Pop();
        lockTicks = 10;

        currentPanel.Open(this);
    }
}