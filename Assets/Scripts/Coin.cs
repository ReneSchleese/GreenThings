using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Transform _rotationAnimationContainer;
    [SerializeField] private Transform _hoverAnimationContainer;
    [SerializeField] private SpriteBlobShadow _blobShadow;
    
    private void Start()
    {
        DOTween.Sequence(this)
            .Append(_rotationAnimationContainer.DOLocalRotate(Vector3.up * 360f, 2f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear))
            .SetLoops(-1);
    }

    private void Update()
    {
        _blobShadow.UpdateShadow();
    }
}