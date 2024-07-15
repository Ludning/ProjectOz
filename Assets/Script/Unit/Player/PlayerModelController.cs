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
        _currentModelState = PlayerModelState.Mage;
    }

    public void SwitchModel()
    {
        switch (_currentModelState)
        {
            case PlayerModelState.Knight:
                _mageModel.SetActive(true);
                _knightModel.SetActive(false);
                _currentModelState = PlayerModelState.Mage;
                ChangeUI_Icon();
                break;
            case PlayerModelState.Mage:
                _knightModel.SetActive(true);
                _mageModel.SetActive(false);
                _currentModelState = PlayerModelState.Knight;
                ChangeUI_Icon();
                break;
        }
    }
    public void ChangeUI_Icon()
    {
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.SwordmanMode,
            value = _currentModelState == PlayerModelState.Knight ? 1 : 0
        };
        MessageManager.Instance.InvokeCallback(msg);
    }
}