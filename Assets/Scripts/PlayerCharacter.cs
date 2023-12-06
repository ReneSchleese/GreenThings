using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    [SerializeField] private PushHitbox _pushHitbox;
    
    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;

    private void Awake()
    {
        _joystick.Move += OnMove;
        _pushHitbox.Init(this);
    }

    private void OnMove(Vector2 delta)
    {
        Velocity = new Vector3(delta.x, 0f, delta.y) * MOVEMENT_SPEED;
        _characterController.Move(Velocity * Time.deltaTime);
        if (Velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(Velocity);
            Chain.OnUpdate();
        }
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Transform Transform => transform;
    public void Push(Vector3 direction)
    {
        _characterController.Move(direction);
    }

    public bool Pushable => false;

    public Vector3 Velocity { get; private set; }
}
