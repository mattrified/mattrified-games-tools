using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MattrifiedLocalizationBase : MonoBehaviour
{
    [SerializeField()]
    protected LanguageScriptableValue languageSV;

    [SerializeField()]
    protected MGLocalizationDictionary localizationDictionary;

    [SerializeField(), Delayed()]
    protected string key;

#if UNITY_EDITOR
    public string LCKey { get { return key; } set { key = value; OnValidate(); } }

    public string[] LCKeyList { get { return localizationDictionary?.KeyArray; } }

    public MGLocalizationDictionary LocalizationDictionary => localizationDictionary;

#endif

    [SerializeField()]
    protected LocalizeValidation validation;

    [System.Flags()]
    protected enum LocalizeValidation
    {
        Pass = 0,
        NoLanguage = 1,
        NoDictionary = 2,
        NoLanguageORDictionary = 3,
        KeyNotFound = 4,
    }

    public void OnValidate()
    {
        validation = LocalizeValidation.Pass;
        if (languageSV == null)
        {
            validation |= LocalizeValidation.NoLanguage;
        }

        if (localizationDictionary == null)
        {
            validation |= LocalizeValidation.NoDictionary;
        }

        if (validation != LocalizeValidation.Pass)
            return;

        if (!localizationDictionary.HasKey(key))
            validation |= LocalizeValidation.KeyNotFound;
    }

    protected abstract void AssignNewString(string value);

    private void Awake()
    {
        if (validation != LocalizeValidation.Pass)
        {
            Debug.Log("This has not been validated.");
        }

        // Sets the value
        languageSV.AddOnValueChangedEvent(OnLanguageChanged);

        // Sets what the text should be on awake.
        OnLanguageChanged(languageSV.Value);
    }

    private void OnDestroy()
    {
        languageSV.RemoveOnSetEvent(OnLanguageChanged);
    }

    protected void OnLanguageChanged(SystemLanguage newLanguage)
    {
        string newString = localizationDictionary.GetValue(key, newLanguage);
        AssignNewString(newString);
    }

    [ContextMenu("Add Key")]
    protected void AddKey()
    {
        if (validation == LocalizeValidation.KeyNotFound)
        {
            localizationDictionary.AddKey(key, GetDefaultText());
        }
        else if (validation == LocalizeValidation.Pass)
        {
            Debug.Log("Key exists.");
        }
        else
        {
            Debug.Log("Missing item.");
        }
        OnValidate();
    }

    protected abstract string GetDefaultText();
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(MattrifiedLocalizationBase), true)]
public class MLEditor : UnityEditor.Editor
{
    UnityEditor.Editor dictEditor;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MattrifiedLocalizationBase t = target as MattrifiedLocalizationBase;

        var kL = t.LCKeyList;
        if (kL == null)
        {
            Debug.Log("NO KL");
            return;
        }

        int result = UnityEditor.EditorGUILayout.Popup("Key Selector", -1, kL);
        if (result >= 0)
            t.LCKey = kL[result];

        if (dictEditor == null)
        {
             if (t.LocalizationDictionary != null)
                dictEditor = UnityEditor.Editor.CreateEditor(t.LocalizationDictionary);
        }
        else
        {
            dictEditor.DrawHeader();
            dictEditor.OnInspectorGUI();

        }
    }
}

#endif