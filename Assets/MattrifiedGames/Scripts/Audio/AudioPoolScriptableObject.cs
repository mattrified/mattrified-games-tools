using MattrifiedGames.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Pool")]
public class AudioPoolScriptableObject : MattrifiedGames.SVData.Arrays.ScriptableArrayBase<int>
{
    [SerializeField()]
    int size;

    [System.NonSerialized()]
    bool initialized = false;

    [System.NonSerialized()]
    int index = 0;

    [System.NonSerialized()]
    GameObject gop;

    [System.NonSerialized()]
    AudioSource[] audioSources;

    public override string GetItemString(int index)
    {
        return index.ToString();
    }

    public AudioSource GetNext()
    {
        if (!initialized)
        {
            gop = new GameObject(this.name + ":  Audio Pool Set");
            this.values = new int[size];
            audioSources = new AudioSource[size];
            for (int i = 0; i < size; i++)
            {
                values[i] = i;
                GameObject go = new GameObject(this.name + "AudioSource " + i);
                go.transform.SetParent(gop.transform);
                audioSources[i] = go.AddComponent<AudioSource>();
                
            }
            DontDestroyOnLoad(gop);
            initialized = true;
        }
        return audioSources[index++ % size];
    }

    private void OnDestroy()
    {
        if (gop != null)
        {
            GameObject.Destroy(gop);
        }
        gop = null;
        this.values = null;
        audioSources = null;
        initialized = false;
    }

#if UNITY_EDITOR

    [ContextMenu("Populate Clips")]
    public void PopClips()
    {
        var clips = MattrifiedGames.Utility.AssetFinder.LoadAllAssetOfType<AudioClipScriptableObjectBase>();
        foreach (var clip in clips)
        {
            clip.pool = this;
            UnityEditor.EditorUtility.SetDirty(clip);
        }

    }

#endif
}