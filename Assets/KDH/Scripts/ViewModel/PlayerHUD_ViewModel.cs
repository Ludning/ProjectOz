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
            OnPropertyChanged(nameof(PlayerHp));
        }
    }

    public float PlayerHp
    {
        get => _playerHp;
        set
        {
            _playerHp = value;
            OnPropertyChanged(nameof(SwordmanMode));
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
        SwordmanMode = message.SwordmanMode;
        PlayerHp = message.PlayerHp;
        PetulanceGuage = message.PetulanceGuage;
    }
}
