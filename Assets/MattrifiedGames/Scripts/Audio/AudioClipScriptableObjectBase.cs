using UnityEngine;

namespace MattrifiedGames.Audio
{
    public abstract class AudioClipScriptableObjectBase : ScriptableObject
    {
        public AudioPoolScriptableObject pool;

        public abstract void PlayFromPool();

        public abstract void Play(AudioSource inSource, float time, Vector3 position);

        public abstract void PlaySimple(AudioSource inSource);

        public abstract AudioClip GetClip(int index = 0);
    }
}
