using System.Collections;

public interface ISceneTransitionable
{
    public IEnumerator PrepareBeingTransitionedFrom();
    public IEnumerator PrepareBeingTransitionedTo();
    public void OnUnload();
    public void OnLoad();
}