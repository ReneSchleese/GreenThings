using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private Camera _camera;
    
    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;
    private Vector3 _positionLastFrame;

    private void Awake()
    {
        _joystick.Move += OnMove;
        _pushHitbox.Init(this);
    }

    private void Update()
    {
        _characterController.Move(_characterController.isGrounded ? Vector3.zero : Physics.gravity);
        Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
        _positionLastFrame = transform.position;
    }

    private void OnMove(Vector2 delta)
    {
        JoystickMagnitude = delta.magnitude;
        Vector3 dirInCamSpace = _camera.transform.TransformDirection(delta);
        Vector3 offset = new Vector3(dirInCamSpace.x, 0f, dirInCamSpace.z).normalized * (JoystickMagnitude * MOVEMENT_SPEED);
        _characterController.Move(offset * Time.deltaTime);
        if (Velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(Velocity.x, 0f, Velocity.z));
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

    public bool IsPushable => false;

    public Vector3 Velocity { get; private set; }
    public float JoystickMagnitude { get; private set; }
}
