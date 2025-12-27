using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class InteractionWidget : MonoBehaviour
{
    [SerializeField] private CanvasGroup _interactionCanvasGroup;
    [SerializeField] private RectTransform _interactionItemTransform;
    [SerializeField] private TextMeshProUGUI _interactionTmPro;

    private FadeableCanvasGroup _fadeableInteractionRegion;
    private RectTransform _canvasTransform;
    
    public void Init(RectTransform canvasTransform)
    {
        _fadeableInteractionRegion = new FadeableCanvasGroup(_interactionCanvasGroup, 0.5f);
        _fadeableInteractionRegion.FadeInstantly(fadeIn: false);
        _canvasTransform = canvasTransform;
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
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
        if (InteractionVolume is null)
        {
            return;
        }
        _interactionItemTransform.transform.position = InteractionVolume.transform.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Game.Instance.MainCamera, InteractionVolume.TextAnchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );
        _interactionItemTransform.anchoredPosition = localPoint;
    }
    
    public InteractionVolume InteractionVolume { get; set; }
}