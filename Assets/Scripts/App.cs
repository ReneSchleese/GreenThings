using UnityEngine;

public class App : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _player;
    
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private static App _instance;
    public static App Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<App>();
            }
            return _instance;
        }
    }

    public PlayerCharacter Player => _player;
}
