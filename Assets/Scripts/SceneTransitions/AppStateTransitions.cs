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

    public IEnumerator StartGame()
    {
        IsCurrentlyTransitioning = true;
        yield return TransitionTo(AppState.LoadingScreen);
        yield return TransitionTo(AppState.Game, TransitionType.NextInCurrentOut);
        IsCurrentlyTransitioning  = false;
    }

    public IEnumerator FromEntryPoint(AppState state)
    {
        IsCurrentlyTransitioning = true;
        Debug.Log($"{nameof(FromEntryPoint)} {state}");
        string stateName = state.ToString();
        IAppState appState = GetAppStateFromLoadedScene<IAppState>(stateName);
        yield return appState.OnLoad();
        CurrentState = appState;
        yield return CurrentState.TransitionIn();
        IsCurrentlyTransitioning = false;
    }

    private IEnumerator TransitionTo(AppState newState, TransitionType transitionType = TransitionType.CurrentOutNextIn)
    {
        string newStateName = newState.ToString();
        AsyncOperation newAppStateOp = SceneManager.LoadSceneAsync(newStateName, LoadSceneMode.Additive);
        Debug.Assert(newAppStateOp != null);
        
        if(transitionType == TransitionType.CurrentOutNextIn)
        {
            yield return CurrentState.TransitionOut();
        }
        
        yield return new WaitUntil(() => newAppStateOp.isDone);
        IAppState newAppState = GetAppStateFromLoadedScene<IAppState>(newStateName);
        yield return newAppState.OnLoad();
        IAppState stateBefore = CurrentState;
        CurrentState = newAppState;
        
        yield return CurrentState.TransitionIn();
        if (transitionType == TransitionType.NextInCurrentOut)
        {
            yield return stateBefore.TransitionOut();
        }
        
        stateBefore.OnUnload();
        SceneManager.UnloadSceneAsync(stateBefore.AppStateName);
    }

    private static T GetAppStateFromLoadedScene<T>(string sceneId) where T : IAppState
    {
        return SceneManager.GetSceneByName(sceneId)
            .GetRootGameObjects()
            .Select(obj => obj.GetComponent<T>()).First(t => t != null);
    }

    public IAppState CurrentState { get; private set; }
    public bool IsCurrentlyTransitioning { get; private set; }
}