using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    public abstract class PanelEffectorBase : MonoBehaviour
    {
        public MenuSystemBase menuSystem;

        public List<MenuPanel> activePanel;

        public bool utilizeInactivePanels;
        public List<MenuPanel> inactivePanels;

        protected bool? panelActive;
        protected virtual void Update()
        {
            panelActive = TestPanel();
        }

        public bool? TestPanel()
        {
            if (activePanel.Contains(menuSystem.CurrentPanel))
            {
                OnPanelActive();
                return true;
            }
            else
            {
                if (!utilizeInactivePanels || inactivePanels.Contains(menuSystem.CurrentPanel))
                {
                    OnPanelInactive();
                    return false;
                }
            }
            return null;
        }

        public abstract void OnPanelActive();
        public abstract void OnPanelInactive();
    }
}