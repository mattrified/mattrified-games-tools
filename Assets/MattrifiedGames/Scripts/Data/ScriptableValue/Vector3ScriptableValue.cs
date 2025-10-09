using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Vector 3")]
    public class Vector3ScriptableValue : ScriptableValue<Vector3, UnityVector3Event>
    {
        public override void CopyTo(ref Vector3 inValue)
        {
            if (!assigned)
                AssignDefault();

            inValue.x = _value.x;
            inValue.y = _value.y;
            inValue.z = _value.z;
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
                }
            }
        }

        public float Z
        {
            get
            {
                if (assigned)
                {
                    return _value.z;
                }
                else
                {
                    return Value.z;
                }
            }

            set
            {
                if (assigned)
                {
                    _value.z = value;
                }
                else
                {
                    AssignDefault();
                    _value.z = value;
                }
            }
        }
    }

    [System.Serializable()]
    public class UnityVector3Event : UnityEvent<Vector3> { }
}