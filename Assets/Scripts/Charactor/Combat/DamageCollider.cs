using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if(other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(null, 30f);
            }
        }
    }
}
