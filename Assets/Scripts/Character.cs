using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    private const float SPEED_FAC = 4f;

    private void Awake()
    {
        _joystick.Move += OnMove;
    }

    private void OnMove(Vector2 obj)
    {
        Vector3 speed = new Vector3(obj.x, 0f, obj.y) * SPEED_FAC * Time.deltaTime;
        _characterController.Move(speed);
    }
}
