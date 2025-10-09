using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Color")]
    public class ColorScriptableValue : ScriptableValue<Color, UnityColorEvent>
    {
        public void SetColor(UnityEngine.UI.Graphic graphic)
        {
            graphic.color = Value;
        }

        public override void CopyTo(ref Color inValue)
        {
            if (!assigned)
                AssignDefault();

            inValue.r = _value.r;
            inValue.g = _value.g;
            inValue.b = _value.b;
            inValue.a = _value.a;
        }

        public float R
        {
            get
            {
                if (assigned)
                {
                    return _value.r;
                }
                else
                {
                    return Value.r;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.r = value;
                }
                else
                {
                    AssignDefault();
                    _value.r = value;
                }
            }
        }

        public float G
        {
            get
            {
                if (assigned)
                {
                    return _value.g;
                }
                else
                {
                    return Value.g;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.g = value;
                }
                else
                {
                    AssignDefault();
                    _value.g = value;
                }
            }
        }

        public float B
        {
            get
            {
                if (assigned)
                {
                    return _value.b;
                }
                else
                {
                    return Value.b;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.b = value;
                }
                else
                {
                    AssignDefault();
                    _value.b = value;
                }
            }
        }

        public float A
        {
            get
            {
                if (assigned)
                {
                    return _value.a;
                }
                else
                {
                    return Value.a;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.a = value;
                }
                else
                {
                    AssignDefault();
                    _value.a = value;
                }
            }
        }
    }

    [System.Serializable()]
    public class UnityColorEvent : UnityEvent<Color> { }
}
