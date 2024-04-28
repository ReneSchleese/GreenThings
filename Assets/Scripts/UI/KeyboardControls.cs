using UnityEngine;
using UnityEngine.UI;

public class KeyboardControls : MonoBehaviour
{
    private JoystickBehaviour _joystick;
    private Button _screamButton;
    private bool _hadJoystickInput;

    private void Start()
    {
        _joystick = FindObjectOfType<JoystickBehaviour>();
        _screamButton = UserInterface.Instance.ScreamButton;
    }

    private void Update()
    {
        HandleJoystick();
        HandleScreamButton();
    }

    private void HandleScreamButton()
    {
        if (Input.GetButtonUp("Jump") && _screamButton.isActiveAndEnabled)
        {
            _screamButton.onClick.Invoke();
        }
    }

    private void HandleJoystick()
    {
        Vector2 amount = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        amount *= 1000;
        bool hasInput = amount != Vector2.zero;
        if (!_hadJoystickInput && hasInput)
        {
            _joystick.OnCustomBeginDrag();
        }
        else if (_hadJoystickInput && !hasInput)
        {
            _joystick.OnCustomEndDrag();
        }
        else if (_hadJoystickInput && hasInput)
        {
            _joystick.OnCustomDrag(amount);
        }

        _hadJoystickInput = hasInput;
    }
}