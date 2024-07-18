using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,
    Patrol,
    Wait,
    Chase,
    JustLostTarget,
    Attack,
    Dead
}

[Serializable]
public class EnemyEditorData
{
    public float AttackRange = 2f;
    public float AttackCooldown = 2f;
    public float ChargeAttackForce = 80f;
    public float EnemyPatrolDistance = 4f;
    public float EnemyPatrolIdleDuration = 1f;
    public float EnemyAlramDistance = 6f;
    public float EnemyAlramLimitTime = 2f;
    //public float EnemyChaseDistance = 9f;
    public bool DetectThroughWall = false;
    public bool CanFireProjectile = false;
    public Transform ProjectileFirePos;
    public GameObject ProjectilePrefab;
    public bool CustomPatrolPoint = false;
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
    private float _attackDamage;
    private float _colDamage;
    private DamageBox _attackCollider;
    public event Action OnKnockbackEnd;
    private bool _isFlying = false;

    private AIState _aiState = AIState.Idle;

    private float _currentStateTime = 0f;
    public float CurrentStateTime => _currentStateTime;


    private Collider _characterCollider;
    private Collider _characterEnvCollider;

    private void Awake()
    {
        _enemyData = DataManager.Instance.GetGameData<EnemyData>(_enemyId);

        _rigidbody = GetComponent<Rigidbody>();
        _behaviorTree = GetComponent<BehaviorTree>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _combat = GetComponent<Combat>();
        _animator = GetComponent<Animator>();
        _attackCollider = GetComponentInChildren<DamageBox>();

        _characterCollider = GetComponent<Collider>();
        _characterEnvCollider = GetComponentInChildren<Collider>();

        _navMeshAgent.updateRotation = false;

        Init(_enemyData);
    }

    Quaternion look;
    private void Update()
    {
        if(_combat.IsDead())
        {
            return;
        }
        _currentStateTime += Time.deltaTime;
        if (_currentAttackTime > 0f)
        {
            _currentAttackTime -= Time.deltaTime;
        }
        Vector3 dir = _navMeshAgent.destination - transform.position;
        dir = dir.normalized;
        if (_detector.GetTarget() != null && _aiState == AIState.Chase)
        {
            if (_isFlying)
            {
                transform.rotation = Quaternion.LookRotation(_detector.GetPosition() - transform.position, Vector3.up);
            }
            else
            {
                Vector3 orig = transform.position; 
                Vector3 target = _detector.GetPosition();
                orig.y = 0;
                target.y = 0;
                transform.rotation = Quaternion.LookRotation(target - orig, Vector3.up);
            }
        }
        else if (Vector3.Distance(_navMeshAgent.destination, transform.position) > 2f)
        {
            Vector3 orig = transform.position;
            Vector3 target = _navMeshAgent.destination;
            if (!_isFlying)
            {
                orig.y = 0;
                target.y = 0;
            }
            look = Quaternion.LookRotation(target - orig, Vector3.up);
            transform.rotation = look;
        }
        else
        {
            transform.rotation = look;
        }

        if (_navMeshAgent.velocity.magnitude > 0.1f)
        {
            _animator.SetBool("IsMoving", true);
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    private void Init(EnemyData enemyData)
    {
        //Combat
        _combat.Init(enemyData.enemyHp);

        _combat.OnDamaged += OnDamaged;
        _combat.OnDead += OnDead;

        _isFlying = enemyData.enemyMoveType == "movetype_enemy_flying";

        _colDamage = enemyData.enemyColDamage * enemyData.enemyBasePower;
        _attackDamage = enemyData.enemyBasePower;
        _attackCooldown = _editorData.AttackCooldown;

        if(_editorData.CustomPatrolPoint == false)
        {
            float moveRange = _editorData.EnemyPatrolDistance;
            _leftPatrolPoint.position = transform.position + moveRange * Vector3.right;
            _rightPatrolPoint.position = transform.position - moveRange * Vector3.right;
        }

        _detector.Init(this, "Player",
            _editorData.EnemyAlramDistance,
            _editorData.DetectThroughWall);
        SharedTransformList targetList = new SharedTransformList();
        targetList.Value = new List<Transform>();
        targetList.Value.Add(_leftPatrolPoint);
        targetList.Value.Add(_rightPatrolPoint);

        SharedFloat attackRange = new SharedFloat();
        attackRange.Value = _editorData.AttackRange;
        SharedFloat detectRange = new SharedFloat();
        detectRange.Value = _editorData.EnemyAlramDistance;
        SharedFloat enemyAlramLimitTime = new SharedFloat();
        enemyAlramLimitTime.Value = _editorData.EnemyAlramLimitTime;
        SharedFloat enemyPatrolIdleDuration = new SharedFloat();
        enemyPatrolIdleDuration.Value = _editorData.EnemyPatrolIdleDuration;
        SharedFloat enemyChaseDistance = new SharedFloat();
        //enemyChaseDistance.Value = _editorData.EnemyChaseDistance;
        enemyChaseDistance.Value = 9999f;

        _behaviorTree.SetVariable("TargetList", targetList);
        _behaviorTree.SetVariable("AttackRange", attackRange);
        _behaviorTree.SetVariable("DetectRange", detectRange);
        _behaviorTree.SetVariable("EnemyAlramLimitTime", enemyAlramLimitTime);
        _behaviorTree.SetVariable("EnemyPatrolIdleDuration", enemyPatrolIdleDuration);
        _behaviorTree.SetVariable("ChaseRange", enemyChaseDistance);

    }
    private void ResetEnemy()
    {
        SetEnableAllCollision(true);
        _animator.SetBool("IsDead", false);

        _isMovable = true;
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
        transform.rotation = Quaternion.LookRotation(_detector.GetPosition() - transform.position, Vector3.up);
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
        StartCoroutine(AttackEnd(.4f));
        if (_isChargeAttack)
        {
            ChargeAttack(_editorData.ChargeAttackForce);
        }
        else if (_editorData.CanFireProjectile)
        {
            FireProjectile();
        }
        else
        {
            Attack();
        }
        return true;
    }

    private void FireProjectile()
    {

        GameObject projectile = Instantiate(_editorData.ProjectilePrefab, _editorData.ProjectileFirePos.position, _editorData.ProjectileFirePos.rotation);
        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        enemyProjectile.Init(_editorData.ProjectileFirePos);
        enemyProjectile.Fire();
    }

    private void ChargeAttack(float force)
    {
        Vector3 dir = _detector.GetPosition() + Vector3.up - transform.position;
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
    private IEnumerator AttackEnd(float delay)
    {
        yield return new WaitForFixedUpdate();
        IsMovable = true;
    }
    private void Attack()
    {
        if (_attackCollider == null)
            return;
        _attackCollider.SetDamage(_attackDamage);
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
        float dist = Vector3.Distance(_detector.GetPosition(), transform.position);
        if (dist <= range)
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
            target.Damaged(_colDamage);
            return;
        }
    }

    private void OnDamaged()
    {
    }

    private void OnDead()
    {
        SetEnableAllCollision(false);
        _animator.SetTrigger("Dead");
        _animator.SetBool("IsDead", true);
        _isMovable = false;
        StartCoroutine(DelayedDisable());
    }
    private void SetEnableAllCollision(bool condition)
    {
        _characterCollider.enabled = condition;

        if (_characterEnvCollider != null)
        {
            _characterEnvCollider.enabled = condition;
        }
    }
    private IEnumerator DelayedDisable()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

    private void SetEnableRigidbody(bool condition)
    {
        _navMeshAgent.velocity = Vector3.zero;
        _rigidbody.isKinematic = !condition;

        if (condition)
        {
            _navMeshAgent.updatePosition = false;
        }
        else
        {
            _navMeshAgent.updatePosition = true;
            _navMeshAgent.nextPosition = transform.position;
        }
    }

    public bool IsTargetVisible()
    {
        if (_editorData.DetectThroughWall)
        {
            return true;
        }
        return _detector.IsTargetVisible();
    }

    private void OnDrawGizmosSelected()
    {
        EnemyDebug();
    }

    private void OnDrawGizmos()
    {
        if (_debug)
        {
            EnemyDebug();
            _debug = false;
        }
    }
    private bool _debug = false;
    public void EnableDebug()
    {
        _debug = true;
    }
    private void EnemyDebug()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_detector.transform.position, _editorData.AttackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_detector.transform.position, _editorData.EnemyAlramDistance);

        Gizmos.color = GetColorByState(_aiState);
        Gizmos.DrawSphere(transform.position + Vector3.up, 1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_leftPatrolPoint.position, 1f);
        Gizmos.DrawSphere(_rightPatrolPoint.position, 1f);
    }

    private Color GetColorByState(AIState state)
    {
        if (_currentAttackTime > 0f)
        {
            return Color.red;
        }
        switch (state)
        {
            case AIState.Idle:
                return Color.green;
            case AIState.Patrol:
                return Color.blue;
            case AIState.Wait:
                return Color.cyan;
            case AIState.Chase:
                return Color.yellow;
            case AIState.JustLostTarget:
                return Color.gray;
            case AIState.Attack:
                return Color.red;
            case AIState.Dead:
                return Color.black;
            default:
                return Color.white;
        }
    }

    internal void SetState(AIState state)
    {
        _aiState = state;
        _currentStateTime = 0f;
    }

    public Transform GetLastTarget()
    {
        return _detector.GetLastTarget();
    }

    internal Vector3 GetTargetPosition()
    {
        return _detector.GetPosition();
    }

    internal Vector3 GetLastTargetPosition()
    {
        return _detector.GetLastPosition();
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
