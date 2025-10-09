using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData
{
    //[CreateAssetMenu(menuName = "Scriptable Value/Display String Array ScriptableValue")]
    public class IntListScriptableValue<T> :  IntScriptableValue
    {
        public bool loop;

        [SerializeField]
        protected T[] indexValues;

        public T IndexedValue => indexValues[Value];

        public T UpdateStringValues(params T[] inComingValues)
        {
            indexValues = inComingValues;
            Value = _value;
            return IndexedValue;
        }

        public override int Value {
            get
            {
                return base.Value;
            }
            set
            {
                if (value < 0)
                    value = loop ? indexValues.Length - 1 : 0;
                else if (value >= indexValues.Length)
                    value = loop ? 0 : indexValues.Length - 1;

                base.Value = value;
            }
        }
    }
}