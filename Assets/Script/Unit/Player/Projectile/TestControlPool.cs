using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Pool;

public class TestControlPool : MonoBehaviour
{
    private IObjectPool<BallMove> _fireballPool;
    private IObjectPool<BallMove> _flameballPool;

    private GameObject _fireballPrefab;
    private GameObject _flameballPrefab;

    private float _chargingValue;
    private float _percentOzMagic;

    private float _inputTimer = 0;
    
    private float _inputChargingTimer;
    
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
        if (Input.GetMouseButton(0))
        {
            _inputTimer += Time.deltaTime;
            if (_inputTimer >= _inputChargingTimer)
            {
                ChargingAttack();
                _inputTimer = 0;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            if(_inputTimer < _inputChargingTimer)
            {
                OnAttack(_fireballPool);
            }
            _inputTimer = 0f;
        }
    }

    private void OnAttack(IObjectPool<BallMove> ballPool)
    {
        var ball = ballPool.Get();
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

    private void ChargingAttack()
    {
        RandomChargingValue();
        Debug.Log(_chargingValue);

        if (_chargingValue >= _percentOzMagic)
        {
            OnAttack(_flameballPool);
        }
        else
        {
            OzMagicManager.Instance.Execute();
            Debug.Log("OzMagic");
        }
    }
}
