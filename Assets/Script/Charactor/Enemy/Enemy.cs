using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[Serializable]
public class EnemyEditorData
{
    public float AttackRange;
    public float AttackCooldown;
    public float AttackDamage;
    public float EnemyPatrolDistance;
    public float EnemyPatrolDuration;
    public float EnemyAlramDistance;
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Combat))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{

    [Header("AI_Patrol")]
    [SerializeField] private Transform _leftPatrolPoint;
    [SerializeField] private Transform _rightPatrolPoint;


    [SerializeField] private string _enemyId;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private bool _isMovable = true;
    public bool IsMovable
    {
        get => _isMovable;
        private set
        {
            _navMeshAgent.isStopped = !value;
            _isMovable = value;
        }
    }

    [SerializeField] private Detector _detector;

    [SerializeField] private bool _isChargeAttack = false;

    [SerializeField] private EnemyEditorData _editorData;
    [SerializeField] private Transform _rotateTarget;

    private static float positionZ = 0;

    private Combat _combat;
    private Rigidbody _rigidbody;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private BehaviorTree _behaviorTree;
    private float _damage;
    private DamageBox _attackCollider;
    public event Action OnKnockbackEnd;
    private bool _isFlying = false;

    private void Awake()
    {
        _enemyData = DataManager.Instance.GetGameData<EnemyData>(_enemyId);

        _rigidbody = GetComponent<Rigidbody>();
        _behaviorTree = GetComponent<BehaviorTree>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<Combat>();
        _animator = GetComponent<Animator>();
        _attackCollider = GetComponentInChildren<DamageBox>();

        _navMeshAgent.updateRotation = false;

        Init(_enemyData);
    }
    Quaternion look;
    private void Update()
    {
        if (_currentAttackTime > 0f)
        {
            _currentAttackTime -= Time.deltaTime;
        }
        Vector3 dir = _navMeshAgent.destination - transform.position;
        dir = dir.normalized;
        if (Vector3.Distance(_navMeshAgent.destination, transform.position) > 2f)
        {
            look = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = look;
        }
        else
        {

            transform.rotation = look;
        }
    }
    private void Init(EnemyData enemyData)
    {
        //Combat
        _combat.Init(enemyData.enemyHp);

        _combat.OnDamaged += OnDamaged;
        _combat.OnDead += OnDead;

        _isFlying = enemyData.enemyMoveType == "movetype_enemy_flying";

        _damage = enemyData.enemyColDamage * enemyData.enemyBasePower;
        //_attackCooldown = enemyData.enemyAttackCooldown;
        //_navMeshAgent.speed = enemyData.enemySpeed;
        //float moveRange = enemyData.enemyPatrolDistance;
        float moveRange = 3f;
        _leftPatrolPoint.position = transform.position + moveRange * Vector3.right;
        _rightPatrolPoint.position = transform.position - moveRange * Vector3.right;

        //_detector.SetRadius(enemyData.noticeDistance);
        _detector.Init("Player", 6f);
        SharedTransformList targetList = new SharedTransformList();
        targetList.Value = new List<Transform>();
        targetList.Value.Add(_leftPatrolPoint);
        targetList.Value.Add(_rightPatrolPoint);

        if (gameObject.CompareTag("Player"))
        {
            return;
        }
        SharedFloat attackRange = new SharedFloat();
        attackRange.Value = 2f;
        SharedFloat detectRange = new SharedFloat();
        detectRange.Value = 6f;
        _behaviorTree.SetVariable("TargetList", targetList);
        _behaviorTree.SetVariable("AttackRange", attackRange);
        _behaviorTree.SetVariable("DetectRange", detectRange);
    }
    private void ResetEnemy()
    {
        _combat.ResetDead();
        gameObject.SetActive(true);
    }






    //전투 관련
    public Combat GetCombat()
    {
        return _combat;
    }

    //공격메서드
    //애니메이션 실행, 움직임
    float _attackCooldown = 1f;
    float _currentAttackTime = 0f;

    public void StartAttackAnimation()
    {
        IsMovable = false;
        _currentAttackTime = _attackCooldown;
        _animator.SetTrigger("Attack");
    }
    public bool IsAttackable()
    {
        if (_currentAttackTime > 0f)
        {
            return false;
        }
        return true;
    }

    public bool CharacterAttack()
    {
        if (_isChargeAttack)
        {
            ChargeAttack(25f);
        }
        else
        {
            Attack();
        }
        return true;
    }
    private void ChargeAttack(float force)
    {
        Vector3 dir = _detector.GetTarget().position + Vector3.up - transform.position;
        SetEnableRigidbody(true);
        dir.z = 0f;
        dir = dir.normalized;
        _rigidbody.AddForce(dir * force, ForceMode.VelocityChange);
        StartCoroutine(ChargeAttackEnd());
        Attack();
    }
    private IEnumerator ChargeAttackEnd()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (_rigidbody.velocity.magnitude < .1f)
            {
                SetEnableRigidbody(false);
                break;
            }
        }
    }
    private void Attack()
    {
        IsMovable = false;
        StartCoroutine(AttackEnd());
        if (_attackCollider == null)
            return;
        _attackCollider.SetDamage(_damage);
        _attackCollider.enabled = true;
    }

    private IEnumerator AttackEnd()
    {
        yield return new WaitForSeconds(_attackCooldown);
        IsMovable = true;
    }

    public bool IsTargetNear(float range)
    {
        Transform target = _detector.GetTarget();

        if (target == null)
        {
            return false;
        }
        if (Vector3.Distance(target.position, transform.position) <= range)
        {
            return true;
        }
        return false;
    }

    public Transform GetTarget()
    {
        return _detector.GetTarget();
    }


    // 이벤트
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out Combat target) == false)
            {
                Debug.Assert(false, "Player has no Combat component");
                return;
            }
            target.Damaged(_damage);
            return;
        }
    }
    private void OnAttack(GameObject taget, float damage)
    {
        if (!taget.TryGetComponent(out Combat targetCombat))
        {
            return;
        }
        targetCombat.Damaged(damage);
    }

    private void OnDamaged()
    {
    }

    private void OnDead()
    {
        gameObject.SetActive(false);
    }

    private void SetEnableRigidbody(bool condition)
    {
        _navMeshAgent.velocity = Vector3.zero;
        _navMeshAgent.updatePosition = !condition;
        _rigidbody.isKinematic = !condition;
        _rigidbody.useGravity = false;
        if (!condition)
        {
            _navMeshAgent.nextPosition = transform.position;
        }
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
