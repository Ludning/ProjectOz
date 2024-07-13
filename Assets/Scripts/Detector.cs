using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private string _targetTag;

    private float _detectionRadius;

    public bool isPlayerInRange { get; private set; }

    [SerializeField] private Transform _target;
    public void Init(string targetTag ,float detectionRadius)
    {
        _targetTag = targetTag; 
        _detectionRadius = detectionRadius;
    }

    public Transform GetTarget()
    {
        return _target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            _target = other.transform;
            Debug.Log("Player detected");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Player left");
        }
    }
}
