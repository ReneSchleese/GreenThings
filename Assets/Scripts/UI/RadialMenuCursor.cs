using DG.Tweening;
using UnityEngine;

public class RadialMenuCursor : MonoBehaviour
{
    [SerializeField] private RectTransform _root, _shellCursor, _leafCursor, _leafCursorPointer;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetStyle(bool setShellCursorActive,  bool animate)
    {
        animate = false;
        DOTween.Kill(this);
        RectTransform transformToScaleDown = setShellCursorActive ? _leafCursor :  _shellCursor;
        RectTransform transformToScaleUp = transformToScaleDown == _leafCursor ?  _shellCursor : _leafCursor;
        if (animate)
        {
            Sequence sequence = DOTween.Sequence().SetId(this);
            
            sequence.Insert(0.0f, transformToScaleDown.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutCubic));
            sequence.Insert(0.2f, transformToScaleUp.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
            sequence.InsertCallback(0.2f, () => transformToScaleUp.gameObject.SetActive(true));
            sequence.OnStart(() =>
            {
                transformToScaleUp.gameObject.SetActive(!setShellCursorActive);
                transformToScaleDown.gameObject.SetActive(setShellCursorActive);
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
            Debug.Log($"SetStyle shellCursorActive={setShellCursorActive}, scaleDown={transformToScaleDown.name}, scaleUp={transformToScaleUp.name}");
        }
    }
    
    public CanvasGroup CanvasGroup =>  _canvasGroup;
    public RectTransform RectTransform => _root;
}