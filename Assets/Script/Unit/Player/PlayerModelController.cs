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
    [SerializeField] private VfxControl transformdVfx;

    public PlayerModelState CurrentModelState => _currentModelState;
    public Animator CurrentAnimator => _currentAnimator;

    private void Start()
    {
        _mageModel.SetActive(true);
        _knightModel.SetActive(false);
        _currentModelState = PlayerModelState.Mage;
        _currentAnimator = _mageMAnimator;


        CharacterMediator.playerCombat.OnDamaged += OnDamaged;
    }
    private void OnDestroy()
    {
        CharacterMediator.playerCombat.OnDamaged -= OnDamaged;
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
                transformdVfx.StartParticle();
                CharacterMediator.playerCombat.IsInvincibility = false;
                ChangeUI_Icon();
                break;
            case PlayerModelState.Mage:
                _mageModel.SetActive(true);
                _knightModel.SetActive(false);
                _currentModelState = PlayerModelState.Mage;
                _currentAnimator = _mageMAnimator;
                CharacterMediator.playerCombat.IsInvincibility = false;
                if(CharacterMediator.CurrentControl is KnightControl knightControl)
                    knightControl.EndRushSlash();
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

    public void OnDamaged()
    {
        BlinkCurrentCharacter();
    }

    [SerializeField] private Renderer _mageRenderer;
    [SerializeField] private Renderer _knightRenderer;
    [SerializeField] private BlinkVfx _blink;
    private void BlinkCurrentCharacter()
    {
        switch (CurrentModelState)
        {
            case PlayerModelState.Mage:
                _blink.Set(_mageRenderer);
                break;
            case PlayerModelState.Knight:
                _blink.Set(_knightRenderer);
                break;
            default:
                break;
        }
        _blink.Play();
    }
}