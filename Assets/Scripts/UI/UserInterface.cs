using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : Singleton<UserInterface>
{
    public event Action HornetScreamInput;
    public event Action HornetDigInput;
    
    [SerializeField] private Button _hornetScreamButton;
    [SerializeField] private Button _digButton;
    [SerializeField] private VirtualJoystickRegion _joystickRegion;
    [SerializeField] private CanvasGroup _canvasGroup;
    private static UserInterface _instance;

    private void Awake()
    {
        _hornetScreamButton.onClick.AddListener(OnHornetScreamPress);
        _digButton.onClick.AddListener(OnHornetDigPress);
    }

    private void OnHornetScreamPress()
    {
        HornetScreamInput?.Invoke();
    }
    
    private void OnHornetDigPress()
    {
        HornetDigInput?.Invoke();
    }

    public Button ScreamButton => _hornetScreamButton;

    public float CanvasGroupAlpha
    {
        get => _canvasGroup.alpha;
        set => _canvasGroup.alpha = value;
    }

    public VirtualJoystick VirtualJoystick => _joystickRegion.VirtualJoystick;
}