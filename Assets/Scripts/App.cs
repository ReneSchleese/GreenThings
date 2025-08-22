using UnityEngine;

public class App : Singleton<App>
{
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private Camera _mainCamera;

    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    
    public PlayerCharacter Player => _player;
    public Camera MainCamera => _mainCamera;
}
