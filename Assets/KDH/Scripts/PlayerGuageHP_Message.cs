using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerHUDType
{
    PlayerHp,
    SwordmanMode,
    PetulanceGuage,
}
public struct PlayerHUD_Message : MessageBase
{
    public PlayerHUDType playerHUDType;
    public float value;
}