using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Boxcast Property")]
    [SerializeField] private Vector3 spherePosition;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position + spherePosition, maxDistance);
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(transform.position + spherePosition, maxDistance, groundLayer);
    }
}
