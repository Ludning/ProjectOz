using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField] private Transform _firePos;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damage = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Init(Transform firePos)
    {
        _firePos = firePos;
    }
    public void Fire()
    {
        Vector3 dir = _firePos.forward;
        _rigidbody.velocity = dir * _speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Combat>().Damaged(_damage);
        }
        Destroy(gameObject);
    }

}
