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
        yield return TransitionTo(AppState.LoadingScreen);
        yield return TransitionTo(AppState.Game, TransitionType.NextInCurrentOut);
    }

    public IEnumerator FromEntryPoint(AppState state)
    {
        Debug.Log($"{nameof(FromEntryPoint)} {state}");
        string stateName = state.ToString();
        IAppState appState = GetAppStateFromLoadedScene<IAppState>(stateName);
        yield return appState.OnLoad();
        CurrentState = appState;
        yield return CurrentState.TransitionIn();
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
            yield return CurrentState.TransitionOut();
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

    public IAppState CurrentState { get; set; }
}