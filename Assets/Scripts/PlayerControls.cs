using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private JoystickBehaviour _joystick;
    private bool _hadInput;

    private void Start()
    {
        _joystick = FindObjectOfType<JoystickBehaviour>();
    }

    private void Update()
    {
        Vector2 amount = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        amount *= 1000;
        bool hasInput = amount != Vector2.zero;
        if (!_hadInput && hasInput)
        {
            _joystick.OnCustomBeginDrag();
        }
        else if (_hadInput && !hasInput)
        {
            _joystick.OnCustomEndDrag();
        }
        else if (_hadInput && hasInput)
        {
            _joystick.OnCustomDrag(amount);
        }
        _hadInput = hasInput;
    }
}