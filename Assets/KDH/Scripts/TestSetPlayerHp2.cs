using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetPlayerHp2 : MonoBehaviour
{
    [SerializeField] private float guage;

    public void OnClickEvent_PetulanceGuage(float addGuage)
    {
        guage += addGuage;

        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.PetulanceGuage,
            value = guage,
        };
        MessageManager.Instance.InvokeCallback(msg);
    }

}
