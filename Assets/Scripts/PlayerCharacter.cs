using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    public const float MOVEMENT_SPEED = 6f;

    private void Awake()
    {
        _joystick.Move += OnMove;
    }

    private void OnMove(Vector2 delta)
    {
        Speed = new Vector3(delta.x, 0f, delta.y) * MOVEMENT_SPEED;
        _characterController.Move(Speed * Time.deltaTime);
    }

    private void Update()
    {
        Chain.BreakOffIfTooFar();
    }

    public Chain Chain { get; } = new();

    public Vector3 WorldPosition => transform.position;
    public Vector3 Speed { get; private set; }
}
