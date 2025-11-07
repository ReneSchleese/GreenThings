using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;

    public void Init()
    {
        _leftStickRegion.Init();
        _rightStickRegion.Init();
        _radialMenu.Init(_rightStickRegion);
        
        _backButton.onClick.AddListener(OnBackButtonPress);
        
        _leftStickRegion.VirtualJoystick.StickInput += input =>
        {
            App.Instance.InputManager.ProcessMovementInput(input);
        };
        
        _radialMenu.FadeInstantly(fadeIn: false);
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }
}