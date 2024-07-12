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

    [SerializeField] private GameObject _fireballPrefab;
    [SerializeField] private GameObject _flameballPrefab;

    private int _chargingValue;
    private int _percentOzMagic = 30;

    private float _inputTimer = 0;

    private void Awake()
    {
        _fireballPool = new ObjectPool<BallMove>(() => CreateBall(_fireballPrefab, _fireballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
        _flameballPool = new ObjectPool<BallMove>(() => CreateBall(_flameballPrefab, _flameballPool), OnGetBall, OnReleaseBall, OnDestroyBall);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            _inputTimer += Time.deltaTime;
            if (_inputTimer >= 2.0f)
            {
                ChargingAttack();
                _inputTimer = 0;
            }
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            if(_inputTimer < 2f)
            {
                OnAttack(_fireballPool);
            }
            _inputTimer = 0f;
        }
        

        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.Rotate(0f, 180f, 0f);
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
            Debug.Log("OzFail");
            // todo
        }
    }
}
