using System;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    [SerializeField] private Enemy _owner;
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
    public void Init(Enemy owner ,string targetTag, float detectionRadius, bool detectThroughWall)
    {
        _owner = owner;
        _targetTag = targetTag;
        _detectionRadius = detectionRadius;
        _detectThroughWall = detectThroughWall;
        _characterColliderLayer = LayerMask.GetMask("Character_Collider");
        _passableGroundLayer = LayerMask.GetMask("Terrain_Passable");
        _impassableGroundLayer = LayerMask.GetMask("Terrain_Impassable");
    }

    private void OnValidate()
    {
        if (_owner == null)
        {
            _owner = transform.parent.GetComponent<Enemy>();
        }
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
        Collider[] overlap = Physics.OverlapSphere(transform.position, _detectionRadius, LayerMask.GetMask("Character_Collider"));

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

    private void OnDrawGizmosSelected()
    {
        _owner.EnableDebug();
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

    internal Vector3 GetLastPosition()
    {
        return _lastTarget.bounds.center;
    }
}
