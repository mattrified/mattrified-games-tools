using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Bool")]
    public class BoolScriptableValue : ScriptableValue<bool, UnityBoolEvent>
    {
        public override void CopyTo(ref bool inValue)
        {
            if (assigned)
                AssignDefault();

            inValue = _value;
        }

        public override void Load(string s)
        {
            Value = bool.Parse(s);
        }

        [ContextMenu("Toggle")]
        public void Toggle()
        {
            Value = !Value;
        }
    }

    [System.Serializable()]
    public class UnityBoolEvent : UnityEvent<bool> { }
}