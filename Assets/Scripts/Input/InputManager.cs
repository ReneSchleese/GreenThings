using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public event Action<Vector2> Moved;
    public event Action BattleCried;
    public event Action Interacted;
    public event Action Scanned;
    public event Action Dug;
    public event Action ToggledFormation;
    
    private GameInput _gameInput;

    public void Init()
    {
        _gameInput = new GameInput();
        _gameInput.Enable();
    }
    
    private void Update()
    {
        if (_gameInput == null)
        {
            return;
        }
        Vector2 moveInput = _gameInput.Game.Move.ReadValue<Vector2>();
        ProcessMovementInput(moveInput);
        
        Vector2 radialInput = _gameInput.Game.RadialMenu.ReadValue<Vector2>();
        ProcessRadialMenuInput(radialInput);
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
    
    public void InvokeInteract() => Interacted?.Invoke();

    public void InvokeScan() => Scanned?.Invoke();

    public void InvokeDig() => Dug?.Invoke();

    public void InvokeToggleFormation() => ToggledFormation?.Invoke();

    public void InvokeBattleCry() => BattleCried?.Invoke();
}
