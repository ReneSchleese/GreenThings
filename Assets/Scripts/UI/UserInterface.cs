using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public event Action HornetScreamPress;
    public event Action<Vector2> JoystickMove;
    
    [SerializeField] private Button _hornetScreamButton;
    [SerializeField] private JoystickBehaviour _joystickBehaviour;
    private static UserInterface _instance;

    private void Awake()
    {
        _joystickBehaviour.Move += OnJoystickInput;
        _hornetScreamButton.onClick.AddListener(OnHornetScreamPress);
    }

    private void OnJoystickInput(Vector2 delta)
    {
        JoystickMove?.Invoke(delta);
    }

    private void OnHornetScreamPress()
    {
        HornetScreamPress?.Invoke();
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