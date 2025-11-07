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
        _radialMenu.Init();

        _rightStickRegion.TeleportStickToPointerDownPos = false;
        _rightStickRegion.OverwriteInitialStickPosition(_radialMenu.transform.position);
        
        _backButton.onClick.AddListener(OnBackButtonPress);
        
        _leftStickRegion.VirtualJoystick.StickInput += (input, relativeDistance) =>
        {
            App.Instance.InputManager.ProcessMovementInput(input, relativeDistance);
        };
        
        _rightStickRegion.VirtualJoystick.StickInput += (input, relativeDistance) => { _radialMenu.OnInput(input, relativeDistance); };
        _rightStickRegion.VirtualJoystick.StickInputBegin += () => { ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: true); };
        _rightStickRegion.VirtualJoystick.StickInputEnd += () => 
        {
            ((IFadeableCanvasGroup)_radialMenu).Fade(fadeIn: false);
            _radialMenu.OnInputEnd();
        };
        
        ((IFadeableCanvasGroup)_radialMenu).FadeInstantly(fadeIn: false);
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }
}