using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private HornetAnimator _animator;
    [SerializeField] private Transform _actor;

    public Chain Chain;
    public const float MOVEMENT_SPEED = 8f;
    private Vector3 _positionLastFrame;
    private Vector3 _lastVelocity;
    private Quaternion _rotDampVelocity;
    private Quaternion _actorRotDampVelocity;

    private void Awake()
    {
        UserInterface.Instance.JoystickMove += OnMove;
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

            Vector3 toCamera = App.Instance.MainCamera.transform.position - _actor.transform.position;
            Quaternion lookRotationTiledTowardsCamera = Utils.AlignNormalWhileLookingAlongDir(toCamera, directionZeroY);
            Quaternion tiltedAwayFromCamera = Quaternion.LerpUnclamped(lookRotation, lookRotationTiledTowardsCamera, -0.125f);
            _actor.rotation = Utils.SmoothDamp(_actor.rotation, tiltedAwayFromCamera, ref _actorRotDampVelocity, 0.05f);
        }
        
        
        Chain.OnUpdate();
    }

    public Vector3 Position => transform.position;

    public Transform Transform => transform;
    public void Push(Vector3 direction)
    {
        _characterController.Move(direction);
    }

    public bool IsPushable => false;
    public void HandleCollision(IPushable otherPushable)
    {
        Vector3 otherPositionInLocalSpace = transform.InverseTransformPoint(otherPushable.Transform.position);
        Vector3 pushToSideDir = Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * transform.forward;
        Vector3 pushBackDir = otherPushable.Transform.position - transform.position;
        var velocityMagnitude = Velocity.magnitude;
        float pushStrength = 0.05f * velocityMagnitude;

        if (!otherPushable.IsPushable)
        {
            return;
        }
        
        Vector3 pushDirection;
        if (velocityMagnitude < 2f)
        {
            pushDirection = pushBackDir;
            otherPushable.Push(pushDirection.normalized * 0.15f);
            //Debug.DrawRay(transform.position, pushDirection * 3f, Color.blue);
        }
        else
        {
            pushDirection = pushToSideDir;
            otherPushable.Push(pushDirection.normalized * pushStrength);
            //Debug.DrawRay(transform.position, pushDirection * 3f, Color.red);
        }
    }

    public Vector3 Velocity { get; private set; }
    public float JoystickMagnitude { get; private set; }
}
