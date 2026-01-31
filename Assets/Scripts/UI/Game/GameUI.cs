using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private VirtualJoystickRegion _leftStickRegion;
    [SerializeField] private VirtualJoystickRegion _rightStickRegion;
    [SerializeField] private RadialMenu _radialMenu;
    [SerializeField] private InteractionWidget _interactionWidget;
    [SerializeField] private RectTransform _safeArea;
    [SerializeField] private MoneyCounter _moneyCounter;

    private Rect _lastSafeArea = new(0, 0, 0, 0);

    public void Init()
    {
        _leftStickRegion.Init();
        _rightStickRegion.Init();
        _radialMenu.Init(_rightStickRegion);
        _interactionWidget.Init(_canvas.GetComponent<RectTransform>());
        _moneyCounter.Init();

        _leftStickRegion.VirtualJoystick.StickInput += ProcessMovementInput;
        
        _radialMenu.FadeInstantly(fadeIn: false);
        _radialMenu.BeingUsedChanged += UpdateInteractionUI;
        Game.Instance.Player.InteractionState.Changed += UpdateInteractionUI;
        ApplySafeArea();
    }
    
    public void Unload()
    {
        _leftStickRegion.VirtualJoystick.StickInput -= ProcessMovementInput;
        _radialMenu.BeingUsedChanged -= UpdateInteractionUI;
        Game.Instance.Player.InteractionState.Changed -= UpdateInteractionUI;
        _moneyCounter.Unload();
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

    private void ProcessMovementInput(Vector2 input)
    {
        App.Instance.InputManager.ProcessMovementInput(input);
    }
    
    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        if (safeArea == _lastSafeArea)
            return;

        _lastSafeArea = safeArea;

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        _safeArea.anchorMin = anchorMin;
        _safeArea.anchorMax = anchorMax;
    }
}