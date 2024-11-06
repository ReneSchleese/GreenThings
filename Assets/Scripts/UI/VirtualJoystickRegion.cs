using UnityEngine;

public class VirtualJoystickRegion : MonoBehaviour
{
    [SerializeField] private VirtualJoystick _virtualJoystick;
    public VirtualJoystick VirtualJoystick => _virtualJoystick;
}