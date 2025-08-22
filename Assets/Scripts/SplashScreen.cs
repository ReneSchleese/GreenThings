using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    
    private IEnumerator Start()
    {
        AsyncOperation loadLoadingScreen = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadLoadingScreen.isDone);
        
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        LoadingScreen loadingScreen = FindFirstObjectByType<LoadingScreen>();
        yield return loadingScreen.FadeIn();
        _canvas.gameObject.SetActive(false);
        yield return new WaitUntil(() => loadGame.isDone);
        UserInterface.Instance.CanvasGroupAlpha = 0f;
        yield return new WaitUntil(() => loadingScreen.EnoughTimeHasPassed);

        UserInterface.Instance.CanvasGroupAlpha = 1f;
        yield return loadingScreen.FadeOut();
        SceneManager.UnloadSceneAsync("SplashScreen");
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
}
