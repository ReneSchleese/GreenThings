using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : Singleton<UserInterface>
{
    public event Action HornetDigInput;
    public event Action SpiritModeToggleInput;
    public event Action ScanInput;
    
    [SerializeField] private Button _digButton;
    [SerializeField] private Button _modeButton;
    [SerializeField] private Button _scanButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;
    private static UserInterface _instance;

    private void Awake()
    {
        _digButton.onClick.AddListener(OnHornetDigPress);
        _modeButton.onClick.AddListener(OnSpiritModeTogglePress);
        _scanButton.onClick.AddListener(OnScanPress);
        _backButton.onClick.AddListener(OnBackButtonPress);
        _leftStickRegion.VirtualJoystick.StickInput += input => App.Instance.InputManager.HandleMovementInput(input);
        _rightStickRegion.VirtualJoystick.StickInput += OnRightStickInput;
        _rightStickRegion.VirtualJoystick.StickInputBegin += () => ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: true);
        _rightStickRegion.VirtualJoystick.StickInputEnd += () =>
        {
            ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: false);
            _radialMenu.OnInputEnd();
        };
        _radialMenu.Init();
        ((IFadeableCanvasGroup)_radialMenu).FadeInstantly(fadeIn: false);
    }

    private void OnRightStickInput(Vector2 input)
    {
        _radialMenu.OnInput(input);
    }

    private void OnHornetDigPress()
    {
        HornetDigInput?.Invoke();
    }

    private void OnSpiritModeTogglePress()
    {
        SpiritModeToggleInput?.Invoke();
    }

    private void OnScanPress()
    {
        ScanInput?.Invoke();
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }
}