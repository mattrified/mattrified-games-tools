using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

[CreateAssetMenu()]
public class MGInputGameSpriteMapping : ScriptableObject
{
    public enum DisplayType
    {
        Arrows = 0,
        Direct = 1,
        Numpad = 2,
    }

    public DisplayType displayType;

    public MGInputSpriteMapping spriteMapper;

    private List<string> stringList = new List<string>();

    [SerializeField()]
    private List<InputSetup> inputSetupList = new List<InputSetup>();

    public enum InputSetupEnum
    {
        NA = 0,
        Confirm = 1,
        Cancel = 2,
        Static = 3,
    }

    [System.Serializable()]
    public class InputSetup
    {
        public string label;
        public InputSetupEnum setupID;

        public string[] keyboardInputID;
        public string[] gamepadInputID;
        public bool matchInputIDs;

        public string numpadString;
        public string arrowString;

        public Key key;
        public GamepadButton btn;

        internal string GetText(MGInputGameSpriteMapping mapping, MGInput input, bool isKeyboard)
        {
            // This means, regardless of the entry, this is what we will return
            switch (setupID)
            {
                case InputSetupEnum.Static: return isKeyboard ? mapping.spriteMapper[key] : mapping.spriteMapper[btn];
                case InputSetupEnum.Confirm:
                    if (input != null)
                    {
                        return isKeyboard ? mapping.spriteMapper[input.confirmInput.key] : mapping.spriteMapper[input.confirmInput.gpButton];
                    }
                    else
                    {
                        return "Confirm";
                    }
                case InputSetupEnum.Cancel:
                    if (input != null)
                    {
                        return isKeyboard ? mapping.spriteMapper[input.cancelInput.key] : mapping.spriteMapper[input.cancelInput.gpButton];
                    }
                    else
                    {
                        return "Cancel";
                    }
            }

            if (mapping.displayType == DisplayType.Numpad)
            {
                if (!string.IsNullOrEmpty(numpadString))
                    return numpadString;
            }

            if (mapping.displayType == DisplayType.Arrows)
            {
                if (!string.IsNullOrEmpty(arrowString))
                    return arrowString;
            }

            string result = string.Empty;
            // We can't display anthing here if there
            if (isKeyboard)
            {
                for (int i = 0; i < keyboardInputID.Length; i++)
                {
                    if (input != null && input.mainInputSet.TryAndFind(keyboardInputID[i], out var info))
                    {
                        result = i == 0 ? mapping.spriteMapper[info.key] : result + "+" + mapping.spriteMapper[info.key];
                    }
                    else
                    {
                        result = i == 0 ? keyboardInputID[i] : result + "+" + keyboardInputID[i];
                    }
                }
            }
            else
            {
                for (int i = 0; i < gamepadInputID.Length; i++)
                {
                    if (input != null && input.mainInputSet.TryAndFind(gamepadInputID[i], out var info))
                    {
                        result = i == 0 ? mapping.spriteMapper[info.gpButton] : result + "+" + mapping.spriteMapper[info.gpButton];
                    }
                    else
                    {
                        result = i == 0 ? gamepadInputID[i] : result + "+" + gamepadInputID[i];
                    }
                }
            }

            return result;
        }
    }

    public void OnValidate()
    {
        foreach (var item in inputSetupList)
        {
            if (item.matchInputIDs)
            {
                item.gamepadInputID = new string[item.keyboardInputID.Length];
                item.keyboardInputID.CopyTo(item.gamepadInputID, 0);
            }
        }
    }

    public string[] GetStringArray(MGInput input)
    {
        if (input == null)
        {
            return new string[inputSetupList.Count];
        }

        bool isKeyboard = input == null || input.device is Keyboard;

        stringList.Clear();

        foreach (var item in inputSetupList)
        {
            stringList.Add(item.GetText(this, input, isKeyboard));
        }

        return stringList.ToArray();
    }

    public string UpdateText(MGInput input, string text)
    {
        return string.Format(text, GetStringArray(input));
    }
}