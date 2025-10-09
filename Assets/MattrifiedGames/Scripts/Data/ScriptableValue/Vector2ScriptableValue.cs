using UnityEngine;
using System.Collections;
using UnityEngine.Events;
namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Vector 2")]
    public class Vector2ScriptableValue : ScriptableValue<Vector2, UnityVector2Event>
    {
        public override void CopyTo(ref Vector2 inValue)
        {
            if (!assigned)
                AssignDefault();

            inValue.x = _value.x;
            inValue.y = _value.y;
        }

        public float X
        {
            get
            {
                if (assigned)
                {
                    return _value.x;
                }
                else
                {
                    return Value.x;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.x = value;
                }
                else
                {
                    AssignDefault();
                    _value.x = value;
                    onValueSetEvent.Invoke(_value);
                }
            }
        }

        public float Y
        {
            get
            {
                if (assigned)
                {
                    return _value.y;
                }
                else
                {
                    return Value.y;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.y = value;
                }
                else
                {
                    AssignDefault();
                    _value.y = value;
                    onValueSetEvent.Invoke(_value);
                }
            }
        }

        const string SAVE_LOAD_FORMAT = "{0}|{1}";

        public override string Save()
        {
            return string.Format("{0}|{1}", X, Y);
        }

        public override void Load(string s)
        {
            var split = s.Split('|');

            Vector2 value = new Vector2(float.Parse(split[0]), float.Parse(split[1]));

            Value = value;
        }
    }

    [System.Serializable()]
    public class UnityVector2Event : UnityEvent<Vector2> { }
}