using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;
    [SerializeField] private Transform _virtualJoystickRoot;
    [SerializeField] private CanvasGroup _joystickGroup;

    private Vector3 _initialRootPos; 

    public void Awake()
    {
        _initialRootPos = _virtualJoystickRoot.position;
        SetAppearance(isDragging:false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetAppearance(isDragging:true);
        _virtualJoystickRoot.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _virtualJoystick.SimulateBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _virtualJoystick.SimulateDrag(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _virtualJoystick.SimulateEndDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _virtualJoystickRoot.position = _initialRootPos;
        SetAppearance(isDragging:false);
    }

    private void SetAppearance(bool isDragging)
    {
        _joystickGroup.alpha = isDragging ? 1f : 0.4f;
    }

    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}