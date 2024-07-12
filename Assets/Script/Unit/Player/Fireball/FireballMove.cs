using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireballMove : BallMove
{
    private int _maxBounceCount;
    private int _bounceCount;

    private float _gravityValue;

    protected override void Awake()
    {
        base.Awake();

        _bulletSpeed = 10.0f;
        _bulletLifeTime = 10.0f;
        _maxBounceCount = 2;
        _gravityValue = 5.81f;
        _gravity = new Vector3(0f, -_gravityValue, 0f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _bounceCount = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Ground")||other.gameObject.CompareTag("Platform")||other.gameObject.CompareTag("Wall"))
        {
            _bounceCount++;

            if ( _bounceCount == _maxBounceCount )
            {
                DestroyBall();
            }

            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Platform"))
                _rb.velocity = new Vector3(_rb.velocity.x, 8.0f + _gravityValue, _rb.velocity.z) + _gravity;
            else
                _rb.velocity = new Vector3(-_rb.velocity.x, _rb.velocity.y, -_rb.velocity.z);
        }
    }
}
