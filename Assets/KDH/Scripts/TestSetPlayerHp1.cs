using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetPlayerHp1 : MonoBehaviour
{
    [SerializeField] private bool mode;
    
    public void OnClickEvent_SwordmanMode()
    {
        mode = !mode;
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.SwordmanMode,
            value = mode ? 1 : 0
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
}
