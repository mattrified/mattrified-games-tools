using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MattrifiedLocalizationTMProUGUI : MattrifiedLocalizationBase
{
    public TextMeshProUGUI tmpProUGUI;

    protected override void AssignNewString(string value)
    {
        tmpProUGUI.text = value;
    }

    private void Update()
    {
        foreach (var d in InputSystem.devices)
        {
            if (d is Keyboard)
            {
                Debug.Log(d);

                Keyboard kb = (Keyboard)d;
                if (kb[Key.Numpad9].wasPressedThisFrame)
                {
                    languageSV.Value = SystemLanguage.Spanish;
                    Debug.Log(languageSV.Value);
                }
                else if (kb[Key.Numpad8].wasPressedThisFrame)
                {
                    languageSV.Value = SystemLanguage.English;
                    Debug.Log(languageSV.Value);
                }
            }
        }
    }

    protected override string GetDefaultText()
    {
        return tmpProUGUI?.text;
    }
}
