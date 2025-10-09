using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace MattrifiedGames.Audio
{
    [CreateAssetMenu(menuName = "Audio/Audio Clip SO")]
    public class AudioClipScriptableObject : AudioClipScriptableObjectBase
    {
        

        public AudioClip clip;
        public AudioMixerGroup mixerGroup;
        public bool loop;

        public FloatRange pitchRange = new FloatRange(1, 1);

        [Range(0, 1)]
        public float volume = 1;

        [Range(0, 1)]
        public float spatialBlend = 0;

        public override AudioClip GetClip(int index = 0)
        {
            return clip;
        }

        public override void Play(AudioSource inSource, float time, Vector3 position)
        {
            // This means that the requested time we want to play the audio is when the clip is over.
            // We don't even bother playing it in this case because it's already over.
            if (time >= clip.length)
                return;

            inSource.Stop();

            inSource.clip = clip;
            inSource.outputAudioMixerGroup = mixerGroup;

            inSource.volume = volume;
            inSource.spatialBlend = spatialBlend;

            inSource.loop = loop;

            inSource.time = time;

            inSource.pitch = pitchRange.Random;

            inSource.transform.position = position;

            inSource.Play();
        }

        public override void PlayFromPool()
        {
            PlaySimple(pool.GetNext());
        }

        public override void PlaySimple(AudioSource inSource)
        {
            Play(inSource, 0f, Vector3.zero);
        }
    }
}