using System;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public event Action HornetScreamInput;
    
    [SerializeField] private Button _hornetScreamButton;
    [SerializeField] private VirtualJoystickRegion _joystickRegion;
    [SerializeField] private CanvasGroup _canvasGroup;
    private static UserInterface _instance;

    private void Awake()
    {
        _hornetScreamButton.onClick.AddListener(OnHornetScreamPress);
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

    public VirtualJoystick VirtualJoystick => _joystickRegion.VirtualJoystick;

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