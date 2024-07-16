using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BallMove : MonoBehaviour
{
    protected Rigidbody _rb;

    protected IObjectPool<BallMove> _ballPool;

    protected Vector3 _direction;
    protected Vector3 _gravity;

    protected float _bulletSpeed;
    protected float _bulletLifeTime;

    protected bool _isDestroyed;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnEnable()
    {
        _isDestroyed = false;
        CancelInvoke(nameof(DestroyBall)); 
    }

    protected void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }

    public void Shoot()
    {
        _direction = transform.forward;
        _rb.velocity = _direction * _bulletSpeed + _gravity;
        Invoke(nameof(DestroyBall), _bulletLifeTime);
    }

    public void SetManagedPool(IObjectPool<BallMove> ballPool)
    {
        _ballPool = ballPool;
    }

    public void DestroyBall()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        _ballPool.Release(this);
    }
}
