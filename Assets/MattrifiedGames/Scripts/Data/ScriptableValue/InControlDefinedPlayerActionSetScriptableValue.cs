#if USING_INCONTROL
using UnityEngine;
using UnityEngine.Events;
using MattrifiedGames.InputControl;
using InControl;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/InControlDefinedPlayerActionSet")]
    public class InControlDefinedPlayerActionSetScriptableValue : ScriptableValue<InControlDefinition.InControlDefinedPlayerActionSet, InControlDefinedPlayerActionSetEvent>
    {
        [SerializeField()]
        private InControlDefinitionScriptableValue definition;

        [SerializeField()]
        private EventSO.EventSO onDisconnectEvent;

        [SerializeField()]
        private GameObject disconnectScreenGO;

        [SerializeField()]
        private GameObjectScriptableValue disconnectMenuObject;

        /// <summary>
        /// Gets the current player action value OR creates a new one with the provided dictionary
        /// </summary>
        /// <param name="defaultInputDefinition"></param>
        /// <returns>The existing or newly creating PlayerActionSet</returns>
        internal InControlDefinition.InControlDefinedPlayerActionSet GetOrCreate()
        {
            if (!assigned)
                Value = definition.Value.CreatePlayerActionSet(null);

            return Value;
        }

        private void OnEnable()
        {
            InputManager.OnDeviceDetached += InputManager_OnDeviceDetached;
        }

        private void InputManager_OnDeviceDetached(InputDevice obj)
        {
            if (assigned)
            {
                if (Value.Device == obj)
                {
                    if (disconnectMenuObject.Value == null)
                    {
                        disconnectMenuObject.Value = Instantiate(disconnectScreenGO);
                        DontDestroyOnLoad(disconnectMenuObject.Value);
                    }
                    onDisconnectEvent.Raise();
                }
            }
        }

        private void OnDisable()
        {
            InputManager.OnDeviceDetached -= InputManager_OnDeviceDetached;
        }

        /// <summary>
        /// This method is empty because if a value isn't assigned, the default is null and assigned should remain false.
        /// </summary>
        protected override void AssignDefault()
        {
        }

        public override InControlDefinition.InControlDefinedPlayerActionSet Value
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
                        _value.Destroy();
                    }
                }
                _value = value;

                assigned = _value != null;

                if (assigned)
                    onValueSetEvent.Invoke(_value);
            }
        }

        public void RemoveValue()
        {
            Value = null;
            assigned = false;
        }
    }

    public class InControlDefinedPlayerActionSetEvent : UnityEvent<InControlDefinition.InControlDefinedPlayerActionSet> { }
}
#endif