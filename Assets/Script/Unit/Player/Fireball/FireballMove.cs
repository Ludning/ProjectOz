using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireballMove : MonoBehaviour
{
    private Rigidbody _rb;

    private IObjectPool<FireballMove> _fireballPools;

    private Vector3 _direction;

    private float _bulletSpeed;
    private float _bulletLifeTime;

    private int _maxBounceCount;
    private int _bounceCount;

    private bool _isDestroyed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _bulletSpeed = 10.0f;
        _bulletLifeTime = 10.0f;
        _maxBounceCount = 2;
    }

    private void OnEnable()
    {
        _bounceCount = 0;
        _isDestroyed = false;
        CancelInvoke("DestroyFireball");
    }

    private void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }

    public void Shoot()
    {
        _direction = transform.forward;
        _rb.velocity = _direction * _bulletSpeed;
        Invoke("DestroyFireball", _bulletLifeTime);
    }

    public void SetManagedPool(IObjectPool<FireballMove> fireballPool)
    { 
        _fireballPools = fireballPool;
    }

    public void DestroyFireball()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        _fireballPools.Release(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("À¸¾Ç");

            _bounceCount++;

            if ( _bounceCount == _maxBounceCount )
            {
                DestroyFireball();
            }

            //_rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            _rb.velocity = new Vector3(_rb.velocity.x, 10.0f, _rb.velocity.z);
        }
    }
}
