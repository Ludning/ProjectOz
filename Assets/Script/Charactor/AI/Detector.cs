using System;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private string _targetTag;
    private float _detectionRadius;
    private bool _detectThroughWall;

    private Vector3 _lastValidPostion;

    public bool isPlayerInRange { get; private set; }

    [SerializeField] private Transform Target
    {         
        get => _target;
        set
        {
            if(value != null)
            {
                _lastTarget = _target;
            }
            _target = value;
        }
    }
    private Transform _target;

    private Transform _lastTarget;


    public void Init(string targetTag, float detectionRadius, bool detectThroughWall)
    {
        _targetTag = targetTag;
        _detectionRadius = detectionRadius;
        _detectThroughWall = detectThroughWall;
    }

    public Transform GetTarget()
    {
        return Target;
    }

    public void FixedUpdate()
    {
        Collider[] overlap = Physics.OverlapSphere(transform.position, _detectionRadius);

        //check overlap contains player taged

        Collider col = overlap.FirstOrDefault((col) => col.CompareTag("Player"));
        if (col != null)
        {
            isPlayerInRange = _detectThroughWall ? true : IsTargetVisible();
        }
        else
        {
            isPlayerInRange = false;
        }
        if(isPlayerInRange)
        {
            Target = col.transform;
            _lastValidPostion = Target.position;
        }
        else
        {
            Target = null;
        }
    }
    public bool IsTargetVisible()
    {
        if(Target == null)
        {
            return false;
        }
        return IsTargetVisible(Target);
    }
    public bool IsTargetVisible(Transform target)
    {
        if (Physics.Raycast(transform.position, 
            target.position - (transform.position),
            out RaycastHit hit, _detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
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

    public Vector3 GetPosition()
    {
        return _lastValidPostion;
    }

    public Transform GetLastTarget()
    {
        return _lastTarget;
    }
}
