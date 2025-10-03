using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions
{
    public IEnumerator StartGame()
    {
        Debug.Log("Start Game");
        // tell current scene/holder about pending transition
        
        // load loading screen and wait
        
        // potentially setup loading screen
        
        // unload current scene
        
        // tell current scene/holder (loading screen) about pending transition

        // load game and wait
        
        // setup game
        
        // unload current scene
        
        AsyncOperation loadingScreenOperation = SceneManager.LoadSceneAsync("LoadingScreen", LoadSceneMode.Additive);
        Debug.Assert(loadingScreenOperation != null);
        yield return new WaitUntil(() => loadingScreenOperation.isDone);
        
        /*AsyncOperation loadGame = SceneManager.LoadSceneAsync("Game", LoadSceneMode.Additive);
        LoadingScreen loadingScreen = FindFirstObjectByType<LoadingScreen>();
        yield return loadingScreen.FadeIn();
        _canvas.gameObject.SetActive(false);
        yield return new WaitUntil(() => loadGame.isDone);
        UserInterface.Instance.CanvasGroupAlpha = 0f;
        yield return new WaitUntil(() => loadingScreen.EnoughTimeHasPassed);

        UserInterface.Instance.CanvasGroupAlpha = 1f;
        yield return loadingScreen.FadeOut();
        SceneManager.UnloadSceneAsync("SplashScreen");
        SceneManager.UnloadSceneAsync("LoadingScreen");*/
    }
}