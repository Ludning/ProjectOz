using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField, ReadOnly] private PcData _playerData;
    private void Start()
    {
        _playerData = DataManager.Instance.GetGameData<PcData>("C102");
    }
}
