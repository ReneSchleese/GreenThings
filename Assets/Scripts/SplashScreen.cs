using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private void Start()
    {
        AsyncOperation loadSceneAsync = SceneManager.LoadSceneAsync("LoadingScreen");
        loadSceneAsync.completed += LoadSceneAsyncOnCompleted;
    }

    private static void LoadSceneAsyncOnCompleted(AsyncOperation obj)
    {
        SceneManager.LoadSceneAsync("Game");
    }
}
