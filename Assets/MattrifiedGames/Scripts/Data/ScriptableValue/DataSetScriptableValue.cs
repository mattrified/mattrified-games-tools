using MattrifiedGames.Utility;
using UnityEngine;

namespace MattrifiedGames.SVData
{
    public abstract class DataSetScriptableValue<T> : IntScriptableValue
    {
        [SerializeField()]
        T[] data;

        public T Current
        {
            get
            {
                return data[Value];
            }
        }

        public int Length { get { return data.Length; } }

        public void Loop(int loopValue)
        {
            Value = StaticHelpers.SafeModolu(Value + loopValue, Length);
        }
    }
}