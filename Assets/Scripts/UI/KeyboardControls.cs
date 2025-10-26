using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class KeyboardControls : MonoBehaviour
{
    private VirtualJoystick _virtualJoystick;
    private Button _screamButton;
    private bool _hadJoystickInput;

    private void Start()
    {
        _virtualJoystick = FindFirstObjectByType<VirtualJoystick>();
        _screamButton = UserInterface.Instance.ScreamButton;
    }

    private void Update()
    {
        TranslateKeyboardInputsToJoystick();
        HandleScreamButton();
    }

    private void HandleScreamButton()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && _screamButton.isActiveAndEnabled)
        {
            _screamButton.onClick.Invoke();
        }
    }

    private void TranslateKeyboardInputsToJoystick()
    {
        Vector2 amount = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        amount *= 1000;
        bool hasInput = amount != Vector2.zero;
        if (!_hadJoystickInput && hasInput)
        {
            _virtualJoystick.SimulateBeginDrag();
        }
        else if (_hadJoystickInput && !hasInput)
        {
            _virtualJoystick.SimulateEndDrag();
        }
        else if (_hadJoystickInput && hasInput)
        {
            _virtualJoystick.SimulateDrag(amount);
        }

        _hadJoystickInput = hasInput;
    }
}