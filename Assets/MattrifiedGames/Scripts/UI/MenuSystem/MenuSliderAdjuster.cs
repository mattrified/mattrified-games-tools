using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MenuSliderAdjuster : MonoBehaviour
{
    Slider slider;

    [SerializeField()]
    float adjustmentValue;

    [SerializeField()]
    FloatScriptableValue scriptableFloatValue;

    bool hasScriptableFloatValue;

    private void Awake()
    {
        slider = GetComponent<Slider>();

        hasScriptableFloatValue = scriptableFloatValue != null;
        if (hasScriptableFloatValue)
        {
            slider.value = scriptableFloatValue.Value;
            scriptableFloatValue.AddOnValueChangedEvent(SetValue);
        }
    }

    private void OnDestroy()
    {
        if (hasScriptableFloatValue)
        {
            scriptableFloatValue.RemoveOnSetEvent(SetValue);
        }
    }

    private void OnEnable()
    {
        hasScriptableFloatValue = scriptableFloatValue != null;
        if (hasScriptableFloatValue)
            slider.value = scriptableFloatValue.Value;
    }

    public void IncreaseValue(bool adjust)
    {
        if (adjust)
        {
            slider.value += adjustmentValue;
            if (hasScriptableFloatValue)
            {
                scriptableFloatValue.Value = slider.value;
            }
        }
    }

    public void DecreaseValue(bool adjust)
    {
        if (adjust)
        {
            slider.value -= adjustmentValue;
            if (hasScriptableFloatValue)
            {
                scriptableFloatValue.Value = slider.value;
            }
        }
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
