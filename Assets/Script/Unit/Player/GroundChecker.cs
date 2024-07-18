using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [Header("Boxcast Property")]
    [SerializeField] private Vector3 spherePosition;
    //[SerializeField] private float maxDistance;
    [SerializeField] private Vector3 boxSize;
    [SerializeField] private LayerMask groundLayer;

    [Header("Debug")]
    [SerializeField] private bool drawGizmo;

    private void OnDrawGizmos()
    {
        if (!drawGizmo) return;

        Gizmos.color = Color.cyan;
        //Gizmos.DrawSphere(transform.position + spherePosition, maxDistance);
        Gizmos.DrawCube(transform.position + spherePosition, boxSize);
    }

    public bool IsGrounded()
    {
        //return Physics.CheckSphere(transform.position + spherePosition, maxDistance, groundLayer);
        return Physics.CheckBox(transform.position + spherePosition, boxSize/2, Quaternion.identity, groundLayer);
    }
}
