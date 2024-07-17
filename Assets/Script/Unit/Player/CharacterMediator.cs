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

    [SerializeField] private MageControl mageControl;
    //[SerializeField] private KnightControl knightControl;

    public MageControl CurrentControl
    {
        get
        {
            switch (playerModelController.CurrentModelState)
            {
                case PlayerModelState.Mage:
                    return mageControl;
                case PlayerModelState.Knight:
                    return mageControl;
            }
            return mageControl;
        }
    }

    public Animator PlayerAnimator => playerModelController.CurrentAnimator;
    public bool IsGround => PlayerGroundChecker.IsGrounded();

    public void OnKeyDownAttackButton()
    {
        CurrentControl.OnKeyDown();
    }
    public void OnKeyUpAttackButton()
    {
        CurrentControl.OnKeyUp();
    }
    public void MovementDash()
    {
        playerMovement.OnInputDash();
    }
    public void OnKeyDownMovementJump()
    {
        playerMovement.OnKeyDownJump();
    }
    public void OnKeyUpMovementJump()
    {
        playerMovement.OnKeyUpJump();
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
