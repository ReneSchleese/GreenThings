using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public event Action HornetScreamInput;
    public event Action<Vector2> JoystickMove;
    
    [SerializeField] private Button _hornetScreamButton;
    [SerializeField] private JoystickBehaviour _joystickBehaviour;
    [SerializeField] private CanvasGroup _canvasGroup;
    private static UserInterface _instance;

    private void Awake()
    {
        _joystickBehaviour.Move += OnJoystickInput;
        _hornetScreamButton.onClick.AddListener(OnHornetScreamPress);
        _canvasGroup.alpha = 0f;
    }

    private void OnJoystickInput(Vector2 delta)
    {
        JoystickMove?.Invoke(delta);
    }

    private void OnHornetScreamPress()
    {
        HornetScreamInput?.Invoke();
    }

    public Button ScreamButton => _hornetScreamButton;

    public float CanvasGroupAlpha
    {
        get => _canvasGroup.alpha;
        set => _canvasGroup.alpha = value;
    }

    public static UserInterface Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UserInterface>();
            }
            return _instance;
        }
    }
}