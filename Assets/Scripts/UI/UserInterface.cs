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
    [SerializeField] private VirtualJoystickRegion _movementStickRegion;
    [SerializeField] private CanvasGroup _canvasGroup;
    private static UserInterface _instance;

    private void Awake()
    {
        _digButton.onClick.AddListener(OnHornetDigPress);
        _modeButton.onClick.AddListener(OnSpiritModeTogglePress);
        _scanButton.onClick.AddListener(OnScanPress);
        _backButton.onClick.AddListener(OnBackButtonPress);
        _movementStickRegion.VirtualJoystick.StickInput += input => App.Instance.InputManager.HandleMovementInput(input);
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