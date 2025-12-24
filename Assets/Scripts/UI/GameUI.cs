using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;
    [SerializeField] private CanvasGroup _interactionCanvasGroup;

    private FadeableCanvasGroup _fadeableInteractionRegion;

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
        Game.Instance.Player.InteractionVolumeEntered += OnInteractionEntered;
        Game.Instance.Player.InteractionVolumeExited += OnInteractionExited;
        _fadeableInteractionRegion = new FadeableCanvasGroup(_interactionCanvasGroup, 0.5f);
        _fadeableInteractionRegion.FadeInstantly(fadeIn: false);
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }

    private void OnInteractionEntered(InteractionId id)
    {
        _fadeableInteractionRegion.Fade(fadeIn: true);
    }
    
    private void OnInteractionExited(InteractionId id)
    {
        _fadeableInteractionRegion.Fade(fadeIn: false);
    }
}