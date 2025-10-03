using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateTransitions
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
        yield return _currentState.PrepareBeingTransitionedFrom();

        const string loadingScreenId = nameof(AppState.LoadingScreen);
        AsyncOperation loadLoadingOp = SceneManager.LoadSceneAsync(loadingScreenId, LoadSceneMode.Additive);
        Debug.Assert(loadLoadingOp != null);
        yield return new WaitUntil(() => loadLoadingOp.isDone);
        LoadingScreen loadingScreen = GetTransitionableFromLoadedScene<LoadingScreen>(loadingScreenId);
        loadingScreen.OnLoad();
        yield return loadingScreen.PrepareBeingTransitionedTo();
        
        _currentState.OnUnload();
        SceneManager.UnloadSceneAsync(_currentState.AppStateName);
        _currentState = loadingScreen;

        const string gameId = nameof(AppState.Game);
        AsyncOperation loadGameOp = SceneManager.LoadSceneAsync(gameId, LoadSceneMode.Additive);
        Debug.Assert(loadGameOp != null);
        yield return new WaitUntil(() => loadGameOp.isDone);
        Game game = GetTransitionableFromLoadedScene<Game>(gameId);
        game.OnLoad();
        
        yield return _currentState.PrepareBeingTransitionedFrom();
        _currentState.OnUnload();
        SceneManager.UnloadSceneAsync(_currentState.AppStateName);
        _currentState = game;
    }

    public IAppState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}