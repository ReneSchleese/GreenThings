using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions
{
    private IAppState _currentState;

    private T GetTransitionableFromLoadedScene<T>(string sceneId) where T : ISceneTransitionable
    {
        return SceneManager.GetSceneByName(sceneId)
            .GetRootGameObjects()
            .Select(obj => obj.GetComponent<T>()).First(t => t != null);
    }
    
    public IEnumerator StartGame()
    {
        Debug.Log("Start Game");
        
        yield return _currentState.PrepareBeingTransitionedFrom();

        const string loadingScreenId = nameof(AppState.LoadingScreen);
        AsyncOperation loadLoadingOp = SceneManager.LoadSceneAsync(loadingScreenId, LoadSceneMode.Additive);
        Debug.Assert(loadLoadingOp != null);
        yield return new WaitUntil(() => loadLoadingOp.isDone);
        LoadingScreen loadingScreen = GetTransitionableFromLoadedScene<LoadingScreen>(loadingScreenId);
        loadingScreen.OnLoadComplete();
        
        _currentState.OnUnload();
        SceneManager.UnloadSceneAsync(nameof(_currentState.Id));
        
        _currentState = loadingScreen;
        yield return _currentState.PrepareBeingTransitionedFrom();

        const string gameId = nameof(AppState.Game);
        AsyncOperation loadGameOp = SceneManager.LoadSceneAsync(gameId, LoadSceneMode.Additive);
        Debug.Assert(loadGameOp != null);
        yield return new WaitUntil(() => loadGameOp.isDone);
        Game game = GetTransitionableFromLoadedScene<Game>(gameId);
        game.OnLoadComplete();
        
        // setup game

        // unload current scene


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