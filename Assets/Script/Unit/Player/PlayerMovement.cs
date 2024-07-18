using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private PcData _mageData;
    private PcData _knightData;
    
    private Vector2 _direction;
    private Vector2 _lastDirection;
    
    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private BoxCollider DashCollider;

    private PlayerModelState _currentState;
    private PcData CurrentData => _currentState == PlayerModelState.Knight ? _knightData : _mageData;
    

    #region Animation Hash
    private readonly int GroundHash = Animator.StringToHash("IsGround");
    private readonly int DashHash = Animator.StringToHash("IsDash");
    private readonly int DirectionYHash = Animator.StringToHash("DirectionY");
    private readonly int MoveHash = Animator.StringToHash("IsMove");
    private readonly int RushSlashHash = Animator.StringToHash("IsAttack");
    #endregion
    
    
    [SerializeField] private float airSpeed = 7f;       // 공중 속도
    [SerializeField] private float dashForce = 20f;       // 대시 속도
    [SerializeField] private float dashDistance = 3f;     // 대시 거리
    [SerializeField] private float dashCooldown = 1f;     // 대시 쿨다운 시간
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask dashLayerMask;     // 대시 경로에서 충돌을 감지할 레이어
    
    //[SerializeField] private float capsuleRadius = 0.5f;
    
    private bool _isRushSlash = false;
    private bool _isDash = false;
    private float dashStartPositionX;
    private float currentDashCooldown = 0f;
    private Vector2 rushStartPosition;

    public float stopDistance = 0.1f; // 멈출 때의 허용 오차
    
    public float _rushSlashDistance; // 돌진베기의 거리

    public bool IsRushSlash => _isRushSlash;
    public bool IsDash => _isDash;

    private event Action RushSlashEndEvent;

    private void Awake()
    {
        _mageData = DataManager.Instance.GetGameData<PcData>("C101");
        _knightData = DataManager.Instance.GetGameData<PcData>("C102");
    }
    private void Update()
    {
        if (IsRushSlash)
            OnUpdateRushSlash();
        else if (_isDash)
            OnUpdateDash();
        else
            OnUpdateMove();
        CharacterMediator.PlayerAnimator.SetBool(MoveHash, (_direction.x != 0) ? true : false);
        CharacterMediator.PlayerAnimator.SetFloat(DirectionYHash, Rigidbody.velocity.y);
        CharacterMediator.PlayerAnimator.SetBool(GroundHash, CharacterMediator.IsGround);
    }

    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(RefreshCooldown);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(RefreshCooldown);
    }

    #region OnInput
    public void OnInputSetDirection(Vector2 direction)
    {
        _direction = direction;
        /*if (_isDash == true)
            return;
        if (direction != Vector2.zero) _lastDirection = direction;
        RotationCharacter(direction);*/
    }

    public void OnInputJump(KeyType type)
    {
        switch (type)
        {
            case KeyType.KeyDown:
                StartJump();
                break;
            case KeyType.KeyUp:
                EndJump();
                break;
        }
    }
    
    public void OnInputDash()
    {
        if (currentDashCooldown <= 0f)
        {
            StartDash();
            currentDashCooldown = dashCooldown;
        }
    }
    #endregion
    //캐릭터 
    private void RotationCharacter(Vector2 direction)
    {
        switch (direction.x)
        {
            case > 0:
                DashCollider.center = new Vector3(0.3f, 1.2f, 0f);
                break;
            case < 0:
                DashCollider.center = new Vector3(-0.3f, 1.2f, 0f);
                break;
        }
    }
    #region OnUpdate
    private void OnUpdateMove()
    {
        if (_direction != Vector2.zero) 
            _lastDirection = _direction;
        RotationCharacter(_direction);
        CharacterMediator.playerModelController.OnInputSetDirection(_direction);

        if (CharacterMediator.IsGround == true)
        {
            Vector3 velocity = new Vector3(_direction.x * CurrentData.pcMoveSpeed * 100 * Time.fixedUnscaledDeltaTime, Rigidbody.velocity.y, 0);
            Rigidbody.velocity = velocity;
        }
        else
        {
            Vector3 velocity = new Vector3(_direction.x * airSpeed * 100 * Time.fixedUnscaledDeltaTime, Rigidbody.velocity.y, 0);
            Rigidbody.velocity = velocity;
        }
    }

    private void OnUpdateDash()
    {
        float distanceTraveled = Mathf.Abs(dashStartPositionX - transform.position.x);
        if (distanceTraveled >= dashDistance - stopDistance)
            EndDash();
    }
    private void OnUpdateRushSlash()
    {
        float rushDistance = Vector2.Distance(rushStartPosition, transform.position);
        if (rushDistance >= _rushSlashDistance - stopDistance)
            RushSlashEndEvent?.Invoke();
    }

    #endregion
    private void StartDash()
    {
        _isDash = true;
        Rigidbody.useGravity = false;
        Rigidbody.velocity = Vector3.zero;
        CharacterMediator.PlayerAnimator.SetBool(DashHash, true);
        Rigidbody.AddForce(_lastDirection * dashForce, ForceMode.Impulse);
        dashStartPositionX = transform.position.x;
    }
    private void EndDash()
    {
        _isDash = false;
        Rigidbody.useGravity = true;
        Rigidbody.velocity = Vector3.zero;
        CharacterMediator.PlayerAnimator.SetBool(DashHash, false);
    }
    public void StartRushSlash(Vector2 targetDirection, float rushForce, float distance, Action endEvent)
    {
        RushSlashEndEvent = endEvent;
        rushStartPosition = transform.position;
        _rushSlashDistance = distance;
        _isRushSlash = true;
        Rigidbody.useGravity = false;
        Rigidbody.velocity = Vector3.zero;
        CharacterMediator.PlayerAnimator.SetBool(RushSlashHash, true);
        Rigidbody.AddForce(targetDirection * rushForce, ForceMode.Impulse);
    }
    public void EndRushSlash()
    {
        _isRushSlash = false;
        Rigidbody.useGravity = true;
        Rigidbody.velocity = Vector3.zero;
        CharacterMediator.PlayerAnimator.SetBool(RushSlashHash, false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (_isDash == true)
        {
            // 충돌한 오브젝트의 레이어를 가져옴
            int otherLayer = other.gameObject.layer;

            // LayerMask를 사용하여 충돌한 오브젝트가 dashLayerMask에 속하는지 확인
            if ((dashLayerMask.value & (1 << otherLayer)) > 0)
            {
                EndDash();
            }
        }
    }

    #region Update Action
    private void RefreshCooldown()
    {
        if (currentDashCooldown > 0f)
        {
            currentDashCooldown -= Time.deltaTime;
            if (currentDashCooldown < 0f)
                currentDashCooldown = 0f;
        }
    }
    #endregion
    
    public void StartJump()
    {
        if(CharacterMediator.IsGround == true)
            Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }
    public void EndJump()
    {
        if(CharacterMediator.IsGround == true)
            Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }
}
