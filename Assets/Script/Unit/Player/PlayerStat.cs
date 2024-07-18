using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    None,
    NormalAttack,//일반공격
    ChargeAttack,//강공격
    Meteor,//메테오
    TimeStop,//시간정지
    UndoTransformation,//변신해제
}
public class PlayerStat : MonoBehaviour
{
    [SerializeField, ReadOnly] private PcData _playerData;
    [SerializeField, ReadOnly] private ResourceData _resourceData;

    [SerializeField] private float _playerCurrentGage;
    [SerializeField, ReadOnly] private bool _isDie;

    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private Combat _playerCombat;

    private bool _isGageReduce = false;

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
    public bool IsDie => _playerCombat.IsDead();
    private void Start()
    {
        _playerData = DataManager.Instance.GetGameData<PcData>("C102");
        _resourceData = DataManager.Instance.GetGameData<ResourceData>("R101");
        _playerCurrentGage = 0f;

        _playerCombat.Init(_playerData.pcHp);
        _playerCombat.OnDamaged += OnDamage;
        _playerCombat.OnDead += OnDie;
    }
    private void Update()
    {
        //Todo 분노게이지 감소
        //MVVM업데이트
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.PetulanceGuage,
            value = _playerCurrentGage
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
    private void OnDestroy()
    {
        _playerCombat.OnDead -= OnDie;
        _playerCombat.OnDamaged -= OnDamage;
    }
    public void OnDamage()
    {
        //MVVM업데이트
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.PlayerHp,
            value = _playerCombat.GetHp(),
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
    private void OnDie()
    {
        //사망처리
        gameObject.SetActive(false);
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
            case AttackType.Meteor:
                PlayerCurrentGage += DataManager.Instance.GetGameData<OzmagicData>("O101").gageGainOz;
                break;
            case AttackType.TimeStop:
                PlayerCurrentGage += DataManager.Instance.GetGameData<OzmagicData>("O201").gageGainOz;
                break;
        }
    }
    public void ChangeTransformation()
    {
        if (PlayerCurrentGage >= 100)
        {
            _isGageReduce = true;
            CharacterMediator.playerModelController.OnInputSwitchModel(PlayerModelState.Knight);
        }
    }
    private void ReduceGage()
    {
        if (_isGageReduce == false)
            return;
        if (PlayerCurrentGage != 0)
        {
            PlayerCurrentGage -= Time.deltaTime;
            return;
        }
        _isGageReduce = false;
        CharacterMediator.playerModelController.OnInputSwitchModel(PlayerModelState.Mage);
    }
    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(ReduceGage);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(ReduceGage);
    }
}