using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : Singleton<UserInterface>
{
    public event Action HornetScreamInput;
    public event Action HornetDigInput;
    public event Action SpiritModeToggleInput;
    
    [SerializeField] private Button _hornetScreamButton;
    [SerializeField] private Button _digButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private VirtualJoystickRegion _joystickRegion;
    [SerializeField] private CanvasGroup _canvasGroup;
    private static UserInterface _instance;

    private void Awake()
    {
        _hornetScreamButton.onClick.AddListener(OnHornetScreamPress);
        _digButton.onClick.AddListener(OnHornetDigPress);
        _modeButton.onClick.AddListener(OnSpiritModeTogglePress);
    }

    private void OnHornetScreamPress()
    {
        HornetScreamInput?.Invoke();
    }
    
    private void OnHornetDigPress()
    {
        HornetDigInput?.Invoke();
    }
    
    private void OnSpiritModeTogglePress()
    {
        SpiritModeToggleInput?.Invoke();
    }

    public Button ScreamButton => _hornetScreamButton;

    public float CanvasGroupAlpha
    {
        get => _canvasGroup.alpha;
        set => _canvasGroup.alpha = value;
    }

    public VirtualJoystick VirtualJoystick => _joystickRegion.VirtualJoystick;
}