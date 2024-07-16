using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class ScaleController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    private bool _isStop = false;
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

    //해당 오브젝트를 제외한 것들의 정지.
    private void PauseAllExceptPlayer(GameObject gameObject)
    {
        // Rigidbody 컴포넌트를 멈춤
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
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
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        foreach (Animator animator in allAnimators)
        {
            if (animator.gameObject != gameObject)
            {
                animator.enabled = false;
            }
        }

        // NavMeshAgent 컴포넌트를 멈춤
        NavMeshAgent[] allNavMeshAgents = FindObjectsOfType<NavMeshAgent>();
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
        Rigidbody[] allRigidbodies = FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in allRigidbodies)
        {
            rb.isKinematic = false;
        }

        // Animator 컴포넌트를 다시 시작
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        foreach (Animator animator in allAnimators)
        {
            animator.enabled = true;
        }

        // NavMeshAgent 컴포넌트를 다시 시작
        NavMeshAgent[] allNavMeshAgents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in allNavMeshAgents)
        {
            agent.isStopped = false;
        }
    }
}
