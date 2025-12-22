using MattrifiedGames.MenuSystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace MattrifiedGames.ButtonConfig
{
    public class MGInputButtonConfigUI : MenuPanel
    {
        public int playerIndex;

        public TextMeshProUGUI header;
        public TextMeshProUGUI subText;

        public MGInputGameSpriteMapping gameSpriteMapping;
        public MGInputButtonConfigUILine[] lines;

        public RectLayoutHelperBehaviour layoutHelper;
        public List<MGInputButtonConfigUILine> keyboardOnlyIgnoreLines;

        public string normalSubText;
        public string resetSubText;
        public string exitSubText;
        public string keyboardConfigSubText;
        public string keyboardOnly;
        public string testSubText;

        int remappingKeyboardIndex = -1;

        List<Key> usedKeys = new List<Key>();

        [SerializeField()]
        List<Key> keysToIgnore = new List<Key>();

        MGInputButtonConfigUILine.BtnConfigLineInfo previousLineInfo = MGInputButtonConfigUILine.BtnConfigLineInfo.NA;

        public UnityEvent OnRequestExitEvent;

        private void OnValidate()
        {
            lines = GetComponentsInChildren<MGInputButtonConfigUILine>();
        }

        [ContextMenu("Set Ignore Keys")]
        void SetIgnoreKeys()
        {
            keysToIgnore = new List<Key>()
        {
            Key.Enter,
            Key.Backspace,
            Key.Escape,
        };

            for (Key k = Key.F1; k <= Key.F24; k++)
                keysToIgnore.Add(k);
        }

        public override void Open(MenuSystemBase menu)
        {
            base.Open(menu);
            var input = MGInputManager.playerInputs[playerIndex];
            if (input == null)
            {
                header.text = "Device Missing";
                return;
            }

            header.text = "Player " + (playerIndex + 1) + " Button Config:  " + input.device.name;

            foreach (var line in lines)
                line.SetInfo(input, gameSpriteMapping.spriteMapper);

            bool isKey = input.device is Keyboard;

            foreach (var line in keyboardOnlyIgnoreLines)
                line.gameObject.SetActive(isKey);

            layoutHelper.UpdateChildren(true);

            SelectNewElement(layoutHelper.menuElements[0]);
        }

        private void Update()
        {
            if (remappingKeyboardIndex < 0)
                return;

            var input = MGInputManager.playerInputs[playerIndex];
            if (input == null)
                return;

            for (Key k = Key.Space; k <= Key.F24; k++)
            {
                if (usedKeys.Contains(k))
                    continue;

                if (input.mainInputSet.TryAndFind(lines[remappingKeyboardIndex].inputID, out var inputResult))
                {
                    if (input.TestStaticInput(k, GamepadButton.A) == MGInputState.Pressed)
                    {
                        inputResult.key = new Key[] { k };
                        usedKeys.Add(k);
                        lines[remappingKeyboardIndex].SetInfo(input, gameSpriteMapping.spriteMapper);
                        remappingKeyboardIndex++;
                        if (remappingKeyboardIndex >= lines.Length)
                        {
                            remappingKeyboardIndex = -1;
                            Locked = false;
                        }
                        else
                            SelectNewElement(lines[remappingKeyboardIndex]);
                        break;
                    }
                }
                else
                {
                    remappingKeyboardIndex++;
                    if (remappingKeyboardIndex >= lines.Length)
                    {
                        remappingKeyboardIndex = -1;
                        Locked = false;
                        break;
                    }
                }
            }
        }

        public override bool UpdateFromBase(MenuSystemBase msBase)
        {
            var input = MGInputManager.playerInputs[playerIndex];
            if (input == null)
                return false;

            if (remappingKeyboardIndex >= 0)
                return false;

            if (!activeElement.TryGetComponent(out MGInputButtonConfigUILine line))
                return false;

            if (line.info != previousLineInfo)
            {
                previousLineInfo = line.info;
                switch (line.info)
                {
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.NA: subText.text = string.Empty; break;
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.Exit: subText.text = gameSpriteMapping.UpdateText(input, exitSubText); break;
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.KeyboardEdit: subText.text = gameSpriteMapping.UpdateText(input, keyboardConfigSubText); break;
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.Reset: subText.text = gameSpriteMapping.UpdateText(input, resetSubText); break;
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.KeyboardOnly: subText.text = gameSpriteMapping.UpdateText(input, keyboardOnly); return false;
                    case MGInputButtonConfigUILine.BtnConfigLineInfo.Test: subText.text = gameSpriteMapping.UpdateText(input, testSubText); return false;

                }
            }

            bool onConfirm = input.TestInput(input.confirmInput) == MGInputState.Pressed;
            bool onCancel = input.TestInput(input.cancelInput) == MGInputState.Pressed;

            switch (line.info)
            {
                case MGInputButtonConfigUILine.BtnConfigLineInfo.Test:
                    return false;
                case MGInputButtonConfigUILine.BtnConfigLineInfo.KeyboardOnly: return false;
                case MGInputButtonConfigUILine.BtnConfigLineInfo.Exit:
                    if (onConfirm || onCancel)
                    {
                        OnRequestExitEvent.Invoke();
                        return true;
                    }
                    return false;
                case MGInputButtonConfigUILine.BtnConfigLineInfo.Reset:
                    if (onConfirm)
                    {
                        input.SetDefaultIDs();
                        foreach (var _l in lines)
                            _l.SetInfo(input, gameSpriteMapping.spriteMapper);
                        return true;
                    }
                    return false;
                case MGInputButtonConfigUILine.BtnConfigLineInfo.KeyboardEdit:
                    if (onConfirm)
                    {
                        input.ClearKeys();
                        foreach (var _l in lines)
                            _l.SetInfo(input, gameSpriteMapping.spriteMapper);

                        remappingKeyboardIndex = 0;
                        usedKeys.Clear();
                        usedKeys.AddRange(keysToIgnore);
                        SelectNewElement(lines[0]);
                        Locked = true;

                    }
                    return false;
            }

            var btnList = input.mainInputSet.ButtonList;

            bool updateMade = false;
            if (input.device is Keyboard keyboard)
            {
                foreach (var btn in btnList)
                {
                    foreach (var k in btn.key)
                    {
                        if (keyboard[k].wasPressedThisFrame)
                        {
                            input.SetNewKey(line.inputID, k);
                            updateMade = true;
                            break;
                        }
                    }
                }
            }
            else if (input.device is Gamepad gamepad)
            {

                foreach (var btn in btnList)
                {
                    foreach (var gpb in btn.gpButton)
                    {
                        if (gamepad[gpb].wasPressedThisFrame)
                        {
                            input.SetNewGamepadButton(line.inputID, gpb);
                            updateMade = true;
                            break;
                        }
                    }
                }
            }

            if (updateMade)
            {
                foreach (var l in lines)
                    l.SetInfo(input, gameSpriteMapping.spriteMapper);
            }

            return base.UpdateFromBase(msBase);
        }

        public void DetermineWindows()
        {
            gameObject.SetActive(MGInputManager.playerInputs[playerIndex] != null);
        }
    }
}