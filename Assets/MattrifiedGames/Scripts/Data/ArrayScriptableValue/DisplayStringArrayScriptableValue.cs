using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Display String Array ScriptableValue")]
    public class DisplayStringArrayScriptableValue :  IntScriptableValue
    {
        public bool loop;

        [SerializeField]
        protected string[] stringValues;

        public string StringValue => stringValues[Value];

        public int? StringToIntValue
        {
            get
            {
                if (int.TryParse(stringValues[Value], out int result))
                {
                    return result;
                }
                return null;
            }
        }

        public string UpdateStringValues(params string[] inComingStringValues)
        {
            stringValues = inComingStringValues;
            Value = _value;
            return StringValue;
        }

        public override int Value {
            get
            {
                return base.Value;
            }
            set
            {
                if (value < 0)
                    value = loop ? stringValues.Length - 1 : 0;
                else if (value >= stringValues.Length)
                    value = loop ? 0 : stringValues.Length - 1;

                base.Value = value;
            }
        }
    }
}