using UnityEngine;

namespace MattrifiedGames.Audio
{
    [CreateAssetMenu(menuName = "Audio/Audio Clip SO Set")]
    public class AudioClipScriptableObjectSet : AudioClipScriptableObjectBase
    {
        public AudioClipScriptableObject[] clips;

        public override AudioClip GetClip(int index = 0)
        {
            if (clips == null || clips.Length == 0 || index < 0)
                return null;

            var acso = clips[index % clips.Length];
            if (acso == null)
                return null;
            return acso.GetClip(index);
        }

        public override void Play(AudioSource inSource, float time, Vector3 position)
        {
            clips[Random.Range(0, clips.Length)].Play(inSource, time, position);
        }

        public override void PlayFromPool()
        {
            clips[Random.Range(0, clips.Length)].PlayFromPool();
        }

        public override void PlaySimple(AudioSource inSource)
        {
            clips[Random.Range(0, clips.Length)].Play(inSource, 0f, Vector3.zero);
        }
    }
}
