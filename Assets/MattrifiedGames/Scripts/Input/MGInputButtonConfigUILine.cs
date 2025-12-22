using MattrifiedGames.MenuSystem;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace MattrifiedGames.ButtonConfig
{
    public class MGInputButtonConfigUILine : MenuElementBase
    {
        public string inputID;

        public TextMeshProUGUI headerText;
        public TextMeshProUGUI spriteText;

        public BtnConfigLineInfo info;

        static readonly Color highlightColor = Color.white;
        static readonly Color unhighlightColor = Color.gray;

        public enum BtnConfigLineInfo
        {
            NA = 0,
            KeyboardOnly = 1,
            KeyboardEdit = 2,
            Reset = 3,
            Exit = 4,
            Test = 5,

        }
        public virtual void OnValidate()
        {
            inputID = name;
            headerText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            spriteText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        protected override void Start()
        {
            base.Start();
            headerText.color = unhighlightColor;
        }

        public override void OnHighlight()
        {
            headerText.color = highlightColor;
            base.OnHighlight();
        }

        public override void OnUnhighlight()
        {
            headerText.color = unhighlightColor;
            base.OnUnhighlight();
        }

        internal void SetInfo(MGInput input, MGInputSpriteMapping spriteMapping)
        {
            if (info >= BtnConfigLineInfo.KeyboardEdit)
            {
                return;
            }

            string s = string.Empty;
            if (input.mainInputSet.TryAndFind(inputID, out MGInputInfo result))
            {
                if (input.device is Keyboard)
                {
                    foreach (Key k in result.key)
                    {
                        s += spriteMapping[k] + "   ";
                    }
                }
                else
                {
                    foreach (GamepadButton btn in result.gpButton)
                    {
                        s += spriteMapping[btn] + "   ";
                    }
                }
            }
            spriteText.text = s;
        }
    }
}