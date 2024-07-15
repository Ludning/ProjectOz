using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterMediator : MonoBehaviour, IMediator
{
    [SerializeField] private PlayerController PlayerController;
    [SerializeField] private PlayerModelSwitcher PlayerModelSwitcher;
    [SerializeField] private PlayerStat playerStat;
    
    public void Notify(object sender, MessageBase message)
    {
        if (message is ICharacterMediatorMessage == false)
            return;

        if (playerStat.IsDie)
            return;
        if (message is CharacterMediatorMessage<PlayerModelState>)
            PlayerModelSwitcher.SwitchModel();
        if (message is CharacterMediatorMessage<PlayerStatControlType> msg)
            playerStat.OnResponMessage(msg);
    }
}
