using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateTransitions
{
    private enum TransitionType
    {
        CurrentOffThenNextTo,
        NextToThenCurrentOff
    }
    private IAppState _currentState;

    private T GetAppStateFromLoadedScene<T>(string sceneId) where T : ISceneTransitionable
    {
        return SceneManager.GetSceneByName(sceneId)
            .GetRootGameObjects()
            .Select(obj => obj.GetComponent<T>()).First(t => t != null);
    }

    private IEnumerator TransitionTo(AppState appState, TransitionType transitionType = TransitionType.CurrentOffThenNextTo)
    {
        string appStateName = appState.ToString();
        AsyncOperation newAppStateOp = SceneManager.LoadSceneAsync(appStateName, LoadSceneMode.Additive);
        Debug.Assert(newAppStateOp != null);
        
        if(transitionType == TransitionType.CurrentOffThenNextTo)
        {
            yield return _currentState.TransitionOff();
        }
        
        yield return new WaitUntil(() => newAppStateOp.isDone);
        IAppState newAppState = GetAppStateFromLoadedScene<IAppState>(appStateName);
        newAppState.OnLoad();
        IAppState stateBefore = _currentState;
        _currentState = newAppState;
        
        yield return _currentState.TransitionTo();
        if (transitionType == TransitionType.NextToThenCurrentOff)
        {
            yield return _currentState.TransitionOff();
        }
        
        stateBefore.OnUnload();
        SceneManager.UnloadSceneAsync(stateBefore.AppStateName);
    }
    
    public IEnumerator StartGame()
    {
        yield return TransitionTo(AppState.LoadingScreen);
        yield return TransitionTo(AppState.Game, TransitionType.NextToThenCurrentOff);
    }

    public IAppState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}