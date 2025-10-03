using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppStateTransitions
{
    private enum TransitionType
    {
        CurrentOutNextIn,
        NextInCurrentOut
    }
    private IAppState _currentState;

    private T GetAppStateFromLoadedScene<T>(string sceneId) where T : ISceneTransitionable
    {
        return SceneManager.GetSceneByName(sceneId)
            .GetRootGameObjects()
            .Select(obj => obj.GetComponent<T>()).First(t => t != null);
    }

    private IEnumerator TransitionTo(AppState newState, TransitionType transitionType = TransitionType.CurrentOutNextIn)
    {
        string newStateName = newState.ToString();
        AsyncOperation newAppStateOp = SceneManager.LoadSceneAsync(newStateName, LoadSceneMode.Additive);
        Debug.Assert(newAppStateOp != null);
        
        if(transitionType == TransitionType.CurrentOutNextIn)
        {
            yield return _currentState.TransitionOut();
        }
        
        yield return new WaitUntil(() => newAppStateOp.isDone);
        IAppState newAppState = GetAppStateFromLoadedScene<IAppState>(newStateName);
        newAppState.OnLoad();
        IAppState stateBefore = _currentState;
        _currentState = newAppState;
        
        yield return _currentState.TransitionIn();
        if (transitionType == TransitionType.NextInCurrentOut)
        {
            yield return _currentState.TransitionOut();
        }
        
        stateBefore.OnUnload();
        SceneManager.UnloadSceneAsync(stateBefore.AppStateName);
    }
    
    public IEnumerator StartGame()
    {
        yield return TransitionTo(AppState.LoadingScreen);
        yield return TransitionTo(AppState.Game, TransitionType.NextInCurrentOut);
    }

    public IAppState CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }
}