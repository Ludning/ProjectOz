using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    NormalAttack,//일반공격
    ChargeAttack,//강공격
    UndoTransformation,//변신해제
}
public class PlayerStat : MonoBehaviour, IDamageable
{
    [SerializeField, ReadOnly] private PcData _playerData;
    [SerializeField, ReadOnly] private ResourceData _resourceData;

    [SerializeField, ReadOnly] private int _playerCurrentHp;
    [SerializeField] private float _playerCurrentGage;
    [SerializeField, ReadOnly] private bool _isDie;

    [SerializeField] private CharacterMediator CharacterMediator;

    public float PlayerCurrentGage
    {
        get => _playerCurrentGage;
        set
        {
            _playerCurrentGage = value;
            if (_playerCurrentGage < 0)
                _playerCurrentGage = 0;
            if (_playerCurrentGage > _resourceData.gageMax)
                _playerCurrentGage = _resourceData.gageMax;
        }
    }
    public bool IsDie => _isDie;
    private void Start()
    {
        _playerData = DataManager.Instance.GetGameData<PcData>("C102");
        _resourceData = DataManager.Instance.GetGameData<ResourceData>("R101");
        _playerCurrentHp = _playerData.pcHp;
        _playerCurrentGage = 0f;
    }

    private void Update()
    {
        
    }

    public void OnDamage(int damage)
    {
        _playerCurrentHp -= damage;
        if (_playerCurrentHp < 0)
        {
            _playerCurrentHp = 0;
            OnDie();
        }

        //MVVM업데이트
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.PlayerHp,
            value = _playerCurrentHp
        };
        MessageManager.Instance.InvokeCallback(msg);
    }

    private void OnDie()
    {
        //사망처리
        _isDie = true;
    }

    public void ChangeGage(AttackType type)
    {
        switch (type)
        {
            case AttackType.NormalAttack:
                PlayerCurrentGage += _resourceData.gageGainNormal;
                break;
            case AttackType.ChargeAttack:
                PlayerCurrentGage += _resourceData.gageGainCharge;
                break;
            case AttackType.UndoTransformation:
                PlayerCurrentGage -= _resourceData.gagePenalty;
                break;
        }
    }
    public void ChangeTransformation()
    {
        if (PlayerCurrentGage >= 100)
        {
            GageReduceStart();
            CharacterMediator.PlayerSwitchModel(PlayerModelState.Knight);
        }
    }


    public void GageReduceStart()
    {

    }
}
