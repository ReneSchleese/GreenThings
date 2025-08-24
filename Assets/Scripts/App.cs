using UnityEngine;

public class App : Singleton<App>
{
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    public Camera MainCamera => _mainCamera;
}
