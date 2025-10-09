using MattrifiedGames.SVData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MattrifiedGames.Utility
{
    [CreateAssetMenu()]
    public class SceneManagementHelper : ScriptableObject
    {
        public UnityEvent OnAsyncLoadStart;
        public UnityFloatEvent OnAsyncLoadUpdate;
        public UnityEvent OnAsyncLoadEnd;

        public void LoadScene(int sceneIndex)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
        }

        public void LoadScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }

        public AsyncOperation LoadSceneAsync(int sceneIndex)
        {
            return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);
        }

        public AsyncOperation LoadSceneAsync(string sceneName)
        {
            return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        }

        public IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
        {
            OnAsyncLoadStart.Invoke();
            var operation = LoadSceneAsync(sceneIndex);
            while (operation.isDone)
            {
                OnAsyncLoadUpdate.Invoke(operation.progress);
                yield return null;
            }
            OnAsyncLoadEnd.Invoke();
        }

        public IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            OnAsyncLoadStart.Invoke();
            var operation = LoadSceneAsync(sceneName);
            while (operation.isDone)
            {
                OnAsyncLoadUpdate.Invoke(operation.progress);
                yield return null;
            }
            OnAsyncLoadEnd.Invoke();
        }

        public void ReloadScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        public void CloseApplication()
        {
            Application.Quit();
        }
    }
}