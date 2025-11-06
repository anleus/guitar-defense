using System.Collections;
using Events;
using Models.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Core
{
    public class SceneController : MonoBehaviour
    {
        private SceneId currentScene = SceneId.Base;
        private bool isLoading;

        private IEnumerator Start()
        {
            PlayerPrefs.DeleteAll();
            yield return new WaitForSeconds(0.2f);

            StartCoroutine(HasDefaultSettings()
                ? LoadSceneAdditive(SceneId.MainMenu)
                : LoadSceneAdditive(SceneId.InitialSettings));
        }

        private void HandleSceneChangeRequested(SceneId newSceneId)
        {
            if (isLoading || newSceneId == currentScene) return;

            StartCoroutine(SwitchScene(newSceneId));
        }

        private IEnumerator SwitchScene(SceneId newSceneId)
        {
            isLoading = true;
            
            if (currentScene != SceneId.Base)
            {
                var previousSceneName = SceneMap.Names[currentScene];
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(previousSceneName);
                if (unloadOp != null)
                    yield return unloadOp;
            }

            yield return LoadSceneAdditive(newSceneId);

            isLoading = false;
        }

        private IEnumerator LoadSceneAdditive(SceneId sceneId)
        {
            var sceneName = SceneMap.Names[sceneId];

            if (!Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.LogError($"Scene '{sceneName}' is not in Build Settings!");
                yield break;
            }

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOp == null)
            {
                Debug.LogError($"Failed to load scene '{sceneName}'");
                yield break;
            }

            while (!loadOp.isDone) yield return null;

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            currentScene = sceneId;
        }

        private bool HasDefaultSettings()
        {
            return PlayerPrefs.HasKey("deviceName");
        }

        private void OnEnable()
        {
            SceneEvents.OnSceneChangeRequested += HandleSceneChangeRequested;
        }

        private void OnDisable()
        {
            SceneEvents.OnSceneChangeRequested -= HandleSceneChangeRequested;
        }
    }
}