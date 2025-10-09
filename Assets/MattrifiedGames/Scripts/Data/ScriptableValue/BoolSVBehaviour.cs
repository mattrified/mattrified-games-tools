using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    public class BoolSVBehaviour : ScriptableValueBehaviour<BoolScriptableValue, bool, UnityBoolEvent>
    {
        public UnityEvent OnTrueEvent;
        public UnityEvent OnFalseEvent;

        public void InvokeTrueFalseEvents(bool value)
        {
            if (value)
                OnTrueEvent.Invoke();
            else
                OnFalseEvent.Invoke();
        }
    }
}