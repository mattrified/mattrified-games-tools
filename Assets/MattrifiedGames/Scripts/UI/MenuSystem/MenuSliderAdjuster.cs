using MattrifiedGames.SVData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MattrifiedGames.MenuSystem
{
    [Tooltip("Behaviour for allowing easier adjustment of a slider with persistent data with a MenuElement.")]
    [RequireComponent(typeof(Slider))]
    public class MenuSliderAdjuster : ValidatedMonoBehaviour
    {
        [SerializeField, Tooltip("The Slider reference.")]
        protected Slider _slider;

        [SerializeField(), Min(0f), Tooltip("The amount the slider is adjusted by default")]
        protected float _adjustmentValue;

        [SerializeField(), Tooltip("The float value adjusted by the slider.")]
        protected FloatScriptableValue _scriptableFloatValue;

        [Header("Text Display")] 
        [SerializeField, Tooltip("Is a display text assigned.")]
        protected bool _hasText;

        [SerializeField(), Tooltip("The display text Component that will display the slider value.")]
        protected TMP_Text _displayText;

        [SerializeField(), Tooltip("The formatted text for the display.  Cannot have more than 0 variables.")]
        protected string _displayTextFormat;

        public override void OnValidate()
        {
            if (_slider == null)
            {
                _slider = GetComponent<Slider>();
            }

            Valid = _slider != null && _scriptableFloatValue != null;

            _hasText = _displayText != null;
        }

        private void Awake()
        {
            if (!TestValidity())
            {
                return;
            }

            _slider.onValueChanged.AddListener(OnSliderUpdated);
            _slider.value = _scriptableFloatValue.Value;
        }

        private void OnDestroy()
        {
            if (!Valid && _slider)
                return;

            _slider.onValueChanged.RemoveListener(OnSliderUpdated);
        }

        private void OnEnable()
        {
            if (!Valid)
                return;

            _slider.value = _scriptableFloatValue.Value;
        }

        /// <summary>
        /// Increases the slider by the default adjustment value.
        /// </summary>
        public void IncreaseValue()
        {
            AdjustValue(_adjustmentValue);
        }

        /// <summary>
        /// Decreases the slider by the default adjustment value.
        /// </summary>
        public void DecreaseValue()
        {
            AdjustValue(-_adjustmentValue);
        }

        /// <summary>
        /// Adjusts the slider by a specified value.
        /// </summary>
        /// <param name="adjustmentValue">The specified value to adjust the slider.</param>
        public void AdjustValue(float adjustmentValue)
        {
            if (!Valid)
                return;

            _slider.value += adjustmentValue;
        }

        /// <summary>
        /// Triggered when the scriptable float value is adjusted and forces the slider to visually updated.
        /// </summary>
        /// <param name="value"></param>
        private void OnSliderUpdated(float value)
        {
            if (!Valid)
                return;

            _scriptableFloatValue.Value = value;

            if (_hasText)
            {
                _displayText.text = string.Format(_displayTextFormat, value);
            }
        }
    }
}