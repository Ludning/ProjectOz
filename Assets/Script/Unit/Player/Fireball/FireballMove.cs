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
        if(other.gameObject.CompareTag("Ground"))
        {
            Debug.Log("À¸¾Ç");

            _bounceCount++;

            if ( _bounceCount == _maxBounceCount )
            {
                DestroyBall();
            }

            _rb.velocity = new Vector3(_rb.velocity.x, 10.0f, _rb.velocity.z);
        }
    }
}
