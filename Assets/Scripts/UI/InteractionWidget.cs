using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InteractionWidget : MonoBehaviour
{
    [SerializeField] private CanvasGroup _interactionCanvasGroup;
    [SerializeField] private RectTransform _interactionItemTransform, _leafCursor;
    [SerializeField] private TextMeshProUGUI _interactionTmPro;

    private FadeableCanvasGroup _fadeableInteractionRegion;
    private RectTransform _canvasTransform;
    private InteractionVolume _interactionVolume;
    private string _wiggleAnimationId;

    public void Init(RectTransform canvasTransform)
    {
        _fadeableInteractionRegion = new FadeableCanvasGroup(_interactionCanvasGroup, 0.5f);
        _fadeableInteractionRegion.FadeInstantly(fadeIn: false);
        _canvasTransform = canvasTransform;
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
        Sequence wiggleSequence = DOTween.Sequence().SetId(WiggleAnimationId);
        wiggleSequence.AppendInterval(0.5f);
        wiggleSequence.Append(_leafCursor.DOPunchAnchorPos(Vector2.down * 5, 0.2f).SetEase(Ease.OutBack));
        wiggleSequence.SetLoops(-1);
    }

    private void OnDestroy()
    {
        DOTween.Kill(WiggleAnimationId);
        DOTween.Kill(this);
    }

    public Tween Fade(bool fadeIn)
    {
        DOTween.Kill(this);
        Tween fade = _fadeableInteractionRegion.Fade(fadeIn);
        fade.SetId(this);
        return fade;
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        if (_interactionVolume is null || _interactionCanvasGroup.alpha == 0)
        {
            return;
        }
        _interactionItemTransform.transform.position = _interactionVolume.transform.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Game.Instance.MainCamera, _interactionVolume.TextAnchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );
        _interactionItemTransform.anchoredPosition = localPoint;
    }

    public void SetInteractionVolume(InteractionVolume interactionVolume)
    {
        _interactionVolume = interactionVolume;
        if (interactionVolume is not null)
        {
            _interactionTmPro.text = $"<font-weight=600>{interactionVolume.DisplayText}</font-weight>";
        }
    }
    
    private string WiggleAnimationId
    {
        get
        {
            _wiggleAnimationId ??= $"{GetInstanceID()}.Wiggle";
            return _wiggleAnimationId;
        }
    }
}