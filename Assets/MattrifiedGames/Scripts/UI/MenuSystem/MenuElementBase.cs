using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MattrifiedGames.MenuSystem
{
    public abstract class MenuElementBase : MonoBehaviour
    {
        [Header("Navigation")]
        [Tooltip("The left sibling of this menu element.")]
        public MenuElementBase leftSibling;

        [Tooltip("The right sibling of this menu element.")]
        public MenuElementBase rightSibling;

        [Tooltip("The up sibling of this menu element.")]
        public MenuElementBase upSibling;

        [Tooltip("The down sibling of this menu element.")]
        public MenuElementBase downSibling;

        /// <summary>
        /// The left sibling of the menu element base.  A public property so elements can be set in Unity.
        /// </summary>
        public MenuElementBase LeftSibling { get { return leftSibling; } set { leftSibling = value; } }

        /// <summary>
        /// The right sibling of the menu element base.  A public property so elements can be set in Unity.
        /// </summary>
        public MenuElementBase RightSibling { get { return rightSibling; } set { rightSibling = value; } }

        /// <summary>
        /// The up sibling of the menu element base.  A public property so elements can be set in Unity.
        /// </summary>
        public MenuElementBase UpSibling { get { return upSibling; } set { upSibling = value; } }

        /// <summary>
        /// The down sibling of the menu element base.  A public property so elements can be set in Unity.
        /// </summary>
        public MenuElementBase DownSibling { get { return downSibling; } set { downSibling = value; } }

        [Tooltip("If true, this element will be skipped when navigating siblings.")]
        public bool skip;

        /// <summary>
        /// Property for skipping; a public property so elements can be set in Unity.
        /// </summary>
        public bool Skip { get { return skip; } set { skip = value; } }

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


        [Header("SFX Palette")]
        [Tooltip("A collection of SFX that are referenced to play different SFX when using this menu element.")]
        public MenuElementSFXPalette sfxPalette;

        [Header("Runtime Platform Information")]
        [Tooltip("A list of platforms that affect this menu elements.")]
        public List<RuntimePlatform> platformList;

        [Tooltip("If true, this button will be excluded if the current platform is contained in the platform list.")]
        public bool excludePlatformList;

        [Tooltip("Event triggered if this menu elements failed the platform test.")]
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

        /// <summary>
        /// Helper that can be used to fake pressing a button with this menu element.
        /// </summary>
        /// <param name="button">The button whose "onClick" will be invoked.</param>
        public void SimulateButtonClick(Button button)
        {
            button.onClick.Invoke();
        }

        /// <summary>
        /// Helper that can be used to fake pressing a toggling a menu element.
        /// </summary>
        /// <param name="button">The Toggle element that will be flipped/toggled.</param>
        public void SimulateToggle(Toggle toggle)
        {
            toggle.isOn = !toggle.isOn;
        }

        /// <summary>
        /// If a menu element is removed from the list, it removes its reference from its siblings and replaces it with any siblings this element may have.
        /// </summary>
        public void Remove(bool destroyGameObject = false)
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

            if (!destroyGameObject)
            {
                gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}