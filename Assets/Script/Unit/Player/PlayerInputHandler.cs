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
        CharacterMediator.SetMovementDirection(direction);
    }
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.MovementDash();
        }
    }
    public void OnStateChange(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.PlayerSwitchTransformation();
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.OnKeyDownMovementJump();
        }
        else if(context.canceled)
        {
            CharacterMediator.OnKeyUpMovementJump();
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CharacterMediator.OnKeyDownAttackButton();
        }
        else if(context.canceled)
        {
            CharacterMediator.OnKeyUpAttackButton();
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        //Debug.Log($"mousePosition {mousePosition}");
    }
    #endregion
}
