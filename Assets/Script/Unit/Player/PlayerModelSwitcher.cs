using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerModelState
{
    Knight,
    Mage,
}
public class PlayerModelSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject _knightModel;
    [SerializeField] private GameObject _mageModel;
    
    [SerializeField, ReadOnly] private PlayerModelState _currentModelState;

    private void Start()
    {
        _mageModel.SetActive(true);
        _knightModel.SetActive(false);
    }

    public void SwitchModel()
    {
        switch (_currentModelState)
        {
            case PlayerModelState.Knight:
                _mageModel.SetActive(true);
                _knightModel.SetActive(false);
                _currentModelState = PlayerModelState.Mage;
                break;
            case PlayerModelState.Mage:
                _knightModel.SetActive(true);
                _mageModel.SetActive(false);
                _currentModelState = PlayerModelState.Knight;
                break;
        }
    }
}