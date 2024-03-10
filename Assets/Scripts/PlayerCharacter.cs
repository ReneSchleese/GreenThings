using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private HornetAnimator _animator;

    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;
    private Vector3 _positionLastFrame;
    private Vector3 _lastVelocity;
    private Quaternion _rotDampVelocity;

    private void Awake()
    {
        _joystick.Move += OnMove;
        _pushHitbox.Init(this);
        _positionLastFrame = transform.position;
    }

    private void Update()
    {
        _characterController.Move(_characterController.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
        Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
        _positionLastFrame = transform.position;
        _animator.UpdateAnimator(Velocity);
    }
    
    private void OnMove(Vector2 delta)
    {
        JoystickMagnitude = delta.magnitude;
        Vector3 offset = new Vector3(delta.x, 0f, delta.y).normalized * (JoystickMagnitude * MOVEMENT_SPEED);
        offset = Quaternion.Euler(0, -45, 0) * offset;
        _characterController.Move(offset * Time.deltaTime);
        if (Velocity == Vector3.zero)
        {
            return;
        }
        
        if (delta != Vector2.zero)
        {
            Vector3 directionZeroY = new(offset.x, 0f, offset.z);
            Quaternion lookRotation = Quaternion.LookRotation(directionZeroY, Vector3.up);
            transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.05f);
        }
        Chain.OnUpdate();
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

    public bool IsPushable => false;

    public Vector3 Velocity { get; private set; }
    public float JoystickMagnitude { get; private set; }
}
