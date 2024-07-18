using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class ScaleController : OzMagic
{
    private OzmagicData _timeStopData_OzMagic;

    PlayerStat pStat;
    Rigidbody[] allRigidbodies;
    Animator[] allAnimators;
    NavMeshAgent[] allNavMeshAgents;    

    [SerializeField] float ozTimestopDuration;
    [SerializeField] float ozTimestopRate;
    [SerializeField] float ozTimestopChainDuration;

    private bool _isStop = false;

    private void Awake()
    {
        _timeStopData_OzMagic = DataManager.Instance.GetGameData<OzmagicData>("O201");

        _ozMagicPercentage = _timeStopData_OzMagic.ozSkillPercentage;
        ozTimestopDuration = _timeStopData_OzMagic.value1;
        ozTimestopRate = _timeStopData_OzMagic.value2;
        ozTimestopChainDuration = _timeStopData_OzMagic.value3;

        SearchAllMovement();
    }
    
    public void SearchAllMovement()
    {
        allRigidbodies = FindObjectsOfType<Rigidbody>();
        allAnimators = FindObjectsOfType<Animator>();
        allNavMeshAgents = FindObjectsOfType<NavMeshAgent>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        pStat = FindFirstObjectByType<PlayerStat>();
        CancelInvoke(nameof(DestroyOzMagic));
    }

    public override void Excute()
    {
        Time.timeScale = ozTimestopRate;
        Invoke(nameof(EndofTimeStop), ozTimestopDuration * ozTimestopRate);
    }
    
    void EndofTimeStop()
    {
        Time.timeScale = 1f;
    }

    public void OnStop()
    {
        if (!_isStop)
        {
            PauseAllExceptPlayer(gameObject);
            _isStop = !_isStop;
        }
        else
        {
            ResumeAll();
            _isStop = !_isStop;
        }
    }

    private void PauseAllExceptPlayer(GameObject gameObject)
    {
        // Rigidbody 
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Animator 
        foreach (Animator animator in allAnimators)
        {
            if (animator.gameObject != gameObject)
            {
                animator.enabled = false;
            }
        }

        // NavMeshAgent ������Ʈ�� ����
        foreach (NavMeshAgent agent in allNavMeshAgents)
        {
            if (agent.gameObject != gameObject)
            {
                agent.isStopped = true;
            }
        }
    }

    private void ResumeAll()
    {
        // Rigidbody ������Ʈ�� �ٽ� ����
        foreach (Rigidbody rb in allRigidbodies)
        {
            rb.isKinematic = false;
        }

        foreach (Animator animator in allAnimators)
        {
            animator.enabled = true;
        }

        foreach (NavMeshAgent agent in allNavMeshAgents)
        {
            agent.isStopped = false;
        }
    }
}
