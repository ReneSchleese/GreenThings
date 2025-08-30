using System;
using Audio;
using DG.Tweening;
using ForestSpirits;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private PushHitbox _pushHitbox;
    [SerializeField] private HornetAnimator _animator;
    [SerializeField] private Transform _actor;
    [SerializeField] private Transform _targetLookRotator;
    [SerializeField] private AudioClip[] _hornetScreams;
    [SerializeField] private AudioClip[] _footstepsGrass;
    [SerializeField] private AudioClip _collectCoin;
    [SerializeField] private HornetAnimationEvents _animationEvents;
    [SerializeField] private bool _applyGravity;

    public const float MOVEMENT_SPEED = 8f;
    private Vector3 _lastVelocity;
    private Quaternion _rotDampVelocity;
    private Quaternion _actorRotDampVelocity;
    private PseudoRandomIndex _screamIndex;
    private PseudoRandomIndex _footstepIndex;
    private CircularBuffer<Vector3> _positionBuffer;
    private readonly Collider[] _colliders = new Collider[128];

    private void Awake()
    {
        UserInterface.Instance.VirtualJoystick.Move += OnMove;
        UserInterface.Instance.HornetScreamInput += OnHornetScream;
        UserInterface.Instance.HornetDigInput += OnHornetDigInput;
        _pushHitbox.Init(this);
        _screamIndex = new PseudoRandomIndex(_hornetScreams.Length);
        _footstepIndex = new PseudoRandomIndex(_footstepsGrass.Length);
        _positionBuffer = new CircularBuffer<Vector3>(20);
        _positionBuffer.SetAll(transform.position);
        _animationEvents.PlayFootStep += PlayFootStep;
    }

    private void Update()
    {
        if (_applyGravity)
        {
            _characterController.Move(_characterController.isGrounded ? Vector3.zero : Physics.gravity * Time.deltaTime);
        }
        _positionBuffer.Add(transform.position);
        Velocity = (_positionBuffer.GetPreviousNth(1) - _positionBuffer.GetPreviousNth(2)) / Time.deltaTime;
        _animator.UpdateAnimator(Velocity);

        Vector3 directionZeroY = Utils.CloneAndSetY(transform.forward, 0f);
        Quaternion lookRotation = Quaternion.LookRotation(directionZeroY, Vector3.up);
        Vector3 toCamera = App.Instance.MainCamera.transform.position - _actor.transform.position;
        Quaternion lookRotationTiledTowardsCamera = Utils.AlignNormalWhileLookingAlongDir(toCamera, directionZeroY);
        Quaternion tiltedAwayFromCamera = Quaternion.LerpUnclamped(lookRotation, lookRotationTiledTowardsCamera, -0.125f);
        _actor.rotation = Utils.SmoothDamp(_actor.rotation, tiltedAwayFromCamera, ref _actorRotDampVelocity, 0.05f);

        int colliderAmount = Physics.OverlapSphereNonAlloc(transform.position + Vector3.up, 1.0f, _colliders, LayerMask.GetMask("Coin"), QueryTriggerInteraction.Collide);
        for (int i = 0; i < colliderAmount; i++)
        {
            if (_colliders[i].TryGetComponent(out Coin coin) && coin.IsCollectable)
            {
                Collect(coin);
            }
        }
    }

    private void Collect(Coin coin)
    {
        Debug.Log("Collecting coin");
        AudioManager.Instance.PlayEffect(_collectCoin, Random.Range(0.8f, 1.2f), volume: 0.3f);
        Destroy(coin.gameObject);
    }

    private void OnMove(Vector2 delta)
    {
        JoystickMagnitude = delta.magnitude;
        Vector3 offset = new Vector3(delta.x, 0f, delta.y).normalized * (JoystickMagnitude * MOVEMENT_SPEED);
        offset = Quaternion.Euler(0, -45, 0) * offset;
        _characterController.Move(offset * Time.deltaTime);
        IsMoving = delta != Vector2.zero;
        if (Velocity == Vector3.zero)
        {
            return;
        }
        
        if (delta != Vector2.zero)
        {
            Vector3 directionZeroY = new(offset.x, 0f, offset.z);
            Quaternion lookRotation = Quaternion.LookRotation(directionZeroY, Vector3.up);
            transform.rotation = Utils.SmoothDamp(transform.rotation, lookRotation, ref _rotDampVelocity, 0.05f);
            _targetLookRotator.rotation = lookRotation;
        }
    }
    
    private void OnHornetScream()
    {
        if (_animator.IsInActiveBattlecry) return;
        int index = _screamIndex.Get();
        AudioManager.Instance.PlayVoice(_hornetScreams[index]);
        _animator.PlayBattlecry(index);
        Game.Instance.Chain.PlayEchoed(index, _hornetScreams[index].length);
    }

    private readonly Collider[] _digColliders = new Collider[128];
    private bool _drawDebugSphere;
    private void OnHornetDigInput()
    {
        int amount = Physics.OverlapSphereNonAlloc(transform.position, 1f, _digColliders, LayerMask.GetMask("BuriedTreasure"));
        for (int i = 0; i < amount; i++)
        {
            if (_digColliders[i].TryGetComponent(out BuriedTreasure buriedTreasure))
            {
                buriedTreasure.OnBeingDug();
                break;
            }
        }

        _drawDebugSphere = true;
        DOVirtual.DelayedCall(1f, () => { _drawDebugSphere = false; });
    }

    private void OnDrawGizmos()
    {
        if (_drawDebugSphere)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }

    private void PlayFootStep()
    {
        float pitch = Random.Range(0.7f, 1.2f);
        AudioManager.Instance.PlayEffect(_footstepsGrass[_footstepIndex.Get()], pitch);
    }

    public Vector3 Position => transform.position;
    public Vector3 BreakPosition => Position;

    public Transform Transform => transform;
    public void Push(Vector3 direction)
    {
        _characterController.Move(direction);
    }

    public bool IsPushable => false;
    public int Priority => 0;
    public void HandleCollision(float radius, IPushable otherPushable)
    {
        if (!otherPushable.IsPushable) return;
        
        const float pushStrengthMax = 3f;
        Vector3 position = transform.position;
        Vector3 otherPushablePos = otherPushable.Transform.position;
        float distance = Vector3.Distance(position, otherPushablePos);
        float pushStrength = Mathf.Max(0.1f, Mathf.Lerp(pushStrengthMax, 0f, distance / radius));
        Vector3 otherPositionInLocalSpace = _targetLookRotator.InverseTransformPoint(otherPushablePos);
        Vector3 velocityNormalized = Velocity.normalized;
        float dot = Vector3.Dot(velocityNormalized, (_positionBuffer.GetYoungest() - _positionBuffer.GetOldest()).normalized);
        bool directionHasBeenSimilarForManyFrames = dot > 0;
        Vector3 pushDir = Velocity.magnitude > 1f && directionHasBeenSimilarForManyFrames
            ? Quaternion.AngleAxis(otherPositionInLocalSpace.x < 0f ? -90 : 90, Vector3.up) * velocityNormalized
            : (otherPushablePos - position).normalized;
        otherPushable.Push(pushDir * pushStrength);
    }

    public Vector3 Velocity { get; private set; }
    public Vector3? TargetDir => transform.position + _targetLookRotator.forward;
    public float JoystickMagnitude { get; private set; }
    public bool IsMoving { get; private set; }
}
