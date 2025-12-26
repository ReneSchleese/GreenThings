using Cinemachine;
using TMPro;
using UnityEngine;

public class InteractionWidget : MonoBehaviour
{
    [SerializeField] private CanvasGroup _interactionCanvasGroup;
    [SerializeField] private RectTransform _interactionItemTransform;
    [SerializeField] private TextMeshProUGUI _interactionTmPro;
    
    private InteractionObject _currentInteractionObject;    
    private FadeableCanvasGroup _fadeableInteractionRegion;
    private RectTransform _canvasTransform;
    
    public void Init(RectTransform canvasTransform)
    {
        _fadeableInteractionRegion = new FadeableCanvasGroup(_interactionCanvasGroup, 0.5f);
        _fadeableInteractionRegion.FadeInstantly(fadeIn: false);
        _canvasTransform = canvasTransform;
    }
    
    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }

    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }

    private void OnCameraUpdated(CinemachineBrain brain)
    {
        if (_currentInteractionObject is null)
        {
            return;
        }
        _interactionItemTransform.transform.position = _currentInteractionObject.transform.position;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Game.Instance.MainCamera, _currentInteractionObject.TextAnchor.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasTransform,
            screenPoint,
            null,
            out Vector2 localPoint
        );
        _interactionItemTransform.anchoredPosition = localPoint;
    }
}