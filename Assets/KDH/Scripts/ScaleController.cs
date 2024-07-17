using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class ScaleController : OzMagic
{
    Rigidbody[] allRigidbodies;
    Animator[] allAnimators;
    NavMeshAgent[] allNavMeshAgents;

    float ozTimestopDuration = 3f; //시간정지 오즈매직 지속시간
    float ozTimestopRate = 0.1f; // 느려지는 애니메이션 배율
    float gageGainOz; //시간정지 오즈매직 게이지 획득량
    float ozSkillPercentage; //시간정지 오즈매직 발동 확률
    //float ozTimestopTarget; 타겟 필요없어보이는걸?

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

    public override void Excute()
    {
        Time.timeScale = ozTimestopRate;
        Invoke(nameof(EndofTimeStop), ozTimestopDuration);
    }

    void EndofTimeStop()
    {
        Time.timeScale = 1f;
    }

    //해당 오브젝트를 제외한 것들의 정지.
    private void PauseAllExceptPlayer(GameObject gameObject)
    {
        // Rigidbody 컴포넌트를 멈춤
        foreach (Rigidbody rb in allRigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }

        // Animator 컴포넌트를 멈춤
        foreach (Animator animator in allAnimators)
        {
            if (animator.gameObject != gameObject)
            {
                animator.enabled = false;
            }
        }

        // NavMeshAgent 컴포넌트를 멈춤
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
        // Rigidbody 컴포넌트를 다시 시작
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
