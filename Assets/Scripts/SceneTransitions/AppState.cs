public interface IAppState : ISceneTransitionable
{
    public AppState Id { get; }
    public string AppStateName => Id.ToString();
}

public enum AppState
{
    SplashScreen,
    LoadingScreen,
    MainMenu,
    Game
}