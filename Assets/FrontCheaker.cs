using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontCheaker : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    private int count = 0;
    public bool IsFrontEmpty => (count == 0) ? true : false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == _layerMask)
        {
            count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _layerMask)
        {
            count--;
        }
    }
}
