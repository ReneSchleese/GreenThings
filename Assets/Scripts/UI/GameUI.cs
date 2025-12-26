using DG.Tweening;
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
        _radialMenu.FadeInBegan += UpdateInteractionUI;
        _radialMenu.FadeOutBegan += UpdateInteractionUI;
        Game.Instance.Player.CurrentInteraction.Changed += UpdateInteractionUI;
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }

    private void OnInteractionVolumeEntered(InteractionObject interaction)
    {
        // DOTween.Kill(this);
        // _fadeableInteractionRegion.Fade(fadeIn: true).SetId(this);
        // _currentInteractionObject = interaction;
        // _interactionTmPro.text = interaction.GetInteractionDisplayText();
    }
    
    private void OnInteractionVolumeExited(InteractionObject interaction)
    {
        // DOTween.Kill(this);
        // _fadeableInteractionRegion.Fade(fadeIn: false).SetId(this).OnComplete(() =>
        // {
        //     _currentInteractionObject = null;
        // });
    }

    private void UpdateInteractionUI()
    {
        Debug.Log("Update interaction");
    }
}