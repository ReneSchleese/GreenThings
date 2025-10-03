using System.Collections;

public interface ISceneTransitionable
{
    public IEnumerator TransitionOut();
    public IEnumerator TransitionIn();
    public void OnUnload();
    public IEnumerator OnLoad();
}