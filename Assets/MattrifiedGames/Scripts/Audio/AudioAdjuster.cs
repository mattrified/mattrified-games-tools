using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class AudioAdjuster : MonoBehaviour
{
    public ClampedInt value;

    public string mixerParameter;
    public AudioMixer mixer;

    /*Use a slider value range of 0.0001 – 1
    instead of -80db – 0db(the 0.0001 is important, and stops the slider breaking at zero)
    When passing the slider value to the SetFloat function, convert it using Mathf.Log10(value) * 20;
    e.g.mixer.SetFloat(“MusicVol”, Mathf.Log10(sliderValue) * 20);*/

    public string valueText;
    public TextMeshProUGUI text;

    public UnityEvent<float> OnVolumeAdjusted;

    private void Awake()
    {
        text.text = string.Format(valueText, value.ClampedValue);
    }

    public void Adjust(int offset)
    {
        value.ClampedValue += offset;

        float vRange = Mathf.Lerp(0.0001f, 1f, value.Percent);

        mixer.SetFloat(mixerParameter, Mathf.Log10(vRange) * 20f);

        text.text = string.Format(valueText, value.ClampedValue);

        OnVolumeAdjusted.Invoke(value.Percent);
    }
}
