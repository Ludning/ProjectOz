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
    #endregion
    

    private float _chargingValue;
    private float _percentOzMagic;

    private float _inputTimer = 0;
    
    private float _inputChargingTimer;

    private bool keyDown = false;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform firePosition;
    [SerializeField] private CharacterMediator CharacterMediator;
    
    private readonly int HashAttack = Animator.StringToHash("IsAttack");
    private static readonly int AttackClipSpeed = Animator.StringToHash("AttackClipSpeed");

    private void Awake()
    {
        _inputChargingTimer = DataManager.Instance.GetGameData<SkillData>("S102").value1;
        _percentOzMagic = DataManager.Instance.GetGameData<SkillData>("S102").value2;
        _fireballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Fireball");
        _flameballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Flameball");
        
        _fireballPool = new ObjectPool<BallMove>(() => CreateBall(_fireballPrefab, _fireballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        _flameballPool = new ObjectPool<BallMove>(() => CreateBall(_flameballPrefab, _flameballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        
        
    }


    private bool afterFire = false;
    public void OnAnimation_Enter()
    {
        _inputTimer = 0;
        if(keyDown == true)
            animator.SetFloat(AttackClipSpeed, 0.2f);
        afterFire = false;
    }
    public void OnAnimation_Fire()
    {
        if(_inputTimer < _inputChargingTimer)
            NormalAttack();
        else
        {
            _inputTimer = 0;
            ChargeAttack();
        }

        afterFire = true;
    }
    public void OnAnimation_Exit()
    {
        _inputTimer = 0;
        animator.SetFloat(AttackClipSpeed, 1f);
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
        if(type == KeyType.KeyDown)
            StartJump();
    }
    

    #region Attack
    private void StartAttack()
    {
        keyDown = true;
        animator.SetBool(HashAttack, true);
        if(afterFire == false)
            animator.SetFloat(AttackClipSpeed, 0.2f);
    }
    private void EndAttack()
    {
        keyDown = false;
        animator.SetBool(HashAttack, false);
        if (_inputTimer < _inputChargingTimer)
        {
            //TODO
            //애니메이션 진행도를 활성화해야함, 약공격
            animator.SetFloat(AttackClipSpeed, 1f);
        }
        _inputTimer = 0f;
    }
    #endregion

    #region OzMagic
    private void NormalAttack()
    {
        SpawnObject(_fireballPool);
        CharacterMediator.playerStat.ChangeGage(AttackType.NormalAttack);
    }
    private void ChargeAttack()
    {
        RandomChargingValue();
        if (_chargingValue >= _percentOzMagic)
        {
            SpawnObject(_flameballPool);
            CharacterMediator.playerStat.ChangeGage(AttackType.ChargeAttack);
        }
        else
        {
            AttackType type = OzMagicManager.Instance.Execute();
            CharacterMediator.playerStat.ChangeGage(type);
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
    private void RefreshInputTime()
    {
        if (keyDown == false)
            return;
        _inputTimer += Time.deltaTime;
        
        if (_inputTimer >= _inputChargingTimer)
        {
            //TODO
            //애니메이션 진행도를 활성화해야함, 강공격
            animator.SetFloat(AttackClipSpeed, 1f);
        }
    }
    #endregion
    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(RefreshInputTime);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(RefreshInputTime);
    }
}
