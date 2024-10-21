using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    private IEnumerator Start()
    {
        AsyncOperation loadLoadingScreen = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadLoadingScreen.isDone);
        
        AsyncOperation loadGame = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        yield return new WaitUntil(() => loadGame.isDone);
        yield return new WaitForSeconds(2f);
        SceneManager.UnloadSceneAsync("SplashScreen");
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
}
