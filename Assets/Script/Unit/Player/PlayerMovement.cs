using UnityEngine;

public enum JumpMode
{
    
}
public class PlayerMovement : MonoBehaviour
{
    private Vector2 _direction;
    private Vector2 _lastDirection;
    
    private PcData _mageData;
    private PcData _knightData;

    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private Rigidbody Rigidbody;

    public PlayerModelState _currentState;
    public PcData CurrentData => _currentState == PlayerModelState.Knight ? _knightData : _mageData;
    
    //임시 변수
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float dashForce = 100f;
    //[SerializeField] private float moveSpeed = 100f;
    //[SerializeField] private int dashDuration = 1000;

    [SerializeField] private bool _isDash = false;

    #region Animation Hash
    private readonly int GroundHash = Animator.StringToHash("IsGround");
    private readonly int DashHash = Animator.StringToHash("IsDash");
    private readonly int DirectionYHash = Animator.StringToHash("DirectionY");
    private readonly int MoveHash = Animator.StringToHash("IsMove");
    #endregion
    
    
    
    
    
    
    [SerializeField] private float dashSpeed = 20f;       // 대시 속도
    [SerializeField] private float dashDistance = 5f;     // 대시 거리
    [SerializeField] private float dashCooldown = 1f;     // 대시 쿨다운 시간
    [SerializeField] private LayerMask dashLayerMask;     // 대시 경로에서 충돌을 감지할 레이어
    
    [SerializeField] private float moveInput;
    [SerializeField] private float lastDashTime;
    [SerializeField] private Vector2 dashStartPosition;
    [SerializeField] private Vector2 dashTargetPosition;

    private void Awake()
    {
        _mageData = DataManager.Instance.GetGameData<PcData>("C101");
        _knightData = DataManager.Instance.GetGameData<PcData>("C102");
    }
    private void Update()
    {
        if (_isDash)
            OnUpdateDash();
        else
            OnUpdateMove();
        
        CharacterMediator.PlayerAnimator.SetBool(MoveHash, (_direction.x != 0) ? true : false);
        CharacterMediator.PlayerAnimator.SetFloat(DirectionYHash, Rigidbody.velocity.y);
        CharacterMediator.PlayerAnimator.SetBool(GroundHash, CharacterMediator.IsGround);
        
        //CharacterMediator.PlayerAnimator.SetTrigger(IsAttack);
        //CharacterMediator.PlayerAnimator.SetFloat(DirectionY, 0.1f);
    }

    #region OnInput
    public void OnInputSetDirection(Vector2 direction)
    {
        _direction = direction;
        if (direction != Vector2.zero) _lastDirection = direction;
        RotationCharacter(direction);
    }
    public void OnInputJump()
    {
        if(CharacterMediator.IsGround == true)
            Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
    }
    public void OnInputDash()
    {
        _isDash = true;
        lastDashTime = Time.time;
        Rigidbody.useGravity = false;
        CharacterMediator.PlayerAnimator.SetBool(DashHash, true);
        dashStartPosition = Rigidbody.position;
        dashTargetPosition = dashStartPosition + _lastDirection * dashDistance;
    }
    #endregion
    
    
    private void RotationCharacter(Vector2 direction)
    {
        switch (direction.x)
        {
            case > 0:
                transform.rotation = Quaternion.Euler(0, 90, 0);
                break;
            case < 0:
                transform.rotation = Quaternion.Euler(0, -90, 0);
                break;
        }
    }

    #region OnUpdate
    private void OnUpdateMove()
    {
        Vector3 velocity = new Vector3(_direction.x * CurrentData.pcMoveSpeed * Time.fixedUnscaledDeltaTime, Rigidbody.velocity.y, 0);
        Rigidbody.velocity = velocity;
    }
    private void OnUpdateDash()
    {
        Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0);
        Vector2 nextPosition = Vector2.MoveTowards(Rigidbody.position, dashTargetPosition, dashSpeed * Time.fixedDeltaTime);
        RaycastHit2D hit = Physics2D.Raycast(Rigidbody.position, _lastDirection, (nextPosition - (Vector2)Rigidbody.position).magnitude, dashLayerMask);
        if (hit.collider != null)
        {
            Debug.Log("DashOff");
            Rigidbody.MovePosition(hit.point);
            _isDash = false;
            Rigidbody.useGravity = true;
            CharacterMediator.PlayerAnimator.SetBool(DashHash, false);
        }
        else
        {
            Rigidbody.MovePosition(nextPosition);
            if (((Vector2)Rigidbody.position - dashStartPosition).magnitude >= dashDistance)
            {
                Debug.Log("DashOff");
                _isDash = false;
                Rigidbody.useGravity = true;
                CharacterMediator.PlayerAnimator.SetBool(DashHash, false);
            }
        }
    }
    #endregion
}
