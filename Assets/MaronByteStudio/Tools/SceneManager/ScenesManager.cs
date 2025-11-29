using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MaronByteStudio
{
    public class ScenesManager : Singleton<ScenesManager>
    {
        protected override bool PersistBetweenScenes => true;
        /// <summary>
        /// Leave empty if not needed
        /// </summary>
        [SerializeField] private string FirstSceneToLoad;
        /// <summary>
        /// Use for loading canvas / image to cover the screen
        /// </summary>
        internal event Action<float> OnLoadingProgressChanged;
        /// <summary>
        /// Use to updater the loading progress bar
        /// </summary>
        internal event Action<bool> IsLoading;
        
        private float loadingProgress; // 0.0 to 1.0

        private void Start()
        {
            if (!string.IsNullOrEmpty(FirstSceneToLoad))
            {
                StartCoroutine(LoadSceneAsync(FirstSceneToLoad, null));
            }
        }

        internal void LoadScene(string sceneName, Action callback = null)
        {
            StartCoroutine(LoadSceneAsync(sceneName, callback));
        }
        
        private IEnumerator LoadSceneAsync(string sceneName, Action callback)
        {
            IsLoading?.Invoke(true);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            while (asyncLoad.progress < 0.9f) // Wait until the scene is almost fully loaded (progress reaches 0.9)
            {
                loadingProgress = Mathf.Clamp01(asyncLoad.progress / 0.9f) * 0.9f; // Map 0–0.9 to 0–90%
                OnLoadingProgressChanged?.Invoke(loadingProgress);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f); // Optionally wait here (e.g. for fade-out or delay)
            
            // Final phase (activation)
            loadingProgress = 0.95f;
            OnLoadingProgressChanged?.Invoke(loadingProgress);
            
            asyncLoad.allowSceneActivation = true; // Allow scene to activate
            while (!asyncLoad.isDone) // Wait until the scene is fully activated
            {
                yield return null;
            }
            
            loadingProgress = 1f;
            OnLoadingProgressChanged?.Invoke(loadingProgress);
            
            Debug.Log($"Scene '{sceneName}' loaded and active!");
            IsLoading?.Invoke(false);
            callback?.Invoke(); // ✅ Now the new scene is loaded and active — call your function
        }

        internal void UnLoadScene(string sceneName, Action callback = null)
        {
            StartCoroutine(UnLoadSceneAsync(sceneName, callback));
        }
        
        private IEnumerator UnLoadSceneAsync(string sceneName, Action callback)
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName); // Begin unloading
            if (unloadOperation == null)
            {
                Debug.LogWarning($"Scene '{sceneName}' could not be unloaded (maybe it’s not loaded?)");
                yield break;
            }
            while (!unloadOperation.isDone) // Wait until the scene is fully unloaded
            {
                yield return null;
            }
            Debug.Log($"Scene '{sceneName}' unloaded successfully.");
            callback?.Invoke();
        }
    }
}
