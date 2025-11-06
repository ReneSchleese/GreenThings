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
        ProcessMovementInput(moveInput);
        
        Vector2 radialInput = _gameInput.Game.RadialMenu.ReadValue<Vector2>();
        ProcessRadialMenuInput(radialInput);
    }

    private void HandleScreamButton()
    {
        if (_gameInput.Game.Scream.WasPerformedThisFrame())
        {
            Screamed?.Invoke();
        }
    }

    public void ProcessMovementInput(Vector2 input)
    {
        if (input.magnitude > 1)
        {
            input = input.normalized;
        }
        Moved?.Invoke(input);
    }
    
    public void ProcessRadialMenuInput(Vector2 input)
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
    
    public void InvokeInteract()
    {
        
    }
    
    public void InvokeScan()
    {
        
    }
    
    public void InvokeDig()
    {
        
    }
    
    public void InvokeToggleFormation()
    {
        
    }
    
    public void InvokeBattleCry()
    {
        
    }
}
