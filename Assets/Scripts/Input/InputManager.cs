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
        Vector2 input = _gameInput.Game.Move.ReadValue<Vector2>();
        HandleMovementInput(input);
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
}
