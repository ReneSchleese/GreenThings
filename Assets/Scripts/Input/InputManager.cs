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
        _gameInput.Enable();
    }
    
    private void Update()
    {
        HandleScreamButton();

        if (_gameInput == null)
        {
            return;
        }
        Vector2 moveInput = _gameInput.Game.Move.ReadValue<Vector2>();
        HandleMovementInput(moveInput);
        
        Vector2 radialInput = _gameInput.Game.RadialMenu.ReadValue<Vector2>();
        HandleRadialMenuInput(radialInput);
    }

    private void HandleScreamButton()
    {
        if (_gameInput.Game.Scream.WasPerformedThisFrame())
        {
            Screamed?.Invoke();
        }
    }

    public void HandleMovementInput(Vector2 input)
    {
        if (input.magnitude > 1)
        {
            input = input.normalized;
        }
        Moved?.Invoke(input);
    }
    
    public void HandleRadialMenuInput(Vector2 input)
    {
        if (input.magnitude > 1)
        {
            input = input.normalized;
        }

        if (input != Vector2.zero)
        {
            Debug.Log($"Radial menu input={input}");
        }
    }
}
