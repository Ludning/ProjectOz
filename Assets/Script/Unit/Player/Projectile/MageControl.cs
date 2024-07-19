using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Pool;

public class MageControl : MonoBehaviour, IControl
{
    #region Resource
    private IObjectPool<BallMove> _fireballPool;
    private IObjectPool<BallMove> _flameballPool;
    private GameObject _fireballPrefab;
    private GameObject _flameballPrefab;
    private GameObject _ozMagicVfxPrefab;
    #endregion
    
    [Header("공격차징중 속도")]
    [SerializeField] private float _attackChargingSpeed = 0.2f;
    [Header("공격발생시 속도")]
    [SerializeField] private float _attackFireSpeed = 1f;
    [Space]
    
    [Header("상승까지의 점프 Press 요구시간")]
    [SerializeField] private float _jumpInputMaxTimer;
    [Header("상승상태의 지속시간")]
    [SerializeField] private float _flyMaxTimer;
    [Header("상승상태의 힘")]
    [SerializeField] private float flyValue = 1f;
    [Space]
    private float _chargingValue;
    private float _percentOzMagic;

    private float _attackInputTimer = 0;
    private float _jumpInputTimer = 0;
    private float _flyInputTimer = 0;
    
    private float _attackInputChargingTimer;
    
    private bool attackKeyDown = false;
    private bool jumpKeyDown = false;
    private bool _isFly = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePosition;
    [SerializeField] private CharacterMediator player;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private VfxControl vfxControl;
    
    private readonly int HashAttack = Animator.StringToHash("IsAttack");
    private static readonly int AttackClipSpeed = Animator.StringToHash("AttackClipSpeed");

    private int _jumpCount;
    
    private void Awake()
    {
        _attackInputChargingTimer = DataManager.Instance.GetGameData<SkillData>("S102").value1;
        _percentOzMagic = DataManager.Instance.GetGameData<SkillData>("S102").value2;
        _fireballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Fireball");
        _flameballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Flameball");
        _ozMagicVfxPrefab = ResourceManager.Instance.LoadResourceWithCaching<GameObject>("Ozmagicmove_vfx");
        
        _fireballPool = new ObjectPool<BallMove>(() => CreateBall(_fireballPrefab, _fireballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        _flameballPool = new ObjectPool<BallMove>(() => CreateBall(_flameballPrefab, _flameballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        
        
    }
    private void Update()
    {
        if (_jumpCount > 0)
        {
            if (player.IsGround == true)
                _jumpCount = 0;
        }
    }

    private bool afterFire = false;
    public void OnAnimation_Enter()
    {
        _attackInputTimer = 0;
        if(attackKeyDown == true)
            animator.SetFloat(AttackClipSpeed, _attackChargingSpeed);
        afterFire = false;
    }
    public void OnAnimation_Fire()
    {
        vfxControl.StartParticle();
        if(_attackInputTimer < _attackInputChargingTimer)
            NormalAttack();
        else
        {
            _attackInputTimer = 0;
            ChargeAttack();
        }
        afterFire = true;
    }
    public void OnAnimation_Exit()
    {
        _attackInputTimer = 0;
        animator.SetFloat(AttackClipSpeed, _attackFireSpeed);
        afterFire = false;
    }
    
    public void OnInputAttack(KeyType type)
    {
        switch (type)
        {
            case KeyType.KeyDown:
                StartAttack();
                break;
            case KeyType.KeyUp:
                EndAttack();
                break;
        }
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

    #region Jump
    public void StartJump()
    {
        jumpKeyDown = true;
        if (player.IsGround == true)
            rb.AddForce(Vector2.up * player.PlayerMovement.JumpForce, ForceMode.Impulse);
    }
    public void EndJump()
    {
        jumpKeyDown = false;
        _isFly = false;
        _jumpInputTimer = 0f;
        _flyInputTimer = 0f;
    }
    #endregion
    #region Attack
    private void StartAttack()
    {
        attackKeyDown = true;
        animator.SetBool(HashAttack, true);
        if(afterFire == false)
            animator.SetFloat(AttackClipSpeed, 0.2f);
    }
    private void EndAttack()
    {
        attackKeyDown = false;
        animator.SetBool(HashAttack, false);
        if (_attackInputTimer < _attackInputChargingTimer)
        {
            //TODO
            //애니메이션 진행도를 활성화해야함, 약공격
            animator.SetFloat(AttackClipSpeed, 1f);
        }
        _attackInputTimer = 0f;
    }
    #endregion

    #region OzMagic
    private void NormalAttack()
    {
        SpawnObject(_fireballPool);
        player.playerStat.ChangeGage(AttackType.NormalAttack);
    }
    private void ChargeAttack()
    {
        RandomChargingValue();
        if (_chargingValue >= _percentOzMagic)
        {
            SpawnObject(_flameballPool);
            player.playerStat.ChangeGage(AttackType.ChargeAttack);
        }
        else
        {
            AttackType type = OzMagicManager.Instance.Execute();
            player.playerStat.ChangeGage(type);
            Debug.Log("OzMagic");
        }
    }
    private void SpawnObject(IObjectPool<BallMove> pool)
    {
        var ball = pool.Get();
        ball.transform.position = firePosition.position;
        ball.transform.rotation = firePosition.rotation;
        ball.Shoot();
    }
    private void RandomChargingValue()
    {
        _chargingValue = UnityEngine.Random.Range(1, 101);
    }
    #endregion
    #region PoolFunction
    private BallMove CreateBall(GameObject prefab, IObjectPool<BallMove> pool)
    {
        var ball = Instantiate(prefab, transform.position, transform.rotation).GetComponent<BallMove>();
        ball.SetManagedPool(pool);
        return ball;
    }
    #endregion
    #region BallControl
    private void OnGetBall(BallMove ball)
    {
        ball.gameObject.SetActive(true);
    }
    private void OnReleaseBall(BallMove ball)
    {
        ball.gameObject.SetActive(false);
    }
    private void OnDestroyBall(BallMove ball)
    {
        Destroy(ball.gameObject);
    }
    #endregion

    #region Update Action
    private void RefreshAttackInputTime()
    {
        if (attackKeyDown == false)
            return;
        _attackInputTimer += Time.deltaTime;
        
        if (_attackInputTimer >= _attackInputChargingTimer)
        {
            //TODO
            //애니메이션 진행도를 활성화해야함, 강공격
            animator.SetFloat(AttackClipSpeed, _attackFireSpeed);
        }
    }
    private void RefreshJumpInputTime()
    {
        if (jumpKeyDown == false && _isFly == false)
            return;
        _jumpInputTimer += Time.deltaTime;
        if (_jumpInputTimer >= _jumpInputMaxTimer)
        {
            _isFly = true;
        }
    }
    private void RefreshFlyTime()
    {
        if (_isFly == false)
            return;
        _flyInputTimer += Time.deltaTime;
        if (_flyInputTimer < _flyMaxTimer)
        {
            player.PlayerMovement.OnFly(flyValue);
        }
        else
        {
            _isFly = false;
        }
    }
    #endregion
    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(RefreshAttackInputTime);
        TimeManager.Instance.RegistCooldownAction(RefreshJumpInputTime);
        TimeManager.Instance.RegistCooldownAction(RefreshFlyTime);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(RefreshAttackInputTime);
        TimeManager.Instance.DeregistCooldownAction(RefreshJumpInputTime);
        TimeManager.Instance.DeregistCooldownAction(RefreshFlyTime);
    }
}
