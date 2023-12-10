using System;
using ForestSpirits;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour, IChainTarget, IPushable
{
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private JoystickBehaviour _joystick;
    [SerializeField] private PushHitbox _pushHitbox;
    
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
        Velocity = (transform.position - _positionLastFrame) / Time.deltaTime;
        _positionLastFrame = transform.position;
    }

    private void OnMove(Vector2 delta)
    {
        Vector3 offset = new Vector3(delta.x, 0f, delta.y) * MOVEMENT_SPEED;
        _characterController.Move(offset * Time.deltaTime);
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

    public bool IsPushable => false;

    public Vector3 Velocity { get; private set; }
}
