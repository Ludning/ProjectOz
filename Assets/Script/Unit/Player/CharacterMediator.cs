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

    public Animator PlayerAnimator => playerModelController.CurrentAnimator;
    public bool IsGround => PlayerGroundChecker.IsGrounded();

    public void MovementDash()
    {
        playerMovement.OnInputDash();
    }
    public void MovementJump()
    {
        playerMovement.OnInputJump();
    }
    public void SetMovementDirection(Vector2 direction)
    {
        playerMovement.OnInputSetDirection(direction);
    }
    public void PlayerSwitchModel(PlayerModelState modelState)
    {
        playerModelController.OnInputSwitchModel(modelState);
    }
    public void PlayerSwitchTransformation()
    {
        playerStat.ChangeTransformation();
    }
}
