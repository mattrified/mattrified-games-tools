using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/String")]
    public class StringScriptableValue : ScriptableValue<string, UnityStringEvent>
    {
        public override void CopyTo(ref string inValue)
        {
            if (assigned)
                AssignDefault();

            inValue = _value;
        }

        public override void Load(string s)
        {
            Value = s;
        }

        public bool ValueEquals(ref string compareString)
        {
            return _value.Equals(compareString);
        }
    }

    [System.Serializable()]
    public class UnityStringEvent : UnityEvent<string> { }
}