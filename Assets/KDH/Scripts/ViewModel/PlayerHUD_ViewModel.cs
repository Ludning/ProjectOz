using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD_ViewModel : ViewModelBase<PlayerHUD_Message>
{
    private bool _swordmanMode;
    private float _playerHp;
    private float _petulanceGuage;

    public bool SwordmanMode
    {
        get => _swordmanMode;
        set
        {
            _swordmanMode = value;
            OnPropertyChanged(nameof(SwordmanMode));
        }
    }

    public float PlayerHp
    {
        get => _playerHp;
        set
        {
            _playerHp = value;
            OnPropertyChanged(nameof(PlayerHp));
        }
    }

    public float PetulanceGuage
    {
        get => _petulanceGuage;
        set
        {
            _petulanceGuage = value;
            OnPropertyChanged(nameof(PetulanceGuage));
        }
    }

    protected override void OnResponseMessage(PlayerHUD_Message message)
    {
        switch(message.playerHUDType)
        {
            case PlayerHUDType.PlayerHp:
                PlayerHp = message.value;
                break;
            case PlayerHUDType.SwordmanMode:
                SwordmanMode = (message.value == 0) ? false : true;
                break;
            case PlayerHUDType.PetulanceGuage:
                PetulanceGuage = message.value;
                break;
        }
    }
}
