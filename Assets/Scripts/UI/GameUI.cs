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
        _backButton.onClick.AddListener(OnBackButtonPress);
        _leftStickRegion.VirtualJoystick.StickInput += input =>
        {
            App.Instance.InputManager.ProcessMovementInput(input);
        };
        _rightStickRegion.VirtualJoystick.StickInput += input => { _radialMenu.OnInput(input); };
        _rightStickRegion.VirtualJoystick.StickInputBegin += () => { ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: true); };
        _rightStickRegion.VirtualJoystick.StickInputEnd += () => 
        {
            ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: false);
            _radialMenu.OnInputEnd();
        };
        
        _radialMenu.Init();
        ((IFadeableCanvasGroup)_radialMenu).FadeInstantly(fadeIn: false);
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }
}