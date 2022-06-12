using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure
{
    public class Startup : MonoBehaviour
    {
        public SceneAsset InitialScene;

        private void Awake()
        {
            StartCoroutine(LoadScene(InitialScene.name));
        }

        private IEnumerator LoadScene(string nextScene)
        {
            if (SceneManager.GetActiveScene().name == nextScene)
            {
                yield break;
            }

            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            while (!waitNextScene.isDone)
                yield return null;
        }
    }
}