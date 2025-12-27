using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Button _backButton;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;
    [SerializeField] private InteractionWidget _interactionWidget;

    public void Init()
    {
        _leftStickRegion.Init();
        _rightStickRegion.Init();
        _radialMenu.Init(_rightStickRegion);
        _interactionWidget.Init(_canvas.GetComponent<RectTransform>());
        
        _backButton.onClick.AddListener(OnBackButtonPress);
        
        _leftStickRegion.VirtualJoystick.StickInput += input =>
        {
            App.Instance.InputManager.ProcessMovementInput(input);
        };
        
        _radialMenu.FadeInstantly(fadeIn: false);
        _radialMenu.BeingUsedChanged += UpdateInteractionUI;
        Game.Instance.Player.InteractionState.Changed += UpdateInteractionUI;
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }

    private void UpdateInteractionUI()
    {
        PlayerInteractionState interactionState = Game.Instance.Player.InteractionState;
        _radialMenu.UpdateWithInteraction(interactionState);
        bool hasVolume = interactionState.InteractionVolume is not null;
        if (hasVolume)
        {
            _interactionWidget.SetInteractionVolume(interactionState.InteractionVolume);
        }
        bool interactionWidgetShouldBeVisible = hasVolume && !_radialMenu.IsBeingUsed;
        _interactionWidget.Fade(interactionWidgetShouldBeVisible);
    }
}