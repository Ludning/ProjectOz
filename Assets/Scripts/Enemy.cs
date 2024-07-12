using System;
using System.Collections;
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
        if (IsStunned) return;

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
    public float TestVal = .5f;
    private IEnumerator CheckKnockbackEnd()
    {
        float timeStamp = Time.time;
        yield return new WaitForFixedUpdate();
        while (true)
        {
            bool isOverTime = Time.time - timeStamp > 1f;


            Vector3 vel = _rigidbody.velocity;
            vel.y = 0f;
            vel *= .3f;

            bool isOnSurface = NavMesh.SamplePosition(transform.position + vel, out NavMeshHit hit, TestVal, NavMesh.AllAreas);


            if (_rigidbody.velocity.magnitude <= 0.05f || isOverTime || !isOnSurface)
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




    //OnAttack attack
    //and also make damage collider
    //or turn on and off the collider



    public void Attack(Animator animator, Rigidbody rigidbody, Vector3 dir, float force)
    {
        animator.SetTrigger("Attack");
        bool isNeedToMoveToward = dir.magnitude > 0.1f;
        if (isNeedToMoveToward)
        {
            rigidbody.AddForce(dir * force, ForceMode.VelocityChange);
        }
    }

    public void TakeHit()
    {
        //checkInvincible
        //addDamage
        //SetInvincibleTime

    }
}
