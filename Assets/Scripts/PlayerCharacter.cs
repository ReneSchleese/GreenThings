using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private Camera _camera;
    [SerializeField] private Animator _animator;

    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;
    private Vector3 _positionLastFrame;
    private Vector3 _lastVelocity;

    private void Awake()
    {
        _joystick.Move += OnMove;
        _pushHitbox.Init(this);
    }

    private void Update()
    {
        _characterController.Move(_characterController.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
        Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
        _positionLastFrame = transform.position;

        if (Velocity != Vector3.zero)
        {
            _animator.SetFloat("MovementSpeed", Velocity.magnitude);
            var stateIsMovement = _animator.GetCurrentAnimatorStateInfo(0).IsName("Movement");
            var isTransitioningToMovement = _animator.GetAnimatorTransitionInfo(0).IsName("IdleToMovement");
            var isName = _animator.GetNextAnimatorStateInfo(0).IsName("Movement");
            if (isName)
            {
                Debug.Log("WOULD BE RUN");
            }
            if (isTransitioningToMovement)
            {
                Debug.Log("TRANSITIONING TO IDLE");
            }
            if (!stateIsMovement && !isTransitioningToMovement && !isName)
            {
                _animator.ResetTrigger("Idle");
                _animator.ResetTrigger("Run");
                Debug.Log("Trigger Run!");
                _animator.SetTrigger("Run");
            }
        }
        else
        {
            
            var stateIsIdle = _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
            var isTransitioningToIdle = _animator.GetAnimatorTransitionInfo(0).IsName("MovementToIdle");
            if (isTransitioningToIdle)
            {
                Debug.Log("TRANSITIONING TO IDLE");
            }
            if (!stateIsIdle && !isTransitioningToIdle)
            {
                _animator.ResetTrigger("Idle");
                _animator.ResetTrigger("Run");
                Debug.Log("Trigger Idle!");
                _animator.SetTrigger("Idle");
            }
        }
    }

    private void OnMove(Vector2 delta)
    {
        JoystickMagnitude = delta.magnitude;
        Vector3 offset = new Vector3(delta.x, 0f, delta.y).normalized * (JoystickMagnitude * MOVEMENT_SPEED);
        offset = Quaternion.Euler(0, -45, 0) * offset;
        _characterController.Move(offset * Time.deltaTime);
        if (Velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Velocity.x, 0f, Velocity.z));
            Chain.OnUpdate();
            
        }
        else
        {
            
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

    public bool IsPushable => false;

    public Vector3 Velocity { get; private set; }
    public float JoystickMagnitude { get; private set; }
}
