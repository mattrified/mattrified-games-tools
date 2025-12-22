using UnityEngine;
namespace MattrifiedGames.MenuSystem
{
    [Tooltip("MonoBehaviour that test if the object is currently highlighted and react / updated it accordingly.")]
    public abstract class MenuElementReactorBase : MonoBehaviour
    {
        [Tooltip("Reference to the menu system used to test this element reactor.")]
        public MenuSystemBase menuSystem;

        [Tooltip("The MenuPanel that contains this reactor.")]
        public MenuPanel panel;

        [Tooltip("The Element controlling this reactor.")]
        public MenuElementBase element;

        [Tooltip("If true, this element will always highlight.")]
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

            if (menuSystem.LockedAndLockTicks)
                return;

            if (menuSystem.CurrentPanelInstanceID != panel.GetInstanceID())
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
}