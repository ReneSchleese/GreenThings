using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IFollowable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    public const float MOVEMENT_SPEED = 4f;
    private Vector3 _previousWorldPos;

    private void Awake()
    {
        _joystick.Move += OnMove;
        _previousWorldPos = WorldPosition;
    }

    private void OnMove(Vector2 obj)
    {
        _previousWorldPos = WorldPosition;
        Speed = new Vector3(obj.x, 0f, obj.y) * MOVEMENT_SPEED;
        _characterController.Move(Speed * Time.deltaTime);
    }

    private void Update()
    {
        ForestSpiritChain.TryClear();
    }

    public ForestSpiritChain ForestSpiritChain { get; } = new();

    public Vector3 WorldPosition => transform.position;
    public bool IsFollowing => false;
    public Vector3 Speed { get; private set; }
}
