using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IKnockbackAble
{
    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;

    private bool _isStunned = false;
    public bool IsStunned
    {
        get => _isStunned;
    }

    public event Action OnKnockbackEnd;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }


    public void KnockbackOnSurface(Vector3 direction, float force)
    {
        direction.y = 0f;
        direction = direction.normalized;

        _navMeshAgent.updatePosition = false;
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.velocity = Vector3.zero;

        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        _isStunned = true;
        StartCoroutine(CheckKnockbackEnd());
    }

    private IEnumerator CheckKnockbackEnd()
    {
        float timeStamp = Time.time;
        yield return new WaitForFixedUpdate();
        while (true)
        {
            bool isOverTime = Time.time - timeStamp > 1f;
            if (_rigidbody.velocity.magnitude <= 0.05f || isOverTime )
            {
                _navMeshAgent.velocity = Vector3.zero;
                _navMeshAgent.updatePosition = true;
                _rigidbody.isKinematic = true;
                _rigidbody.useGravity = false;

                _isStunned = false;

                _navMeshAgent.nextPosition = transform.position;

                OnKnockbackEnd?.Invoke();
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
