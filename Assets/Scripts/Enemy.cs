using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IKnockbackAble
{
    [SerializeField] string _enemyId;
    [SerializeField] private EnemyData _enemyData;

    float _currentHp;

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
        _enemyData = DataManager.Instance.GetGameData<EnemyData>(_enemyId);
        _currentHp = _enemyData.enemyHp;
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

    /// <summary>
    /// 쿨다운 계산 및 테두리 밖을 튕겨 나갔는지 검사
    /// </summary>
    /// <returns></returns>
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




    //공격메ㅔ서드
    //애니메이션 실행, 움직임



    public void Attack(Animator animator, Rigidbody rigidbody, Vector3 dir, float force)
    {
        animator.SetTrigger("Attack");
        bool isNeedToMoveToward = dir.magnitude > 0.1f;
        if (isNeedToMoveToward)
        {
            rigidbody.AddForce(dir * force, ForceMode.VelocityChange);
        }
    }

    public void TakeDamage(float damage, Vector3 attcakerPos)
    {
        _currentHp -= damage;
    }

    public void TakeHit()
    {
        //checkInvincible
        //addDamage
        //SetInvincibleTime

    }
}
