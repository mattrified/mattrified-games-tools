using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public abstract class MenuElementBase : MonoBehaviour
{
    public MenuElementSFXPalette sfxPalette;

    public MenuElementBase leftSibling, rightSibling, upSibling, downSibling;

    public bool skip;
    public bool Skip { get { return skip; } set { skip = value; } }

    public MenuElementBase LeftSibling { get { return leftSibling; } set { leftSibling = value; } }
    public MenuElementBase RightSibling { get { return rightSibling; } set { rightSibling = value; } }
    public MenuElementBase UpSibling { get { return upSibling; } set { upSibling = value; } }
    public MenuElementBase DownSibling { get { return downSibling; } set { downSibling = value; } }

    public virtual void OnLeftPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnLeftSFX(true); }

    public virtual void OnRightPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnRightSFX(true); }

    public virtual void OnUpPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnUpSFX(true); }

    public virtual void OnDownPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnDownSFX(true); }

    public virtual void OnLeftHold(bool isHolding) { if (gameObject.activeSelf && sfxPalette && isHolding) { sfxPalette.OnLeftSFX(false); } }

    public virtual void OnRightHold(bool isHolding) { if (gameObject.activeSelf && sfxPalette && isHolding) { sfxPalette.OnRightSFX(false); } }

    public virtual void OnUpHold(bool isHolding) { if (gameObject.activeSelf && sfxPalette && isHolding) { sfxPalette.OnUpSFX(false); } }

    public virtual void OnDownHold(bool isHolding) { if (gameObject.activeSelf && sfxPalette && isHolding) { sfxPalette.OnDownSFX(false); } }

    public virtual void OnConfirmPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnConfirmSFX(); }

    public virtual void OnCancelPressed() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnCancelSFX(); }

    public virtual bool UpdateFromPanel(MenuSystemBase menuSystem, MenuPanel panel) { return false; }

    public List<RuntimePlatform> platformList;
    public bool excludePlatformList;
    public UnityEvent OnFailPlatformTest;

    protected virtual void Start()
    {
        if (platformList == null || platformList.Count == 0)
            return;

        if (platformList.Contains(Application.platform) == excludePlatformList)
        {
            skip = true;
            OnFailPlatformTest.Invoke();
        }
    }

    public virtual void OnInputPressed(int index)
    {
        if (gameObject.activeSelf && sfxPalette)
        {
            sfxPalette.OnIndexedSFX(index);
        }
        
    }

    public virtual void OnHighlight() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnHighlightSFX(); }

    public virtual void OnUnhighlight() { if (gameObject.activeSelf && sfxPalette) sfxPalette.OnUnhighlightSFX(); }

    public void SimulateButtonClick(Button button)
    {
        button.onClick.Invoke();
    }

    public void SimulateToggle(Toggle toggle)
    {
        toggle.isOn = !toggle.isOn;
    }

    public void Remove()
    {
        var dS = downSibling;
        var uS = upSibling;
        var lS = leftSibling;
        var rS = rightSibling;

        downSibling = null;
        rightSibling = null;
        leftSibling = null;
        upSibling = null;

        if (dS != null)
            dS.upSibling = uS;

        if (uS != null)
            uS.downSibling = dS;

        if (lS != null)
            lS.rightSibling = rS;

        if (rS != null)
            rS.leftSibling = lS;

        gameObject.SetActive(false);
    }
}