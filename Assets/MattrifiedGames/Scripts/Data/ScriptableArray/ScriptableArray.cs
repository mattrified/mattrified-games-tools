using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattrifiedGames.SVData.Arrays
{
    public abstract class ScriptableArrayBaseUntyped : ScriptableObject
    {
        public abstract string[] CreateStringArray();
    }

    public abstract class ScriptableArrayBase<T> : ScriptableArrayBaseUntyped
    {
        [SerializeField()]
        protected T[] values;

        public int Length
        {
            get
            {
                return values.Length;
            }
        }

        public override string[] CreateStringArray()
        {
            string[] array = new string[values.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = GetItemString(i);
            }
            return array;
        }

        public abstract string GetItemString(int index);

        public T this[int index]
        {
            get
            {
                return MattrifiedGames.Utility.StaticHelpers.GetItemFromArraySafe(index, values);
            }

            set
            {
                if (index < 0 || index >= Length)
                    return;

                values[index] = value;
            }
        }
    }
}