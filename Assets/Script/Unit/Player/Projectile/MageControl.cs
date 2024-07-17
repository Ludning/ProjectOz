using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Pool;

public class MageControl : MonoBehaviour
{
    private IObjectPool<BallMove> _fireballPool;
    private IObjectPool<BallMove> _flameballPool;

    private GameObject _fireballPrefab;
    private GameObject _flameballPrefab;

    private float _chargingValue;
    private float _percentOzMagic;

    private float _inputTimer = 0;
    
    private float _inputChargingTimer;

    private bool keyDown = false;
    private bool keyUp = false;

    [SerializeField] private Animator animator;
    private readonly int HashAttack = Animator.StringToHash("IsAttack");
    
    private void Awake()
    {
        _fireballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Fireball");
        _flameballPrefab = ResourceManager.Instance.LoadResource<GameObject>("Flameball");
        
        _fireballPool = new ObjectPool<BallMove>(() => CreateBall(_fireballPrefab, _fireballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        _flameballPool = new ObjectPool<BallMove>(() => CreateBall(_flameballPrefab, _flameballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        
        _inputChargingTimer = DataManager.Instance.GetGameData<SkillData>("S102").value1;
        _percentOzMagic = DataManager.Instance.GetGameData<SkillData>("S102").value2;
    }

    private void Update()
    {
        if (keyDown)
        {
            _inputTimer += Time.deltaTime;
            if (_inputTimer >= _inputChargingTimer)
            {
                animator.SetTrigger(HashAttack);
                _inputTimer = 0;
            }
        }
    }

    public void OnKeyDown()
    {
        animator.SetTrigger(HashAttack);
        keyDown = true;
        keyUp = false;
    }

    public void OnKeyUp()
    {
        keyDown = false;
        keyUp = true;
        if(_inputTimer < _inputChargingTimer)
        {
            animator.SetTrigger(HashAttack);
        }
        _inputTimer = 0f;
    }

    public void OnAttack()
    {
        if(_inputTimer < _inputChargingTimer)
            NormalAttack();
        else
            ChargeAttack();
    }
    private void NormalAttack()
    {
        SpawnObject(_fireballPool);
    }
    private void ChargeAttack()
    {
        RandomChargingValue();
        Debug.Log(_chargingValue);

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
        ball.transform.position = transform.position;
        ball.transform.rotation = transform.rotation;
        ball.Shoot();
    }
    
    private BallMove CreateBall(GameObject prefab, IObjectPool<BallMove> pool)
    {
        var ball = Instantiate(prefab, transform.position, transform.rotation).GetComponent<BallMove>();
        ball.SetManagedPool(pool);
        return ball;
    }
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

    private void RandomChargingValue()
    {
        _chargingValue = UnityEngine.Random.Range(1, 101);
    }

    
}
