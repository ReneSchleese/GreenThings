using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickRegion : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private VirtualJoystick _virtualJoystick;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("PointerDown");
    }

    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}