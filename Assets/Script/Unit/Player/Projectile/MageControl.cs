using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Pool;

public class MageControl : MonoBehaviour
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

    private void OnEnable()
    {
        TimeManager.Instance.RegistCooldownAction(ReflashInputTime);
    }
    private void OnDisable()
    {
        TimeManager.Instance.DeregistCooldownAction(ReflashInputTime);
    }

    public void OnAnimation_Enter()
    {
        Debug.Log("Enter!");
        _inputTimer = 0;
        animator.SetFloat(AttackClipSpeed, 0.2f);
    }
    public void OnAnimation_Exit()
    {
        Debug.Log("Exit!");
        _inputTimer = 0;
        animator.SetFloat(AttackClipSpeed, 1f);
    }
    public void OnAnimation_Fire()
    {
        Debug.Log("Fire!");
        if(_inputTimer < _inputChargingTimer)
            NormalAttack();
        else
        {
            _inputTimer = 0;
            ChargeAttack();
        }
    }
    public void OnInput(KeyType type)
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
    
    

    #region Attack
    private void StartAttack()
    {
        keyDown = true;
        animator.SetBool(HashAttack, true);
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
    }
    private void ChargeAttack()
    {
        RandomChargingValue();
        if (_chargingValue >= _percentOzMagic)
        {
            SpawnObject(_flameballPool);
        }
        else
        {
            OzMagicManager.Instance.Execute();
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
    private void ReflashInputTime()
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
}
