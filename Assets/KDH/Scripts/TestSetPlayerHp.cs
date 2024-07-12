using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetPlayerHp : MonoBehaviour
{
    [SerializeField] private int playerHp;
    public void OnClickEvent()
    {
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            PlayerHp = playerHp,
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
}
