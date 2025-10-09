using MattrifiedGames.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/MenuElementSFXPalette")]
public class MenuElementSFXPalette : ScriptableObject
{
    [SerializeField()]
    private AudioClipScriptableObjectBase openSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase highlightSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase unhighlightSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase closeSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase confirmSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase cancelSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase leftSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase rightSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase upSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase downSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase leftPressSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase rightPressSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase upPressSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase downPressSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase[] indexedSFX;

    [SerializeField()]
    private AudioClipScriptableObjectBase errorSFX;

    internal void OnLeftSFX(bool press) { PlaySFX(press ? leftPressSFX : leftSFX); }

    

    internal void OnRightSFX(bool press) { PlaySFX(press ? rightPressSFX : rightSFX); }
    internal void OnUpSFX(bool press) { PlaySFX(press ? upPressSFX : upSFX); }
    internal void OnDownSFX(bool press) { PlaySFX(press ? downPressSFX: downSFX); }



    internal void OnHighlightSFX() { PlaySFX(highlightSFX); }
    internal void OnUnhighlightSFX() { PlaySFX(unhighlightSFX); }
    internal void OnOpenSFX() { PlaySFX(openSFX); }
    internal void OnCloseSFX() { PlaySFX(closeSFX); }
    internal void OnConfirmSFX() { PlaySFX(confirmSFX); }
    internal void OnCancelSFX() { PlaySFX(cancelSFX); }

    internal void OnErrorSFX() { PlaySFX(errorSFX); }

    internal void OnIndexedSFX(int index)
    {
        if (index < indexedSFX.Length)
        {
            PlaySFX(indexedSFX[index]);
        }
    }

    void PlaySFX(AudioClipScriptableObjectBase sfx)
    {
        if (sfx)
            sfx.PlayFromPool();
    }
}
