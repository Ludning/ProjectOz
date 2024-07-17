using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 _direction;
    private Vector2 _lastDirection;
    
    private PcData _mageData;
    private PcData _knightData;

    [SerializeField] private CharacterMediator CharacterMediator;
    [SerializeField] private Rigidbody Rigidbody;
    [SerializeField] private CapsuleCollider capsuleCollider;

    private PlayerModelState _currentState;
    private PcData CurrentData => _currentState == PlayerModelState.Knight ? _knightData : _mageData;
    


    #region Animation Hash
    private readonly int GroundHash = Animator.StringToHash("IsGround");
    private readonly int DashHash = Animator.StringToHash("IsDash");
    private readonly int DirectionYHash = Animator.StringToHash("DirectionY");
    private readonly int MoveHash = Animator.StringToHash("IsMove");
    #endregion
    
    
    [SerializeField] private float airSpeed = 7f;       // 공중 속도
    [SerializeField] private float dashForce = 20f;       // 대시 속도
    [SerializeField] private float dashDistance = 3f;     // 대시 거리
    [SerializeField] private float dashCooldown = 1f;     // 대시 쿨다운 시간
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask dashLayerMask;     // 대시 경로에서 충돌을 감지할 레이어
    
    //[SerializeField] private float capsuleRadius = 0.5f;
    
    private bool _isDash = false;
    private float dashStartPositionX;
    private float _colliderHeight;

    private float currentDashCooldown = 0f;
    private float distanceTraveled;
    public float stopDistance = 0.1f; // 멈출 때의 허용 오차

    private void Awake()
    {
        _mageData = DataManager.Instance.GetGameData<PcData>("C101");
        _knightData = DataManager.Instance.GetGameData<PcData>("C102");
        _colliderHeight = GetWorldHeight();
    }
    private void Update()
    {
        if (_isDash)
            OnUpdateDash();
        else
            OnUpdateMove();

        OnUpdateCooldown();
        
        CharacterMediator.PlayerAnimator.SetBool(MoveHash, (_direction.x != 0) ? true : false);
        CharacterMediator.PlayerAnimator.SetFloat(DirectionYHash, Rigidbody.velocity.y);
        CharacterMediator.PlayerAnimator.SetBool(GroundHash, CharacterMediator.IsGround);
    }

    #region OnInput
    public void OnInputSetDirection(Vector2 direction)
    {
        if (_isDash == true)
            return;
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
        if (currentDashCooldown <= 0f)
        {
            StartDash();
            currentDashCooldown = dashCooldown;
        }
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
    private void OnUpdateCooldown()
    {
        if (currentDashCooldown > 0f)
        {
            currentDashCooldown -= Time.deltaTime;
            if (currentDashCooldown < 0f)
                currentDashCooldown = 0f;
        }
    }
    private void OnUpdateMove()
    {
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
        // 이동한 거리 계산
        distanceTraveled = Mathf.Abs(dashStartPositionX - transform.position.x);

        if (distanceTraveled >= dashDistance - stopDistance)
        {
            // 설정한 거리를 이동했을 때 대쉬 중지
            EndDash();
        }
    }
    #endregion
    private void StartDash()
    {
        _isDash = true;
        Rigidbody.useGravity = false;
        Rigidbody.velocity = Vector3.zero;
        CharacterMediator.PlayerAnimator.SetBool(DashHash, true);
        distanceTraveled = 0;
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


    float GetWorldHeight()
    {
        float localHeight = capsuleCollider.height;
        int direction = capsuleCollider.direction;
        Vector3 scale = capsuleCollider.transform.lossyScale;

        float worldHeight = 0f;
        switch (direction)
        {
            case 0: // X축
                worldHeight = localHeight * scale.x;
                break;
            case 1: // Y축
                worldHeight = localHeight * scale.y;
                break;
            case 2: // Z축
                worldHeight = localHeight * scale.z;
                break;
        }
        return worldHeight;
    }
}
