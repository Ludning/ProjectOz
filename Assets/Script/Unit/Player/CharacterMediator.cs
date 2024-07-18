using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMediator : MonoBehaviour
{
    public PlayerStat playerStat;
    public ScaleController ScaleController;
    public GroundChecker PlayerGroundChecker;
    public MageControl MageControl;
    public KnightControl knightControl;
    public PlayerMovement PlayerMovement;
    public PlayerModelController playerModelController;
    public IControl CurrentControl
    {
        get
        {
            switch (playerModelController.CurrentModelState)
            {
                case PlayerModelState.Mage:
                    return MageControl;
                case PlayerModelState.Knight:
                    return knightControl;
            }
            return null;
        }
    }
    public Animator PlayerAnimator => playerModelController.CurrentAnimator;
    public bool IsGround => PlayerGroundChecker.IsGrounded();

}
