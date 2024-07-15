using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetPlayerHp : MonoBehaviour
{
    [SerializeField] private float PlayerHp;

    public void OnClickEvent_PlayerHp(float Damage)
    {
        PlayerHp -= Damage;
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.PlayerHp,
            value = PlayerHp
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
    

}
