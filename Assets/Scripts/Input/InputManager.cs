using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> Moved;
    public event Action Screamed;
    private GameInput _gameInput;

    public void Init()
    {
        _gameInput = new GameInput();
    }
    
    private void Update()
    {
        HandleKeyboardLeftStick();
        HandleScreamButton();
    }

    private void HandleScreamButton()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Screamed?.Invoke();
        }
    }

    private void HandleKeyboardLeftStick()
    {
        float x = 0f;
        float y = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed) x += 1f;
            if (Keyboard.current.sKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed) y += 1f;
        }
        Vector2 input = new(x, y);
        Moved?.Invoke(input.normalized);
    }
}
