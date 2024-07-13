using BehaviorDesigner.Runtime;
using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{

    [Header("AI_Patrol")]
    [SerializeField] private float _moveRange;
    [SerializeField] private Transform _leftPatrolPoint;
    [SerializeField] private Transform _rightPatrolPoint;

    [SerializeField] private Combat _combat;

    [SerializeField] private string _enemyId;
    [SerializeField] private EnemyData _enemyData;

    [SerializeField] private bool _isMovable = false;

    [SerializeField] private Detector _detector;

    private Rigidbody _rigidbody;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private float damage;
    private Collider _attackCollider;


    public event Action OnKnockbackEnd;
    private void Awake()
    {
        _combat = new Combat();
        _enemyData = DataManager.Instance.GetGameData<EnemyData>(_enemyId);
        _combat.Init(transform, _enemyData.enemyHp);
        _combat.OnDamaged += OnDamaged;
        _combat.OnDead += OnDead;
        damage = _enemyData.enemyColDamage * _enemyData.enemyBasePower;
        _rigidbody = GetComponent<Rigidbody>();
        _navMeshAgent = GetComponent<NavMeshAgent>();


        _leftPatrolPoint.position = transform.position + _moveRange * Vector3.right;
        _rightPatrolPoint.position = transform.position - _moveRange * Vector3.right;
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
        if (attacker == null)
        {
            return;
        }
        Vector3 attackDir = (transform.position - attacker.transform.position).normalized;
    }




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            if (other.TryGetComponent(out Enemy target) == false)
            {
                Debug.Assert(false, "Player has no Combat component");
                return;
            }
            _combat.DealDamage(target.GetCombat(), damage);
            return;
        }
    }

    public Combat GetCombat()
    {
        return _combat;
    }

    //공격메ㅔ서드
    //애니메이션 실행, 움직임


    float _attackCooldown = 0f;
    public bool Attack()
    {
        if(_attackCooldown >= 0f)
        {
            return false;
        }
        _animator.SetTrigger("Attack");
        Vector3 dir = _detector.GetTarget().position - transform.position;
        float force = dir.magnitude * 10f;
        dir = dir.normalized;
        _rigidbody.AddForce(dir * force, ForceMode.VelocityChange);
        _attackCooldown = 3f;
        return true;
    }

    public void EnableAttackCollider()
    {
        _attackCollider.enabled = true;
    }

    public void TakeDamage(Combat attacker, float damage)
    {
        _combat.TakeDamage(attacker, damage);
    }

    private void OnDead(Combat attacker, Combat damaged)
    {
        gameObject.SetActive(false);
    }

    private void ResetEnemy()
    {
        _combat.ResetDead();
        gameObject.SetActive(true);
    }

    public bool IsMovable()
    {
        return _isMovable;
    }

    public bool IsTargetNear(float range)
    {
        Transform target = _detector.GetTarget();

        if(target == null)
        {
            return false;
        }
        if(Vector3.Distance(target.position, transform.position) <= range)
        {
            return true;
        }
        return false;
    }

    public Transform GetTarget()
    {
        return _detector.GetTarget();
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
