using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMediator : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerModelController playerModelController;
    [SerializeField] private PlayerStat playerStat;
    
    [SerializeField] private ScaleController ScaleController;
    [SerializeField] private GroundChecker PlayerGroundChecker;
    


    public void MovementDash()
    {
        if(PlayerGroundChecker.IsGrounded())
            playerMovement.OnJump();   
    }
    public void MovementJump()
    {
        if(PlayerGroundChecker.IsGrounded())
            playerMovement.OnJump();   
    }
    public void SetMovementDirection(Vector2 direction)
    {
        playerMovement.SetDirection(direction);
    }
    public void PlayerSwitchModel(PlayerModelState modelState)
    {
        playerModelController.SwitchModel(modelState);
    }
    public void PlayerSwitchTransformation()
    {
        playerStat.ChangeTransformation();
    }
}
