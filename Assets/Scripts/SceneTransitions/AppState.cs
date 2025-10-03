public interface IAppState : ISceneTransitionable
{
    public AppState Id { get; }
}

public enum AppState
{
    SplashScreen,
    LoadingScreen,
    MainMenu,
    Game
}