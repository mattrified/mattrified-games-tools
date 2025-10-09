using UnityEngine.UI;

namespace MattrifiedGames.SVData
{
    [System.Serializable()]
    public class IntCondition
    {
        public int value;
        public bool equals = true;
        public UnityIntEvent intEvent;
    }

    public class IntSVBehaviourConditional : IntSVBehaviour
    {
        public IntCondition[] conditions;

        protected override void Awake()
        {
            base.Awake();
            scriptableValue.AddOnValueChangedEvent(TestConditions);
            TestConditions(scriptableValue.Value);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            scriptableValue.RemoveOnSetEvent(TestConditions);
        }

        void TestConditions(int value)
        {
            for (int i =0; i < conditions.Length; i++)
            {
                if ((conditions[i].value == value) == conditions[i].equals)
                {
                    conditions[i].intEvent.Invoke(value);
                }
            }
        }
    }


}