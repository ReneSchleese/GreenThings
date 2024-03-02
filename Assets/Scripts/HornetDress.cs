using UnityEngine;

public class HornetDress : MonoBehaviour
{
    [SerializeField] private Transform _dressBone;
    [SerializeField] private Transform _target;
    [SerializeField] [Range(0, 1)] private float _strength = 0.5f;

    private Vector3 _offset;
    private Vector3 _initialLocalPos;
    
    private void Awake()
    {
        _initialLocalPos = _dressBone.localPosition;
        _offset = _dressBone.position - _target.position;
    }

    private void FixedUpdate()
    {
        _dressBone.position = Vector3.Lerp(_dressBone.parent.TransformPoint(_initialLocalPos), _target.position + _offset, _strength);
    }
}
