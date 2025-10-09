using MattrifiedGames.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DebugTestSO))]
public class DebugTestSOEditor : Editor
{
    const string USING_INCONTROL = "USING_INCONTROL";
    const string USING_QUANTUM = "USING_QUANTUM";
    const string USING_CURVY = "USING_CURVY";
    const string USING_SGF = "USING_SGF";
    const string USING_CINEMACHINE = "USING_CINEMACHINE";
    const string USING_PHOTON = "USING_PHOTON";
    const string USING_TRUESYNC = "USING_TRUESYNC";
    const string USING_TEXT_MESH_PRO = "USING_TEXTMESHPRO";

    [System.NonSerialized()]
    public Dictionary<BuildTargetGroup, string> defineSymbols;

    public string[] usingStrings;

    public override void OnInspectorGUI()
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        base.OnInspectorGUI();
        if (debugTest.data != null && GUILayout.Button("Load"))
        {
            debugTest.LoadData();
        }

        if (usingStrings == null)
        {
            usingStrings = new string[]
            {
                USING_CINEMACHINE,USING_CURVY,USING_INCONTROL,USING_PHOTON,USING_QUANTUM,USING_SGF,USING_TRUESYNC,USING_TEXT_MESH_PRO
            };
        }

        if (defineSymbols == null)
        {
            defineSymbols = new Dictionary<BuildTargetGroup, string>();
            for (int i = 0; i < debugTest.buildTargetGroups.Count; i++)
            {
                defineSymbols.Add(debugTest.buildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(debugTest.buildTargetGroups[i]));
            }
        }
        else
        {
            for (int i = 0; i < debugTest.buildTargetGroups.Count; i++)
            {
                if (defineSymbols.ContainsKey(debugTest.buildTargetGroups[i]))
                    continue;
                defineSymbols.Add(debugTest.buildTargetGroups[i], PlayerSettings.GetScriptingDefineSymbolsForGroup(debugTest.buildTargetGroups[i]));
            }
        }

        for (int i = 0; i < debugTest.buildTargetGroups.Count; i++)
        {
            EditorGUILayout.Space();

            defineSymbols[debugTest.buildTargetGroups[i]] = EditorGUILayout.TextField(debugTest.buildTargetGroups[i].ToString(),
                defineSymbols[debugTest.buildTargetGroups[i]]);

            for (int j = 0; j < usingStrings.Length; j++)
            {
                bool s = defineSymbols[debugTest.buildTargetGroups[i]].Contains(usingStrings[j]);
                bool toggle = EditorGUILayout.Toggle(usingStrings[j], s);
                if (s != toggle)
                {
                    defineSymbols[debugTest.buildTargetGroups[i]] = EditSymbol(defineSymbols[debugTest.buildTargetGroups[i]],
                        usingStrings[j], toggle);
                    SetSymbols(debugTest.buildTargetGroups[i]);
                }
            }
        }
    }

    private void SetSymbols(object buildTargetGroup)
    {
        BuildTargetGroup group = (BuildTargetGroup)buildTargetGroup;

        PlayerSettings.SetScriptingDefineSymbolsForGroup(group, defineSymbols[group]);
        AssetDatabase.Refresh();
    }

    private string EditSymbol(string symbols, string indSymbol, bool add)
    {
        if (add)
        {
            if (string.IsNullOrEmpty(symbols))
                return new string(indSymbol.ToCharArray());
            else if (symbols.EndsWith(";"))
                return symbols + indSymbol;
            else
                return symbols + ";" + indSymbol;
        }
        else
        {
            if (symbols.Contains(indSymbol + ";"))
            {
                return symbols.Replace(indSymbol + ";", "");
            }
            else
            {
                return symbols.Replace(indSymbol, "");
            }
        }
    }

    void AddInControl(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_INCONTROL, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void AddCinemachine(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_CINEMACHINE, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void AddSGF(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_SGF, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void AddQuantum(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_QUANTUM, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void AddCurvy(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_CURVY, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void AddTextMeshPro(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_TEXT_MESH_PRO, true);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveInControl(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_INCONTROL, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveSGF(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_SGF, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveQuantum(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_QUANTUM, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveCinemachine(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_CINEMACHINE, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveCurvy(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_CURVY, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    void RemoveTextMeshProp(object index)
    {
        DebugTestSO debugTest = (DebugTestSO)target;
        string newSymbol = EditSymbol(defineSymbols[debugTest.buildTargetGroups[(int)index]], USING_TEXT_MESH_PRO, false);
        defineSymbols[debugTest.buildTargetGroups[(int)index]] = newSymbol;
    }

    private bool CheckSymbols(string symbols, string indSymbol)
    {
        return symbols.Contains(indSymbol);
    }
}