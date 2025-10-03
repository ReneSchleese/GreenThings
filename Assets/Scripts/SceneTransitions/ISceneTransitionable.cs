using System.Collections;

public interface ISceneTransitionable
{
    public IEnumerator TransitionOff();
    public IEnumerator TransitionTo();
    public void OnUnload();
    public void OnLoad();
}