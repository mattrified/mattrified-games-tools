#if USING_INCONTROL

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MattrifiedGames.InputControl;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu()]
    public class InControlDefinitionScriptableValue : ScriptableValue<InControlDefinition, InControlDefinitionEvent>
    {
        [SerializeField()]
        InControlDefinedPlayerActionSetScriptableValue associatedController;

        public InControlDefinedPlayerActionSetScriptableValue AssociatedController
        {
            get
            {
                return associatedController;
            }
        }

        protected override void AssignDefault()
        {
            _value = Instantiate(defaultValue);
            assigned = true;
        }

        public override InControlDefinition Value
        {
            get
            {
                return base.Value;
            }

            set
            {
                if (_value != value)
                {
                    if (assigned)
                    {
                        // unsure if this is necessary
                        Destroy(_value);
                    }
                }

                _value = value;
                assigned = _value != null;

                if (assigned)
                    onValueSetEvent.Invoke(_value);
            }
        }

        public override void Load(string s)
        {
            if (!assigned)
                AssignDefault();

            _value.PopulateFromXMLData(s);

            // Resets the controller if it was setup
            if (associatedController.Value != null)
                associatedController.Value = _value.CreatePlayerActionSet(associatedController.Value.Device);
        }

        public override string Save()
        {
            return Value.CreateXMLData();
        }

#if UNITY_EDITOR
        [ContextMenu("Select Value")]
        public void SelectValue()
        {
            UnityEditor.Selection.activeObject = Value;
        }
#endif
    }

    [System.Serializable()]
    public class InControlDefinitionEvent : UnityEvent<InControlDefinition> { }
}
#endif