using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;
    [SerializeField] private CanvasGroup _interactionCanvasGroup;
    [SerializeField] private RectTransform _interactionWidget;
    [SerializeField] private TextMeshProUGUI _interactionTmPro;
    [SerializeField] private Canvas _canvas;

    private FadeableCanvasGroup _fadeableInteractionRegion;
    private InteractionObject _currentInteractionObject;
    private RectTransform _canvasTransform;

    public void Init()
    {
        _canvasTransform = _canvas.GetComponent<RectTransform>();
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
    
    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        if (_currentInteractionObject is null)
        {
            return;
        }
        _interactionWidget.transform.position = _currentInteractionObject.transform.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Game.Instance.MainCamera, _currentInteractionObject.TextAnchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );
        _interactionWidget.anchoredPosition = localPoint;
    }

    private void OnBackButtonPress()
    {
        App.Instance.TransitionToMainMenu();
    }

    private void OnInteractionEntered(InteractionObject interaction)
    {
        _fadeableInteractionRegion.Fade(fadeIn: true);
        _currentInteractionObject = interaction;
        _interactionTmPro.text = interaction.InteractionId.ToString();
    }
    
    private void OnInteractionExited(InteractionObject interaction)
    {
        _fadeableInteractionRegion.Fade(fadeIn: false);
        _currentInteractionObject = null;
    }
}