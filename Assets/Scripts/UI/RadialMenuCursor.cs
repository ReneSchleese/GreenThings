using DG.Tweening;
using UnityEngine;

public class RadialMenuCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _root, _leafCursorTransform, _leafCursorPointer, _leafCursorScaler;
    [SerializeField] private CanvasGroup _rootGroup, _shellGroup, _leafGroup;

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
        CanvasGroup groupToFadeOut = setShellCursorActive ? _leafGroup :  _shellGroup;
        CanvasGroup groupToFadeIn = groupToFadeOut == _leafGroup ?  _shellGroup : _leafGroup;
        if (animate)
        {
            Sequence sequence = DOTween.Sequence().SetId(this);
            const float duration = 0.2f;
            sequence.Insert(0.0f, groupToFadeOut.DOFade(0f, duration).SetEase(Ease.OutCubic));
            sequence.Insert(0.0f, groupToFadeIn.DOFade(1f, duration).SetEase(Ease.OutCubic));
            if (!setShellCursorActive)
            {
                sequence.Insert(0.1f, _leafCursorScaler.DOPunchScale(Vector3.one * 0.3f, 0.1f).SetEase(Ease.OutBack));
            }
            sequence.OnComplete(OnComplete);
        }
        else
        {
            OnComplete();
        }
        return;

        void OnComplete()
        {
            groupToFadeIn.alpha = 1f;
            groupToFadeOut.alpha = 0f;
        }
    }
    
    public CanvasGroup RootGroup =>  _rootGroup;
    public RectTransform RectTransform => _root;
    public RectTransform LeafCursorTransform => _leafCursorTransform;
}