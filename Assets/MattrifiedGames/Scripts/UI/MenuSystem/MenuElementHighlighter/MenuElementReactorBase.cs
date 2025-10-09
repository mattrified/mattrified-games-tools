using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MenuElementReactorBase : MonoBehaviour
{

    public MenuSystemBase menuSystem;
    public MenuPanel panel;
    public MenuElementBase element;

    public bool alwaysHighlight;

    protected virtual void OnValidate()
    {
        if (menuSystem == null)
            menuSystem = GetComponentInParent<MenuSystemBase>();

        if (panel == null)
            panel = GetComponentInParent<MenuPanel>();

        if (element == null)
            element = GetComponent<MenuElementBase>();
    }

    private void Update()
    {
        if (alwaysHighlight)
        {
            OnHighlighted();
            return;
        }

        if (menuSystem.TestAllLocks)
            return;

        if (menuSystem.CurrentPanel.GetInstanceID() != panel.GetInstanceID())
        {
            OnUnhighlighted();
            return;
        }

        if (menuSystem.CurrentPanel.ActiveElement.GetInstanceID() == element.GetInstanceID())
        {
            OnHighlighted();
        }
        else
        {
            OnUnhighlighted();
        }
    }
    public abstract void OnUnhighlighted();
    public abstract void OnHighlighted();
}
