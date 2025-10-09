using MattrifiedGames.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class CSVEditorWindow : EditorWindow
{
    private string[] tabHeader = new string[] { "Files", "Edit" };

    int tab;

    string currentType;
    int length;

    CVSHelper cvsHelper = null;

    string[][] setup;

    [MenuItem("Tools/Mattrified Games/Spreadsheet Editor")]
    public static void OpenSEW()
    {
        EditorWindow.GetWindow<CSVEditorWindow>();
    }

    private void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, tabHeader);

        switch (tab)
        {
            case 0:
                cvsHelper = (CVSHelper)EditorGUILayout.ObjectField("CSV Helper", cvsHelper, typeof(CVSHelper), false);

                if (GUILayout.Button("Setup"))
                {
                    string s = cvsHelper.mainCSV.text;
                    string[] strRows = s.Split('\n');
                    setup = new string[strRows.Length][];
                    for (int i = 0; i < setup.Length; i++)
                    {
                        string[] comma = strRows[i].Split(',');
                        setup[i] = comma;
                    }

                }
                break;
            case 1:
                DrawItems();
                break;
        }
    }

    private void DrawItems()
    {
        for (int i = 0; i < setup.Length; i++)
        {
            for (int j = 0; j < setup[i].Length; j++)
            {
                GUILayout.Label(setup[i][j]);
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
