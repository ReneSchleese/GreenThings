using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;

    private void Awake()
    {
        _joystick.Move += OnMove;
    }

    private void OnMove(Vector2 delta)
    {
        Speed = new Vector3(delta.x, 0f, delta.y) * MOVEMENT_SPEED;
        _characterController.Move(Speed * Time.deltaTime);
        if (Speed != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Speed);
            Chain.OnUpdate();
        }
    }

    public Vector3 WorldPosition => transform.position;
    public Vector3 Speed { get; private set; }
}
