using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.MattrifiedGames.Scripts.UI
{
    [ExecuteInEditMode()]
    public class LayoutDriver : UIBehaviour, ILayoutController
    {
        public Vector2 margin;
        public RectTransform controlledChild;
        public Vector2 max;

        new protected void OnValidate()
        {
            SetLayoutHorizontal();
            SetLayoutVertical();
        }

        protected override void Start()
        {
            base.Start();
            SetLayoutHorizontal();
            SetLayoutVertical();
        }

        public void SetLayoutHorizontal()
        {
            if (controlledChild == null)
                return;

            var size = controlledChild.sizeDelta;
            size.x = Mathf.Min(max.x, margin.x + LayoutUtility.GetPreferredWidth((RectTransform)transform));
            controlledChild.sizeDelta = size;
        }

        public void SetLayoutVertical()
        {
            if (controlledChild == null)
                return;

            var size = controlledChild.sizeDelta;
            size.y = Mathf.Min(max.y, margin.y + LayoutUtility.GetPreferredHeight((RectTransform)transform));
            controlledChild.sizeDelta = size;
        }
    }
}
