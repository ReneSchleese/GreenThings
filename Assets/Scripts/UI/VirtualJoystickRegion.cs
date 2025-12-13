using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;

    private Vector3 _initialSickPos; 

    public void Init()
    {
        _initialSickPos = _virtualJoystick.transform.position;
        _virtualJoystick.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (TeleportStickToPointerDownPos)
        {
            _virtualJoystick.transform.position = eventData.position;
        }
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
    
    public void OverwriteInitialStickPosition(Vector3 position)
    {
        _initialSickPos = position;
        _virtualJoystick.transform.position = _initialSickPos;
    }

    public bool TeleportStickToPointerDownPos { get; set; } = true;
    
    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}