using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;
    [SerializeField] private Transform _virtualJoystickRoot;

    private Vector3 _initialRootPos; 

    public void Awake()
    {
        _initialRootPos = _virtualJoystickRoot.position;
        _virtualJoystick.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _virtualJoystickRoot.position = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("begin drag VirtualJoystickRegion");
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
    }

    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}