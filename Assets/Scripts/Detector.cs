using System;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private string _targetTag;

    private float _detectionRadius;

    public bool isPlayerInRange { get; private set; }

    [SerializeField] private Transform _target;

    public void Init(string targetTag, float detectionRadius)
    {

        _targetTag = targetTag;
        _detectionRadius = detectionRadius;
    }

    public Transform GetTarget()
    {
        return _target;
    }

    public void FixedUpdate()
    {
        Collider[] overlap = Physics.OverlapSphere(transform.position, _detectionRadius);

        //check overlap contains player taged

        Collider col = overlap.FirstOrDefault((col) => col.CompareTag("Player"));
        if (col != null)
        {
            IsPlayerVisible(col.transform);
            _target = col.transform;
        }
        else
        {
            isPlayerInRange = false;
        }
    }
    private bool IsPlayerVisible(Transform target)
    {
        if (Physics.Raycast(target.position + transform.up, transform.position - target.position + transform.up, out RaycastHit hit, _detectionRadius))
        {
            if (hit.collider.CompareTag("Player") && target.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
