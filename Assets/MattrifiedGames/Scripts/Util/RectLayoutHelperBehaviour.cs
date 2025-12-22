using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    public class RectLayoutHelperBehaviour : MonoBehaviour
    {
        public bool autoPopulate;
        public bool autoUpdate;

        public List<RectTransform> children;

        public List<MenuElementBase> menuElements;

        public int columns, rows;

        private void OnValidate()
        {
            UpdateChildren(autoUpdate);
        }
        public void UpdateChildren(bool updateRect)
        {
            children = new List<RectTransform>();
            foreach (Transform t in transform)
            {
                if (!t.gameObject.activeSelf)
                    continue;

                var rt = t.GetComponent<RectTransform>();
                if (rt == null)
                    continue;

                children.Add(rt);
            }

            menuElements = new List<MenuElementBase>();
            for (int i = 0; i < children.Count; i++)
            {
                MenuElementBase mb = children[i].GetComponent<MenuElementBase>();
                if (mb != null)
                {
                    menuElements.Add(mb);
                }
            }

            if (updateRect)
                UpdateRects();
        }

        public Vector2 minOffset;
        public Vector2 maxOffset;

        public LayoutType layoutType;

        public enum LayoutType
        {
            Vertical_TopBottom = 0,
            Vertical_BottomTop = 1,
            Horizontal_LeftRight = 2,
            Horizontal_RightLeft = 4,
            Vertical_Offset = 5,
            Horizontal_Offset = 6,
            Grid = 10,
        }

        public void UpdateRects()
        {
            switch (layoutType)
            {
                case LayoutType.Horizontal_LeftRight: UpdateHorizontal(true); break;
                case LayoutType.Horizontal_RightLeft: UpdateHorizontal(false); break;
                case LayoutType.Vertical_BottomTop: UpdateVertical(false); break;
                case LayoutType.Vertical_TopBottom: UpdateVertical(true); break;
                case LayoutType.Vertical_Offset: UpdateVerticalOffset(); break;
                case LayoutType.Horizontal_Offset: UpdateHorizontalOffset(); break;
                case LayoutType.Grid: UpdateGrid(); break;
            }

            // TODO:  button setup
        }

        private void UpdateHorizontalOffset()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].anchoredPosition = minOffset + maxOffset * i;
            }

            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].leftSibling = menuElements[i - 1 >= 0 ? i - 1 : menuElements.Count - 1];
                menuElements[i].rightSibling = menuElements[i + 1 < menuElements.Count ? i + 1 : 0];
            }
        }

        private void UpdateVerticalOffset()
        {
            for (int i = 0; i < children.Count; i++)
            {
                children[i].anchoredPosition = minOffset + maxOffset * i;
            }

            bool topToBottom = maxOffset.y <= 0f;
            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].upSibling = menuElements[i - 1 >= 0 ? i - 1 : menuElements.Count - 1];
                menuElements[i].downSibling = menuElements[i + 1 < menuElements.Count ? i + 1 : 0];

                // Swaps the items
                if (!topToBottom)
                {
                    var rr = menuElements[i].upSibling;
                    menuElements[i].upSibling = menuElements[i].downSibling;
                    menuElements[i].downSibling = rr;
                }
            }
        }


        void UpdateGrid()
        {
            if (rows * columns != children.Count)
            {
                Debug.LogWarning($"Should have {rows * columns} children, not {children.Count}.");
                return;
            }

            float minC = 1f / columns;
            float minR = 1f / rows;

            for (int i = 0; i < children.Count; i++)
            {
                int c = i % columns;
                int r = i / columns;

                children[i].anchorMin = new Vector2(c * minC, (rows - 1 - r) * minR);
                children[i].anchorMax = new Vector2((c + 1) * minC, (rows - r) * minR);

                children[i].offsetMin = minOffset;
                children[i].offsetMax = -maxOffset;
            }

            for (int i = 0; i < menuElements.Count; i++)
            {
                int leftIndex = i - 1;
                if (i % columns == 0)
                    leftIndex = i + columns - 1;

                int rightIndex = i + 1;
                if (i % columns == columns - 1)
                    rightIndex = i - columns + 1;

                int topIndex = i - columns;
                if (topIndex < 0)
                {
                    topIndex = (columns * rows) - columns + (i % columns);
                }

                int bottomIndex = i + columns;
                if (bottomIndex >= rows * columns)
                {
                    bottomIndex = i % columns;
                }

                menuElements[i].leftSibling = menuElements[leftIndex];
                menuElements[i].RightSibling = menuElements[rightIndex];
                menuElements[i].upSibling = menuElements[topIndex];
                menuElements[i].downSibling = menuElements[bottomIndex];
            }
        }

        void UpdateHorizontal(bool leftToRight)
        {
            if (!leftToRight)
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2((children.Count - 1 - i) * min, 0);
                    children[i].anchorMax = new Vector2(((children.Count - i)) * min, 1);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }
            else
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(i * min, 0);
                    children[i].anchorMax = new Vector2((i + 1) * min, 1);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }

            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].leftSibling = menuElements[i - 1 >= 0 ? i - 1 : menuElements.Count - 1];
                menuElements[i].rightSibling = menuElements[i + 1 < menuElements.Count ? i + 1 : 0];
            }
        }

        void UpdateVertical(bool topToBottom)
        {
            if (topToBottom)
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(0, (children.Count - 1 - i) * min);
                    children[i].anchorMax = new Vector2(1, ((children.Count - i)) * min);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }
            else
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(0, i * min);
                    children[i].anchorMax = new Vector2(1, (i + 1) * min);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }

            for (int i = 0; i < menuElements.Count; i++)
            {
                menuElements[i].upSibling = menuElements[i - 1 >= 0 ? i - 1 : menuElements.Count - 1];
                menuElements[i].downSibling = menuElements[i + 1 < menuElements.Count ? i + 1 : 0];

                // Swaps the items
                if (!topToBottom)
                {
                    var rr = menuElements[i].upSibling;
                    menuElements[i].upSibling = menuElements[i].downSibling;
                    menuElements[i].downSibling = rr;
                }
            }
        }

        //GameObject go;

        /*private void OnGUI()
        {
            if (Selection.activeGameObject != null)
            {
                if (GUILayout.Button("Build"))
                {
                    go = Selection.activeGameObject;

                    children = new List<RectTransform>();
                    foreach (Transform t in Selection.activeGameObject.transform)
                    {
                        if (t is RectTransform)
                            children.Add((RectTransform)t);
                    }
                }
            }

            if (go == null)
            {
                return;
            }

            GUILayout.Label("Building for:  " + go.name);

            minOffset = EditorGUILayout.Vector2Field("Min Offset", minOffset);
            maxOffset = EditorGUILayout.Vector2Field("Max Offset", maxOffset);

            if (children == null || children.Count <= 0)
                return;

            if (GUILayout.Button("Hori"))
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(i * min, 0);
                    children[i].anchorMax = new Vector2((i + 1) * min, 1);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }

            if (GUILayout.Button("Vert - Top to Bottom"))
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(0, (children.Count - 1 - i) * min);
                    children[i].anchorMax = new Vector2(1, ((children.Count - i)) * min);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }

            if (GUILayout.Button("Vert - Bottom to Top"))
            {
                float min = 1f / children.Count;

                for (int i = 0; i < children.Count; i++)
                {
                    children[i].anchorMin = new Vector2(0, i * min);
                    children[i].anchorMax = new Vector2(1, (i + 1) * min);

                    children[i].offsetMin = minOffset;
                    children[i].offsetMax = -maxOffset;
                }
            }

            if (GUILayout.Button("Setup Menu Vertically"))
            {
                for (int i = 0; i < children.Count; i++)
                {
                    var mb = children[i].GetComponent<MenuElementBase>();

                    int pre = i - 1;
                    if (pre < 0)
                        pre = children.Count - 1;

                    int post = i + 1;

                    if (post >= children.Count)
                        post = 0;

                    var preMB = children[pre].GetComponent<MenuElementBase>();
                    var postMB = children[post].GetComponent<MenuElementBase>();

                    if (mb == null || preMB == null || postMB == null)
                        continue;

                    mb.upSibling = preMB;
                    mb.downSibling = postMB;
                    mb.leftSibling = null;
                    mb.rightSibling = null;
                }
            }
        }*/
    }
}