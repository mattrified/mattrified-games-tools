using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Clamped Int")]
    public class ClampedIntScriptableValue : ScriptableValue<ClampedInt, UnityClampedIntEvent>
    {
        public override void CopyTo(ref ClampedInt inValue)
        {
            if (!assigned)
                AssignDefault();

            inValue.SetMinMax(_value.Min, _value.Max);
            inValue.ClampedValue = _value.ClampedValue;
        }

        public int Min
        {
            get
            {
                if (assigned)
                {
                    return _value.Min;
                }
                else
                {
                    return Value.Min;
                }
            }

            set
            {
                if (!assigned)
                    AssignDefault();

                _value.Min = value;

                onValueSetEvent.Invoke(_value);
            }
        }

        public int Max
        {
            get
            {
                if (assigned)
                {
                    return _value.Max;
                }
                else
                {
                    return Value.Max;
                }
            }

            set
            {
                if (!assigned)
                    AssignDefault();

                _value.Max = value;

                onValueSetEvent.Invoke(_value);
            }
        }

        public int ClampedValue
        {
            get
            {
                if (assigned)
                {
                    return _value.ClampedValue;
                }
                else
                {
                    return Value.ClampedValue;
                }
            }

            set
            {
                if (!assigned)
                    AssignDefault();

                _value.ClampedValue = value;

                onValueSetEvent.Invoke(_value);
            }
        }

        const string SAVE_LOAD_FORMAT = "{0}|{1}|{2}";

        public override string Save()
        {
            return string.Format(SAVE_LOAD_FORMAT, Min, Max, Value);
        }

        public override void Load(string s)
        {
            var split = s.Split('|');

            ClampedInt value = new ClampedInt();
            value.SetMinMax(int.Parse(split[0]), int.Parse(split[1]));
            value.ClampedValue = int.Parse(split[2]);

            Value = value;
        }
    }

    [System.Serializable()]
    public class UnityClampedIntEvent : UnityEvent<ClampedInt> { }
}