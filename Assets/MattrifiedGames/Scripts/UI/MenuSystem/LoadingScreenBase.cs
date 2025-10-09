using MattrifiedGames.SVData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MattrifiedGames.UI
{
    public abstract class LoadingScreenBase: MonoBehaviour
    {
        protected AsyncOperation currentOperation;

        protected string sceneToLoad;
        
        [SerializeField()]
        protected float fadeInTime = 1f;

        [SerializeField()]
        protected float loadDelay = 1f;

        [SerializeField()]
        protected float fadeOutTime = 1f;

        protected float fadeTime;
        protected float loadTime;

        protected LoadState state = LoadState.Done;

        public enum LoadState
        {
            FadeIn = 0,
            Loading = 1,
            FadeOut = 2,
            Done = 3,
        }
        
        public virtual void LoadScene(string currentScene)
        {
            sceneToLoad = currentScene;
            gameObject.SetActive(true);
        }

        protected virtual void OnEnable()
        {
            state = LoadState.FadeIn;

            fadeTime = 0f;
            loadTime = 0f;

            DontDestroyOnLoad(this.gameObject);

            OnEnableLoadScreen();
        }

        protected abstract void OnEnableLoadScreen();

        protected virtual void OnDisable()
        {
        }

        protected abstract void OnUpdate();

        [SerializeField()]
        protected bool destroyOnFinish;

        protected abstract void OnFadeIn();
        protected abstract void OnFadeInComplete();
        protected abstract void OnBeginFadeOut();
        protected abstract void OnFadeOut();
        protected abstract void OnLoading();
        protected abstract void OnLoadComplete();

        protected void Update()
        {
            OnUpdate();

            switch (state)
            {
                case LoadState.FadeIn:
                    fadeTime += Time.deltaTime;
                    OnFadeIn();
                    if (fadeTime >= fadeInTime)
                    {
                        state = LoadState.Loading;
                        currentOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad);
                        OnFadeInComplete();
                    }
                    
                    break;
                case LoadState.Loading:
                    loadTime += Time.deltaTime;
                    if (currentOperation.progress >= 1f && loadTime >= loadDelay)
                    {
                        fadeTime = 0f;

                        OnBeginFadeOut();

                        state = LoadState.FadeOut;
                    }
                    else
                    {
                        OnLoading();
                    }
                    break;
                case LoadState.FadeOut:
                    fadeTime += Time.deltaTime;
                    OnFadeOut();
                    if (fadeTime >= fadeOutTime)
                    {
                        state = LoadState.Done;
                    }
                    break;
                case LoadState.Done:

                    OnLoadComplete();

                    if (!destroyOnFinish)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                    break;
            }
        }
    }
}