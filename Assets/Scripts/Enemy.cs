using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] string _enemyId;
    [SerializeField] private EnemyData _enemyData;

    [SerializeField]
    private Combat _combat;

    private Rigidbody _rigidbody;
    private NavMeshAgent _navMeshAgent;
    private float damage;
    private Collider _attackCollider;

    private bool _isStunned = false;
    public bool IsStunned
    {
        get => _isStunned;
    }

    public event Action OnKnockbackEnd;
    private void Awake()
    {
        _combat = new Combat();
        _enemyData = DataManager.Instance.GetGameData<EnemyData>(_enemyId);
        _combat.Init(transform, _enemyData.enemyHp);
        _combat.OnDamaged += OnDamaged;
        damage = _enemyData.enemyColDamage * _enemyData.enemyBasePower;
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnAttack(GameObject taget, float damage)
    {
        if (!taget.TryGetComponent(out Combat targetCombat))
        {
            return;
        }
        _combat.DealDamage(targetCombat, damage);
    }

    private void OnDamaged(Combat attacker)
    {
        Vector3 attackDir = (transform.position - attacker.transform.position).normalized;
    }




    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {

            if (other.TryGetComponent(out Combat target) == false)
            {
                Debug.Assert(false, "Player has no Combat component");
                return;
            }
            _combat.DealDamage(target, damage);
            return;
        }
    }

    //공격메ㅔ서드
    //애니메이션 실행, 움직임



    public void Attack(Combat target, Animator animator, Rigidbody rigidbody, Vector3 dir, float force)
    {
        animator.SetTrigger("Attack");
        bool isNeedToMoveToward = dir.magnitude > 0.1f;
        if (isNeedToMoveToward)
        {
            rigidbody.AddForce(dir * force, ForceMode.VelocityChange);
        }
    }

    public void EnableAttackCollider()
    {
        _attackCollider.enabled = true;
    }

    





    /* Not Used
    //public void KnockbackOnSurface(Vector3 direction, float force)
    //{
    //    if (IsStunned) return;

    //    direction.y = 0f;
    //    direction = direction.normalized;

    //    _navMeshAgent.updatePosition = false;
    //    _rigidbody.isKinematic = false;
    //    _rigidbody.useGravity = true;
    //    _rigidbody.velocity = Vector3.zero;

    //    _rigidbody.AddForce(direction * force, ForceMode.Impulse);
    //    _isStunned = true;


    //    StartCoroutine(CheckKnockbackEnd());
    //}

    ///// <summary>
    ///// 쿨다운 계산 및 테두리 밖을 튕겨 나갔는지 검사
    ///// </summary>
    ///// <returns></returns>
    //private IEnumerator CheckKnockbackEnd()
    //{
    //    float timeStamp = Time.time;
    //    yield return new WaitForFixedUpdate();
    //    while (true)
    //    {
    //        bool isOverTime = Time.time - timeStamp > 1f;


    //        Vector3 vel = _rigidbody.velocity;
    //        vel.y = 0f;
    //        vel *= .3f;

    //        bool isOnSurface = NavMesh.SamplePosition(transform.position + vel
    //            , out NavMeshHit hit, _navMeshAgent.height / 2f, NavMesh.AllAreas);


    //        if (_rigidbody.velocity.magnitude <= 0.05f || isOverTime || !isOnSurface)
    //        {
    //            _navMeshAgent.velocity = Vector3.zero;
    //            _navMeshAgent.updatePosition = true;
    //            _rigidbody.isKinematic = true;
    //            _rigidbody.useGravity = false;

    //            _isStunned = false;

    //            _navMeshAgent.nextPosition = transform.position;

    //            OnKnockbackEnd?.Invoke();
    //            yield break;
    //        }
    //        yield return new WaitForFixedUpdate();
    //    }
    //}
    */
}
