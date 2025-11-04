using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;

    private Vector3 _initialSickPos; 

    public void Awake()
    {
        _initialSickPos = _virtualJoystick.transform.position;
        _virtualJoystick.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _virtualJoystick.transform.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnDrag(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _virtualJoystick.OnEndDrag();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _virtualJoystick.transform.position = _initialSickPos;
    }

    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}