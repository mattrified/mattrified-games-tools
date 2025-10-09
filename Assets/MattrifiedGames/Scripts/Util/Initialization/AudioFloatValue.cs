using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace MattrifiedGames.SVData
{
    [CreateAssetMenu(menuName = "Scriptable Value/Audio Float")]
    public class AudioFloatValue : IntScriptableValue
    {
        public int min = 0, max = 100;

        public bool isGlobalAudio;
        public string mixerKey;

        public bool useLog;
        public AudioMixer mixer;

        protected override void AssignDefault()
        {
            base.AssignDefault();
            SetAudioValue();
        }

        void SetAudioValue()
        {
            if (isGlobalAudio)
            {
                AudioListener.volume = Mathf.InverseLerp(min, max, _value);
            }
            else
            {
                mixer.SetFloat(mixerKey, useLog ? Mathf.Log10(Mathf.Max(0.0001f, Mathf.InverseLerp(min, max, _value))) * 20 : _value);
            }
        }

        public void SetIntegerText(TextMeshProUGUI tmp)
        {
            tmp.text = Mathf.Round(Value * 100).ToString();
        }

        public override int Value { get => base.Value; set { base.Value = Mathf.Clamp(value, min, max); SetAudioValue(); } }
    }
}