using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private CharacterMediator CharacterMediator;
    
    #region Input Function

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>().normalized;
        CharacterMediator.PlayerMovement.OnInputSetDirection(direction);
        CharacterMediator.playerModelController.OnInputSetDirection(direction);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
            CharacterMediator.PlayerMovement.OnInputDash();
    }
    public void OnStateChange(InputAction.CallbackContext context)
    {
        if (context.started)
            CharacterMediator.playerStat.ChangeTransformation();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            CharacterMediator.PlayerMovement.OnInputJump(KeyType.KeyDown);
        else if(context.canceled)
            CharacterMediator.PlayerMovement.OnInputJump(KeyType.KeyUp);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            CharacterMediator.CurrentControl?.OnInputAttack(KeyType.KeyDown);
        else if(context.canceled)
            CharacterMediator.CurrentControl?.OnInputAttack(KeyType.KeyUp);
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
    }
    #endregion
}
