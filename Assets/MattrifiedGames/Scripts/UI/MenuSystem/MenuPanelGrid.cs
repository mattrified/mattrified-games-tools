using System;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.MenuSystem
{
    [Tooltip("A MenuPanel that handles navigating a grid of menu elements.")]
    public class MenuPanelGrid : MenuPanel
    {
        [Tooltip("The number of columns and rows in the setup.")]
        public Vector2Int ColumnsRows;

        [Tooltip("A page contains a set grid elements calculated with Columns times Rows.")]
        public int TotalPages;

        [SerializeField, Tooltip("The current column, row, and page of the grid setup.")]
        Vector3Int CurrentColumnRowPage;

        public int Column { get => CurrentColumnRowPage[0]; set => CurrentColumnRowPage[0] = value; }
        public int Row { get => CurrentColumnRowPage[1]; set => CurrentColumnRowPage[1] = value; }
        public int Page { get => CurrentColumnRowPage[2]; set => CurrentColumnRowPage[2] = value; }

        [Tooltip("If true, the grid movement will loop to the first or last column when out of range")]
        public bool loopColumns;

        [Tooltip("If true, the grid movement will loop to the first or last row when out of range")]
        public bool loopRows;

        [Tooltip("If true, the grid movement will loop to the first or last page when out of range.")]
        public bool loopPages;

        public int ItemsPerPage => ColumnsRows[0] * ColumnsRows[1];

        [SerializeField]
        private int _index;

        public int Index
        {
            get
            {
                return Page * ItemsPerPage + Column + Row * ColumnsRows[0];
            }
        }

        [Tooltip("The list of menu elements in the panel for grid setup")]
        public List<MenuElementBase> MenuElements;

        private void OnValidate()
        {
            Column = Mathf.Clamp(Column, 0, ColumnsRows[0] - 1);
            Row = Mathf.Clamp(Row, 0, ColumnsRows[1] - 1);
            Page = Mathf.Clamp(Page, 0, TotalPages - 1);

            _index = Index;

            for (int i = 0; i < MenuElements.Count; i++)
            {
                MenuElements[i].gameObject.SetActive(i >= Page * ItemsPerPage && i < (Page + 1) * ItemsPerPage);
            }
        }

        public override void OnLeftPressed()
        {
            base.OnLeftPressed();

            Column--;
            if (Column < 0)
            {
                GoToPreviousPage();
                if (!loopColumns)
                {
                    Column = ColumnsRows[0] - 1;
                }
            }
        }

        private void GoToPreviousPage()
        {
            Page--;
            if (Page < 0)
            {
                if (loopPages)
                {
                    Page = TotalPages - 1;
                }
            }
            Page = Mathf.Clamp(Page, 0, TotalPages - 1);
        }

        private void GoToNextPage()
        {
            Page++;
            if (Page >= TotalPages)
            {
                if (loopPages)
                {
                    Page = 0;
                }
            }
            Page = Mathf.Clamp(Page, 0, TotalPages - 1);
        }


        [ContextMenu("Setup")]
        private void Setup()
        {
            MenuElements = new List<MenuElementBase>(GetComponentsInChildren<MenuElementBase>(true));
            TotalPages = Mathf.CeilToInt((float)MenuElements.Count / ItemsPerPage);
        }
    }
}