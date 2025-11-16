using DG.Tweening;
using UnityEngine;

public class RadialMenuCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _root, _shellCursor, _leafCursor, _leafCursorPointer;
    [SerializeField] private CanvasGroup _canvasGroup;

    private bool _shellCursorIsActive;

    public void SetStyle(bool setShellCursorActive,  bool animate)
    {
        bool didChange = _shellCursorIsActive != setShellCursorActive;
        if (!didChange)
        {
            return;
        }
        
        _shellCursorIsActive = setShellCursorActive;
        DOTween.Kill(this);
        RectTransform transformToScaleDown = setShellCursorActive ? _leafCursor :  _shellCursor;
        RectTransform transformToScaleUp = transformToScaleDown == _leafCursor ?  _shellCursor : _leafCursor;
        if (animate)
        {
            Sequence sequence = DOTween.Sequence().SetId(this);

            const float duration = 0.1f;
            sequence.Insert(0.0f, transformToScaleDown.DOScale(Vector3.zero, duration).SetEase(Ease.OutCubic));
            sequence.Insert(duration, transformToScaleUp.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
            sequence.InsertCallback(duration, () => transformToScaleUp.gameObject.SetActive(true));
            sequence.OnStart(() =>
            {
                transformToScaleUp.gameObject.SetActive(false);
                transformToScaleDown.gameObject.SetActive(true);
            });
            sequence.OnComplete(OnComplete);
        }
        else
        {
            OnComplete();
        }
        return;

        void OnComplete()
        {
            transformToScaleUp.gameObject.SetActive(true);
            transformToScaleDown.gameObject.SetActive(false);
            transformToScaleDown.localScale = transformToScaleDown.gameObject.activeSelf ? Vector3.one : Vector3.zero;
            transformToScaleUp.localScale = transformToScaleUp.gameObject.activeSelf ? Vector3.one : Vector3.zero;
        }
    }
    
    public CanvasGroup CanvasGroup =>  _canvasGroup;
    public RectTransform RectTransform => _root;
}