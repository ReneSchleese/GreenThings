using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IFollowable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    public const float MOVEMENT_SPEED = 4f;
    private ForestSpiritChain _spiritChain;

    private void Awake()
    {
        _joystick.Move += OnMove;
    }

    private void OnMove(Vector2 obj)
    {
        Vector3 speed = new Vector3(obj.x, 0f, obj.y) * MOVEMENT_SPEED * Time.deltaTime;
        _characterController.Move(speed);
    }


    public ForestSpiritChain ForestSpiritChain { get; } = new();
    public Vector3 WorldPosition => transform.position;
    public bool IsFollowing => false;
}
