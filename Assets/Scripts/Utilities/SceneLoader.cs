using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;

/// <summary>
/// A static utility class for handling smooth, preloaded scene transitions.
/// It disables VR interactors during the load to prevent errors.
/// </summary>
public static class SceneLoader
{
    private static bool _isLoading = false;

    /// <summary>
    /// Loads a new scene using a smooth, preloading coroutine.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public static void LoadScene(string sceneName)
    {
        if (_isLoading)
        {
            Debug.LogWarning("[SceneLoader] A scene transition is already in progress. Ignoring request.");
            return;
        }

        // Use the CoreManager instance to start the coroutine, as static classes cannot.
        if (CoreManager.Instance != null)
        {
            CoreManager.Instance.StartCoroutine(LoadSceneRoutine(sceneName));
        }
        else
        {
            Debug.LogError("[SceneLoader] CoreManager is not available! Loading scene directly as a fallback.");
            SceneManager.LoadScene(sceneName);
        }
    }

    private static IEnumerator LoadSceneRoutine(string sceneName)
    {
        _isLoading = true;

        // 1. Disable interactors to prevent interaction during scene change
        List<IInteractor> interactors = new List<IInteractor>();
        GameObject ovrRig = CoreManager.Instance.GetRig();

        if (ovrRig != null)
        {
            interactors = ovrRig.GetComponentsInChildren<IInteractor>(true).ToList();
            foreach (var interactor in interactors)
            {
                if (interactor != null) interactor.Disable();
            }
        }

        // 2. Start loading the scene asynchronously but don't activate it yet
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = false;

            // Wait until the scene is 90% loaded
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // 3. Activate the new scene for an instant switch
            asyncLoad.allowSceneActivation = true;
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogError($"[SceneLoader] Failed to start loading scene: {sceneName}");
        }

        // 4. Re-enable the interactors
        foreach (var interactor in interactors)
        {
            if (interactor != null)
            {
                interactor.Enable();
            }
        }

        // 5. Reset the loading flag
        _isLoading = false;
    }
}