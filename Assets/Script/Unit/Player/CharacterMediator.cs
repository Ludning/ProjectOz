using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMediator : MonoBehaviour
{
    [SerializeField] public PlayerStat playerStat;

    [SerializeField] private ScaleController ScaleController;
    [SerializeField] private GroundChecker PlayerGroundChecker;

    [SerializeField] private MageControl MageControl;
    [SerializeField] private KnightControl knightControl;

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
