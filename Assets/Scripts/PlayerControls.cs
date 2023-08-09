using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    private JoystickBehaviour _joystick;

    private void Start()
    {
        _joystick = FindObjectOfType<JoystickBehaviour>();
    }

    private void Update()
    {
        Vector2 amount = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _joystick.InvokeMove(amount);
    }
}