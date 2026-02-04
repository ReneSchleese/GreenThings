using DG.Tweening;
using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    [SerializeField] private Transform _animationContainer;
    [SerializeField] private float _animationDuration;
    private void Start()
    {
        DOTween.Sequence(this)
            .Append(_animationContainer.DOLocalRotate(Vector3.up * 360f, _animationDuration, RotateMode.WorldAxisAdd)
            .SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }
}