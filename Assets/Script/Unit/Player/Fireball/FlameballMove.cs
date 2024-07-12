using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlameballMove : MonoBehaviour
{
    private Rigidbody _rb;

    private IObjectPool<FlameballMove> _flameballPool;

    private Vector3 _direction;

    private float _bulletSpeed;
    private float _bulletLifeTime;

    private bool _isDestroyed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _bulletSpeed = 10.0f;
        _bulletLifeTime = 10.0f;
    }

    private void OnEnable()
    {
        _isDestroyed = false;
        CancelInvoke("DestroyFlameball");
    }

    private void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }

    public void Shoot()
    { 
    
    }

    public void SetManagedPool(IObjectPool<FlameballMove> flameball)
    { 
        _flameballPool = flameball;
    }
}
