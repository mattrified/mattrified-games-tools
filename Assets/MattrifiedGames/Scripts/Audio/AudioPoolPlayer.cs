using MattrifiedGames.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPoolPlayer : MonoBehaviour
{
    public AudioClipScriptableObjectBase clip;
    public AudioPoolScriptableObject pool;

    public void PlaySimple()
    {
        clip.PlaySimple(pool.GetNext());
    }
}
