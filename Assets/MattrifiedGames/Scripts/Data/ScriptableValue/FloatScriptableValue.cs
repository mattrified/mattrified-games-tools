using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Float")]
    public class FloatScriptableValue : ScriptableValue<float, UnityFloatEvent>
    {
        public override void CopyTo(ref float inValue)
        {
            if (assigned)
                AssignDefault();

            inValue = _value;
        }

        public override void Load(string s)
        {
            Value = float.Parse(s);
        }
    }

    [System.Serializable()]
    public class UnityFloatEvent : UnityEvent<float> { }

}