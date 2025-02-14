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

    [SerializeField] private Collider Target
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
    private Collider _target;

    private Collider _lastTarget;

    private int _characterColliderLayer;
    private int _passableGroundLayer;
    private int _impassableGroundLayer;
    public void Init(string targetTag, float detectionRadius, bool detectThroughWall)
    {
        _targetTag = targetTag;
        _detectionRadius = detectionRadius;
        _detectThroughWall = detectThroughWall;
        _characterColliderLayer = LayerMask.GetMask("Character_Collider");
        _passableGroundLayer = LayerMask.GetMask("Terrain_Passable");
        _impassableGroundLayer = LayerMask.GetMask("Terrain_Impassable");
    }

    public Transform GetTarget()
    {
        if(Target == null)
        {
            return null;
        }
        return Target.transform;
    }

    public void FixedUpdate()
    {
        Collider[] overlap = Physics.OverlapSphere(transform.position, _detectionRadius);

        //check overlap contains player taged

        Collider col = overlap.FirstOrDefault((col) => col.CompareTag("Player"));

        isPlayerInRange = col != null;

        if(isPlayerInRange)
        {
            Target = col;
            _lastValidPostion = Target.bounds.center;
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


    public bool IsTargetVisible(Collider target)
    {
        Vector3 center = transform.position;
        Vector3 targetCenter = target.bounds.center;
        if (Physics.Raycast(center, 
            targetCenter - (center),
            out RaycastHit hit, _detectionRadius, _characterColliderLayer | _passableGroundLayer | _impassableGroundLayer))
        {
            Debug.DrawLine(center, hit.point, Color.magenta);
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        else
        {
            Debug.DrawRay(center, targetCenter - (center), Color.green);
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
        if(_lastTarget == null)
        {
            return null;
        }
        return _lastTarget.transform;
    }
}
