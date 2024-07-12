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
}
