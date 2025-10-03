using System.Collections;

public interface ISceneTransitionable
{
    public IEnumerator PrepareBeingTransitionedFrom();
    public void OnUnload();
    public void OnLoadComplete();
}