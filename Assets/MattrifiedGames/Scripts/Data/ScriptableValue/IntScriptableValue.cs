using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Int")]
    public class IntScriptableValue : ScriptableValue<int, UnityIntEvent>
    {
        public override void CopyTo(ref int inValue)
        {
            if (assigned)
                AssignDefault();

            inValue = _value;
        }

        public override void Load(string s)
        {
            Value = int.Parse(s);
        }

        public void Increment(int value)
        {
            Value += value;
        }


        [ContextMenu("Log")]
        public void Log()
        {
            Debug.Log(this.name + "'s value:  " + Value.ToString());
        }
    }


    [System.Serializable()]
    public class UnityIntEvent : UnityEvent<int> { }
}