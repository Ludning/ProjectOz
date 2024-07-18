using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class ScaleController : OzMagic
{
    [SerializeField] private PlayerMovement playerMovement;
    
    Rigidbody[] allRigidbodies;
    Animator[] allAnimators;
    NavMeshAgent[] allNavMeshAgents;

    [SerializeField] float ozTimestopChainDuration = 5f;
    [SerializeField] float ozTimestopRate = 0.1f;

    private bool _isStop = false;

    private void Start()
    {
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
        CancelInvoke(nameof(DestroyOzMagic));
    }

    public override void Excute()
    {
        Time.timeScale = ozTimestopRate;
        Invoke(nameof(InitTimeScale), _lifeTime);
    }
    
    void InitTimeScale()
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

    //ÇØ´ç ¿ÀºêÁ§Æ®¸¦ Á¦¿ÜÇÑ °ÍµéÀÇ Á¤Áö.
    private void PauseAllExceptPlayer(GameObject gameObject)
    {
        // Rigidbody ÄÄÆ÷³ÍÆ®¸¦ ¸ØÃã
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Animator ÄÄÆ÷³ÍÆ®¸¦ ¸ØÃã
        foreach (Animator animator in allAnimators)
        {
            if (animator.gameObject != gameObject)
            {
                animator.enabled = false;
            }
        }

        // NavMeshAgent ÄÄÆ÷³ÍÆ®¸¦ ¸ØÃã
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
        // Rigidbody ÄÄÆ÷³ÍÆ®¸¦ ´Ù½Ã ½ÃÀÛ
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
