using UnityEngine;

public enum PlayerModelState
{
    Mage,
    Knight,
}
public class PlayerModelController : MonoBehaviour
{
    [SerializeField] private GameObject _mageModel;
    [SerializeField] private GameObject _knightModel;
    [SerializeField] private Animator _mageMAnimator;
    [SerializeField] private Animator _knightAnimator;
    
    [SerializeField, ReadOnly] private PlayerModelState _currentModelState;
    [SerializeField, ReadOnly] private Animator _currentAnimator;
    [SerializeField] private CharacterMediator CharacterMediator;

    public PlayerModelState CurrentModelState => _currentModelState;
    public Animator CurrentAnimator => _currentAnimator;
    
    

    private void Start()
    {
        _mageModel.SetActive(true);
        _knightModel.SetActive(false);
        _currentModelState = PlayerModelState.Mage;
        _currentAnimator = _mageMAnimator;
    }
    public void OnInputSwitchModel(PlayerModelState modelState)
    {
        if (modelState == _currentModelState)
            return;
        switch (modelState)
        {
            case PlayerModelState.Knight:
                _knightModel.SetActive(true);
                _mageModel.SetActive(false);
                _currentModelState = PlayerModelState.Knight;
                _currentAnimator = _knightAnimator;
                ChangeUI_Icon();
                break;
            case PlayerModelState.Mage:
                _mageModel.SetActive(true);
                _knightModel.SetActive(false);
                _currentModelState = PlayerModelState.Mage;
                _currentAnimator = _mageMAnimator;
                ChangeUI_Icon();
                break;
        }
    }
    public void OnInputSetDirection(Vector2 direction)
    {
        if (CharacterMediator.PlayerMovement.IsDash == true)
            return;
        switch (direction.x)
        {
            case > 0:
                _mageModel.transform.rotation = Quaternion.Euler(0, 90, 0);
                _knightModel.transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case < 0:
                _mageModel.transform.rotation = Quaternion.Euler(0, -90, 0);
                _knightModel.transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
        }
    }
    
    private void ChangeUI_Icon()
    {
        PlayerHUD_Message msg = new PlayerHUD_Message()
        {
            playerHUDType = PlayerHUDType.SwordmanMode,
            value = _currentModelState == PlayerModelState.Knight ? 1 : 0
        };
        MessageManager.Instance.InvokeCallback(msg);
    }

    public void Attack()
    {
        
    }
}